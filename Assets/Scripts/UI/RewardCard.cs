using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class RewardCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private TextMeshProUGUI countText;
        
        private Tween _showTween;
        private Tween _inertiaTween;
        private Tween _bounceTween;
        private Tween _pressTween;
        private Tween _floatTween;
        private Vector3 _originalScale = Vector3.one;
        private RectTransform _rectTransform;
        private RectTransform _parentRectTransform;
        private Canvas _canvas;
        private Action _method;
        private Action _hideMethod;
        
        private Vector2 _lastDragPosition;
        private Vector2 _dragStartPosition;
        private Vector2 _dragVelocity;
        private Vector2 _currentInertiaVelocity;
        private bool _isDragging;
        private float _lastMovementTime;
        private Transform _endPoint;
        private bool _methodInvoked = false;
        private bool _hasBeenInteracted = false; 
        private Coroutine _inactivityTimerCoroutine;
        private Vector2 _pointerDownPosition;
        private bool _wasDragged = false;
        private bool _coinPulseSent = false;
        private const float INITIAL_INACTIVITY_TIMEOUT = 10f; 
        private const float INACTIVITY_TIMEOUT = 5f; 
        private const float MIN_VELOCITY = 30f; 
        private const float MIN_DRAG_DISTANCE = 5f; 
        private const float MOVEMENT_TIMEOUT = 0.1f; 
        private const int VELOCITY_SAMPLES = 3;
        private Vector2[] _velocityHistory = new Vector2[VELOCITY_SAMPLES];
        private int _velocityHistoryIndex = 0;
        private const float BOUNCE_DURATION = 0.3f;
        private const float DEFORM_SCALE = 0.85f; 
        private const float STRETCH_SCALE = 1.1f;
        private const float BOUNCE_COEFFICIENT = 0.75f;
        private const float INERTIA_FRICTION = 0.85f; 
        private const float FLOAT_RADIUS = 150f;
        private const float FLOAT_DURATION = 5f;
        private const float CLICK_MAX_DISTANCE = 10f;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            _parentRectTransform = _rectTransform.parent as RectTransform;
            
            transform.localScale = Vector3.zero;
        }

        public void Show(int count, Transform endPoint, Action method = null)
        {
            _method = method;
            _endPoint = endPoint;
            countText.text = $"+{count}";
            _coinPulseSent = false;
            
            _showTween?.Kill();
            
            transform.localScale = Vector3.zero;
            
            _showTween = DOTween.Sequence()
                .Append(transform.DOScaleY(_originalScale.y * 1.2f, 0.2f).SetEase(Ease.OutQuad))
                .Append(transform.DOScaleX(_originalScale.x * 1.15f, 0.2f).SetEase(Ease.OutQuad))
                .Join(transform.DOScaleY(_originalScale.y * 0.95f, 0.2f).SetEase(Ease.OutQuad))
                .Append(transform.DOScale(_originalScale, 0.2f).SetEase(Ease.OutBack)).OnComplete(() =>
                {
                    transform.localScale = _originalScale;
                    StartFloating();
                });
            
            _hasBeenInteracted = false;
            ResetInactivityTimer(INITIAL_INACTIVITY_TIMEOUT);
        }

        public void Hide(Action method = null)
        {
            _methodInvoked = false;
            _hideMethod = method;
            
            StopInactivityTimer();
            
            _showTween?.Kill();
            _inertiaTween?.Kill();
            _bounceTween?.Kill();
            _pressTween?.Kill();
            _floatTween?.Kill();
            
            if (_endPoint != null)
            {
                RectTransform endPointRect = _endPoint.GetComponent<RectTransform>();
                
                Vector2 endPosition;
                if (_endPoint.parent == _rectTransform.parent)
                {
                    endPosition = endPointRect.anchoredPosition;
                }
                else
                {
                    Camera cam = _canvas.worldCamera ?? Camera.main;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        _parentRectTransform,
                        RectTransformUtility.WorldToScreenPoint(cam, endPointRect.position),
                        cam,
                        out endPosition);
                }
                
                _showTween = DOTween.Sequence()
                    .Append(_rectTransform.DOAnchorPos(endPosition, 0.3f).SetEase(Ease.InQuad))
                    .Join(transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)).OnComplete(() =>
                    {
                        if (!_coinPulseSent)
                        {
                            CoinsScale.RequestPulse();
                            _coinPulseSent = true;
                        }

                        if (!_methodInvoked)
                        {
                            _hideMethod?.Invoke();
                            _method?.Invoke();
                            _methodInvoked = true;
                            
                        }
                        Destroy(gameObject);
                    });
            }
            else
            {
                _showTween = transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    if (!_coinPulseSent)
                    {
                        CoinsScale.RequestPulse();
                        _coinPulseSent = true;
                    }

                    if (!_methodInvoked)
                    {
                        _hideMethod?.Invoke();
                        _method?.Invoke();
                        _methodInvoked = true;
                    }
                    Destroy(gameObject);
                });
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnInteractionStart();
            
            _isDragging = true;
            _wasDragged = true;
            _inertiaTween?.Kill();
            _floatTween?.Kill();
            
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform.parent as RectTransform,
                eventData.position,
                _canvas.worldCamera,
                out localPoint);
            
            _lastDragPosition = localPoint;
            _dragStartPosition = _rectTransform.anchoredPosition;
            _dragVelocity = Vector2.zero;
            _lastMovementTime = Time.time;
            
            for (int i = 0; i < VELOCITY_SAMPLES; i++)
                _velocityHistory[i] = Vector2.zero;
            
            _velocityHistoryIndex = 0;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform.parent as RectTransform,
                eventData.position,
                _canvas.worldCamera,
                out localPoint);
            
            Vector2 deltaPosition = localPoint - _lastDragPosition;
            
            if (deltaPosition.magnitude > 0.01f)
            {
                _lastMovementTime = Time.time;
                
                Vector2 currentVelocity = deltaPosition / Time.deltaTime;
                _velocityHistory[_velocityHistoryIndex] = currentVelocity;
                _velocityHistoryIndex = (_velocityHistoryIndex + 1) % VELOCITY_SAMPLES;
                
                Vector2 maxVelocity = Vector2.zero;
                float maxMagnitude = 0f;
                for (int i = 0; i < VELOCITY_SAMPLES; i++)
                {
                    float magnitude = _velocityHistory[i].magnitude;
                    if (magnitude > maxMagnitude)
                    {
                        maxMagnitude = magnitude;
                        maxVelocity = _velocityHistory[i];
                    }
                }
                
                _dragVelocity = currentVelocity.magnitude > maxMagnitude ? currentVelocity : maxVelocity;
            }
            else
                _dragVelocity = Vector2.zero;
            
            
            Vector2 newPosition = _rectTransform.anchoredPosition + deltaPosition;
            Vector2 clampedPosition = ClampPositionToBounds(newPosition);
            _rectTransform.anchoredPosition = clampedPosition;
            
            _lastDragPosition = localPoint;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
            
            Vector2 totalDragDistance = _rectTransform.anchoredPosition - _dragStartPosition;
            float dragDistanceMagnitude = totalDragDistance.magnitude;
            
            float timeSinceLastMovement = Time.time - _lastMovementTime;

            if (_dragVelocity.magnitude > MIN_VELOCITY && dragDistanceMagnitude > MIN_DRAG_DISTANCE && timeSinceLastMovement < MOVEMENT_TIMEOUT)
            {
                ApplyInertia(_dragVelocity);
            }
            else
            {
                _dragVelocity = Vector2.zero;
                StartFloating();
            }
        }

        private void ApplyInertia(Vector2 initialVelocity)
        {
            _currentInertiaVelocity = initialVelocity;
            ApplyInertiaInternal(initialVelocity);
        }

        private void ApplyInertiaInternal(Vector2 velocity)
        {
            Vector2 currentVelocity = velocity;
            Vector2 lastVelocity = velocity;
            float duration = Mathf.Clamp(currentVelocity.magnitude / 200f, 0.2f, 3f);
            float lastTime = Time.time;

            _inertiaTween = DOTween.To(
                () => 0f,
                t =>
                {
                    float now = Time.time;
                    float delta = now - lastTime;
                    lastTime = now;
                    if (delta <= 0f) return;

                    float frictionFactor = Mathf.Pow(INERTIA_FRICTION, delta);
                    currentVelocity *= frictionFactor;
                    lastVelocity = currentVelocity;

                    if (currentVelocity.magnitude < MIN_VELOCITY * 0.5f)
                    {
                        _inertiaTween?.Kill();
                        StartFloating();
                        return;
                    }

                    Vector2 nextPos = _rectTransform.anchoredPosition + currentVelocity * delta;
                    Vector2 bounds = GetBounds();
                    Vector2 clampedPos = ClampPositionToBounds(nextPos);

                    bool hitBoundary = false;
                    bool isHorizontalHit = false;
                    bool isTopHit = false;

                    if (Mathf.Abs(nextPos.x - clampedPos.x) > 0.01f)
                    {
                        hitBoundary = true;
                        isHorizontalHit = true;
                    }
                    else if (Mathf.Abs(nextPos.y - clampedPos.y) > 0.01f)
                    {
                        hitBoundary = true;
                        isHorizontalHit = false;
                        isTopHit = nextPos.y > bounds.y;
                    }

                    if (hitBoundary)
                    {
                        _inertiaTween?.Kill();
                        if (isTopHit)
                        {
                            Hide();
                            return;
                        }
                        HandleBoundaryHit(isHorizontalHit, currentVelocity);
                        return;
                    }

                    _rectTransform.anchoredPosition = clampedPos;
                },
                1f, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (lastVelocity.magnitude > MIN_VELOCITY * 0.5f)
                    {
                        ApplyInertiaInternal(lastVelocity);
                    }
                    else
                    {
                        StartFloating();
                    }
                });
        }

        private Vector2 GetBounds()
        {
            if (_parentRectTransform == null) return Vector2.zero;
            
            Rect parentRect = _parentRectTransform.rect;
            Rect cardRect = _rectTransform.rect;
            
            float halfWidth = cardRect.width * 0.5f;
            float halfHeight = cardRect.height * 0.5f;
            
            float bottomBound = (parentRect.height * 0.5f - halfHeight) * 0.75f;
            
            return new Vector2(
                parentRect.width * 0.5f - halfWidth,
                bottomBound
            );
        }

        private Vector2 ClampPositionToBounds(Vector2 position)
        {
            Vector2 bounds = GetBounds();
            
            return new Vector2(
                Mathf.Clamp(position.x, -bounds.x, bounds.x),
                Mathf.Clamp(position.y, -bounds.y, bounds.y)
            );
        }

        private void HandleBoundaryHit(bool isHorizontalHit, Vector2 incomingVelocity)
        {
            _inertiaTween?.Kill();
            
            float incomingSpeed = incomingVelocity.magnitude;
            float maxBounceSpeed = 400f;
            if (incomingSpeed > maxBounceSpeed)
                incomingVelocity = incomingVelocity.normalized * maxBounceSpeed;

            Vector2 reflectedVelocity = incomingVelocity;
            reflectedVelocity = isHorizontalHit ? 
                new Vector2(-incomingVelocity.x * BOUNCE_COEFFICIENT, incomingVelocity.y * BOUNCE_COEFFICIENT) : 
                new Vector2(incomingVelocity.x * BOUNCE_COEFFICIENT, -incomingVelocity.y * BOUNCE_COEFFICIENT);

            _currentInertiaVelocity = reflectedVelocity;
            
            if (_currentInertiaVelocity.magnitude > 5f)
                ApplyInertiaInternal(_currentInertiaVelocity);
            else
                StartFloating();
            
            
            _bounceTween?.Kill();
            
            if (isHorizontalHit)
            {
                _bounceTween = DOTween.Sequence()
                    .Append(transform.DOScaleX(DEFORM_SCALE, BOUNCE_DURATION * 0.3f).SetEase(Ease.OutQuad))
                    .Join(transform.DOScaleY(STRETCH_SCALE, BOUNCE_DURATION * 0.3f).SetEase(Ease.OutQuad))
                    .Append(transform.DOScale(_originalScale, BOUNCE_DURATION * 0.7f).SetEase(Ease.OutBack));
            }
            else
            {
                _bounceTween = DOTween.Sequence()
                    .Append(transform.DOScaleY(DEFORM_SCALE, BOUNCE_DURATION * 0.3f).SetEase(Ease.OutQuad))
                    .Join(transform.DOScaleX(STRETCH_SCALE, BOUNCE_DURATION * 0.3f).SetEase(Ease.OutQuad))
                    .Append(transform.DOScale(_originalScale, BOUNCE_DURATION * 0.7f).SetEase(Ease.OutBack));
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnInteractionStart();
            
            _wasDragged = false;
            _floatTween?.Kill();
            _inertiaTween?.Kill();
            
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform.parent as RectTransform,
                eventData.position,
                _canvas.worldCamera,
                out localPoint);
            _pointerDownPosition = localPoint;
            
            _pressTween?.Kill();
            _pressTween = transform.DOScale(_originalScale * 0.9f, 0.1f).SetEase(Ease.OutQuad);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _pressTween?.Kill();
            _pressTween = transform.DOScale(_originalScale, 0.1f).SetEase(Ease.OutQuad);
            
            if (!_wasDragged && !_isDragging)
            {
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform.parent as RectTransform, eventData.position, _canvas.worldCamera, out localPoint);
                
                float clickDistance = Vector2.Distance(_pointerDownPosition, localPoint);
                if (clickDistance < CLICK_MAX_DISTANCE)
                    Hide();
            }
        }

        private void OnInteractionStart()
        {
            if (!_hasBeenInteracted)
            {
                _hasBeenInteracted = true;
                ResetInactivityTimer(INACTIVITY_TIMEOUT);
            }
            else
            {
                ResetInactivityTimer(INACTIVITY_TIMEOUT);
            }
        }
        
        private void ResetInactivityTimer(float timeout)
        {
            StopInactivityTimer();
            _inactivityTimerCoroutine = StartCoroutine(InactivityTimerCoroutine(timeout));
        }
        
        private void StopInactivityTimer()
        {
            if (_inactivityTimerCoroutine != null)
            {
                StopCoroutine(_inactivityTimerCoroutine);
                _inactivityTimerCoroutine = null;
            }
        }
        
        private IEnumerator InactivityTimerCoroutine(float timeout)
        {
            yield return new WaitForSeconds(timeout);
            Hide();
        }
        
        private void StartFloating()
        {
            if (_isDragging) return;
            
            _floatTween?.Kill();
            
            Vector2 startPos = _rectTransform.anchoredPosition;
            Vector2 bounds = GetBounds();
            
            float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = UnityEngine.Random.Range(FLOAT_RADIUS * 0.5f, FLOAT_RADIUS);
            Vector2 targetPos = startPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            targetPos = ClampPositionToBounds(targetPos);
            
            _floatTween = DOTween.To(
                () => _rectTransform.anchoredPosition,
                pos =>
                {
                    Vector2 clampedPos = ClampPositionToBounds(pos);
                    _rectTransform.anchoredPosition = clampedPos;
                },
                targetPos,
                FLOAT_DURATION)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    StartFloating();
                });
        }

        private void OnDestroy()
        {
            StopInactivityTimer();
            
            if (!_methodInvoked)
            {
                _hideMethod?.Invoke();
                _method?.Invoke();
                _methodInvoked = true;
            }
            
            _showTween?.Kill();
            _inertiaTween?.Kill();
            _bounceTween?.Kill();
            _pressTween?.Kill();
            _floatTween?.Kill();
        }
    }
}
