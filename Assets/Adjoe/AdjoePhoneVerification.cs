using UnityEngine;
using System;

namespace io.adjoe.sdk
{
    /// <summary>
    /// Provides methods to verify a user's phone number.
    /// </summary>
    public class AdjoePhoneVerification
    {

        private AndroidJavaObject pv;

        /// <summary>
        /// Creates a new instance of this class and initializes the phone verification.
        /// </summary>
        /// <param name="appHash">The app hash.</param>
        public AdjoePhoneVerification(string appHash)
        {
            #if UNITY_EDITOR
                Debug.Log("Called AdjoePhoneVerification(appHash=" + appHash + ")");
                return;
            #endif

            pv = new AndroidJavaObject("io.adjoe.sdk.AdjoePhoneVerification", appHash, null);
        }

        /// <summary>
        /// Creates a new instance of this class and initializes the phone verification.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public AdjoePhoneVerification(Callback callback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called AdjoePhoneVerification(callback=" + callback + ")");
                return;
            #endif

            pv = new AndroidJavaObject("io.adjoe.sdk.AdjoePhoneVerification", null, new CallbackWrapper(callback));
        }

        /// <summary>
        /// Creates a new instance of this class and initializes the phone verification.
        /// </summary>
        /// <param name="appHash">The app hash.</param>
        /// <param name="callback">The callback.</param>
        public AdjoePhoneVerification(string appHash, Callback callback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called AdjoePhoneVerification(appHash=" + appHash + ", callback=" + callback + ")");
                return;
            #endif

            pv = new AndroidJavaObject("io.adjoe.sdk.AdjoePhoneVerification", appHash, new CallbackWrapper(callback));
        }

        /// <summary>
        /// Sets the app hash.
        /// </summary>
        /// <remarks>
        /// Only required if you have not passed it to the constructor.
        /// </remarks>
        /// <param name="appHash">The app hash.</param>
        public void SetAppHash(string appHash)
        {
            #if UNITY_EDITOR
                Debug.Log("Called AdjoePhoneVerification.SetAppHash(appHash=" + appHash + ")");
                return;
            #endif

            if (pv != null)
            {
                pv.Call("setAppHash", appHash);
            }
        }

        /// <summary>
        /// Sets the callback which is used during <c>AdjoePhoneVerification.StartManual</c>, <c>AdjoePhoneVerification.StartAutomatic</c> and <c>AdjoePhoneVerification.StartAutomaticWithPhoneNumber</c>.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public void SetCheckCallback(CheckCallback callback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called AdjoePhoneVerification.SetCheckCallback(callback=" + callback + ")");
                return;
            #endif

            if (pv != null)
            {
                pv.Call("setCheckCallback", new CheckCallbackWrapper(callback));
            }
        }

        /// <summary>
        /// Sets the callback which is used during <c>AdjoePhoneVerification.StartManual</c>, <c>AdjoePhoneVerification.StartAutomatic</c> and <c>AdjoePhoneVerification.StartAutomaticWithPhoneNumber</c>.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public void SetVerifyCallback(VerifyCallback callback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called AdjoePhoneVerification.SetVerifyCallback(callback=" + callback + ")");
                return;
            #endif

            if (pv != null)
            {
                pv.Call("setVerifyCallback", new VerifyCallbackWrapper(callback));
            }
        }

        /// <summary>
        /// Checks whether the phone number is already verified.
        /// </summary>
        /// <param name="callback">A callback which is invoked after the check has finished.</param>
        public void GetStatus(StatusCallback callback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called AdjoePhoneVerification.GetStatus(callback=" + callback + ")");
                return;
            #endif

            if (pv != null)
            {
                pv.Call("getStatus", GetCurrentContext(), new StatusCallbackWrapper(callback));
            }
        }

        /// <summary>
        /// Starts the manual phone verification process.
        /// </summary>
        /// <param name="phoneNumber">The phone number to verify.</param>
        public void StartManual(string phoneNumber)
        {
            #if UNITY_EDITOR
                Debug.Log("Called AdjoePhoneVerification.StartManual(phoneNumber=" + phoneNumber + ")");
                return;
            #endif

            if (pv != null)
            {
                pv.Call("startManual", GetCurrentContext(), phoneNumber);
            }
        }

        /// <summary>
        /// Starts the automatic phone verification process.
        /// </summary>
        /// <remarks>
        /// During this process methods from the <c>Callback</c> that you have set in the constructor and the callbacks set via <c>AdjoePhoneVerification.SetCheckCallback</c> and <c>AdjoePhoneVerification.SetVerifyCallback</c> will be invoked.
        /// </remarks>
        /// <param name="fragmentActivity">An <c>AndroidJavaObject</c> which holds a reference to a <c>FragmentActivity</c>.</param>
        public void StartAutomatic(AndroidJavaObject fragmentActivity)
        {
            #if UNITY_EDITOR
                Debug.Log("Called AdjoePhoneVerification.StartAutomatic(fragmentActivity=" + fragmentActivity + ")");
                return;
            #endif

            if (pv != null)
            {
                pv.Call("startAutomatic", fragmentActivity);
            }
        }

        /// <summary>
        ///        /// Starts the automatic phone verification process.
        /// </summary>
        /// <remarks>
        /// During this process methods from the <c>Callback</c> that you have set in the constructor and the callbacks set via <c>AdjoePhoneVerification.SetCheckCallback</c> and <c>AdjoePhoneVerification.SetVerifyCallback</c> will be invoked.
        /// </remarks>
        /// <param name="activity">An <c>AndroidJavaObject</c> which holds a reference to an <c>Activity</c>.</param>
        /// <param name="googleApiClient">An <c>AndroidJavaObject</c> which holds a reference to a <c>GoogleApiClient</c>.</param>
        public void StartAutomatic(AndroidJavaObject activity, AndroidJavaObject googleApiClient)
        {
            #if UNITY_EDITOR
                Debug.Log("Called AdjoePhoneVerification.StartAutomatic(activity=" + activity + ", googleApiClient=" + googleApiClient + ")");
                return;
            #endif

            if (pv != null)
            {
                pv.Call("startAutomatic", activity, googleApiClient);
            }
        }

        /// <summary>
        /// Starts the automatic phone verification process for a given phone number.
        /// </summary>
        /// <remarks>
        /// During this process methods from the <c>Callback</c> that you have set in the constructor and the callbacks set via <c>AdjoePhoneVerification.SetCheckCallback</c> and <c>AdjoePhoneVerification.SetVerifyCallback</c> will be invoked.
        /// </remarks>
        /// <param name="phoneNumber">The phone number.</param>
        public void StartAutomaticWithPhoneNumber(string phoneNumber)
        {
            #if UNITY_EDITOR
                Debug.Log("AdjoePhoneVerification.StartAutomaticWithPhoneNumber(phoneNumber=" + phoneNumber + ")");
                return;
            #endif

            if (pv != null)
            {
                pv.Call("startAutomaticWithPhoneNumber", GetCurrentContext(), phoneNumber);
            }
        }

        /// <summary>
        /// Verifies a one-time verification code that the user has received in a SMS.
        /// </summary>
        /// <param name="code">The code.</param>
        public void VerifyCode(string code)
        {
            #if UNITY_EDITOR
                Debug.Log("Called AdjoePhoneVerification.VerifyCode(code=" + code + ")");
                return;
            #endif

            if (pv != null)
            {
                pv.Call("verifyCode", GetCurrentContext(), code);
            }
        }

        /// <summary>
        /// Call this method from your Android <c>Activity.onActivityResult</c> method.
        /// </summary>
        /// <param name="activity">The <c>Activity</c> from which this method is called.</param>
        /// <param name="requestCode">The <c>requestCode</c> parameter from <c>Activity.onActivityResult</c>.</param>
        /// <param name="resultCode">The <c>resultCode</c> parameter from <c>Activity.onActivityResult</c>.</param>
        /// <param name="data">The <c>data</c> parameter from <c>Activity.onActivityResult</c>.</param>
        public void OnActivityResult(AndroidJavaObject activity, int requestCode, int resultCode, AndroidJavaObject data)
        {
            #if UNITY_EDITOR
                Debug.Log("Called AdjoePhoneVerification.OnActivityResult(activity=" + activity + ", requestCode=" + requestCode + ", resultCode=" + resultCode + ", data=" + data + ")");
                return;
            #endif

            if (pv != null)
            {
                pv.Call("onActivityResult", activity, requestCode, resultCode, data);
            }
        }

        /// <summary>
        /// Call this method from your Android <c>Activity.onResume</c> method.
        /// </summary>
        /// <param name="activity">The <c>Activity</c> from which this method is called.</param>
        public void OnResume(AndroidJavaObject activity)
        {
            #if UNITY_EDITOR
                Debug.Log("Called AdjoePhoneVerification.OnResume(activity=" + activity + ")");
                return;
            #endif

            if (pv != null)
            {
                pv.Call("onResume", activity);
            }
        }

        /// <summary>
        /// Call this method from your Android <c>Activity.onDestroy</c> method.
        /// </summary>
        /// <param name="activity">The <c>Activity</c> from which this method is called.</param>
        public void OnDestroy(AndroidJavaObject activity)
        {
            #if UNITY_EDITOR
                Debug.Log("Called AdjoePhoneVerification.OnDestroy(activity=" + activity + ")");
                return;
            #endif
            
            if (pv != null)
            {
                pv.Call("onDestroy", activity);
            }
        }

        internal static AndroidJavaObject GetCurrentContext()
        {
            return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        }

        /// <summary>
        /// A callback used for the automatic phone verification process.
        /// </summary>
        public interface Callback
        {
            /// <summary>
            /// Invoked when an error occurs.
            /// </summary>
            /// <param name="exception">The exception.</param>
            void OnError(Exception exception);

            /// <summary>
            /// Invoked when the timeout for the SMS receiver is reached (typically five minutes).
            /// </summary>
            void OnSmsTimeout();

            /// <summary>
            /// Invoked when the dialog to choose the account could not be displayed to the user.
            /// </summary>
            /// <param name="exception">The exception.</param>
            void OnRequestHintFailure(Exception exception);

            /// <summary>
            /// invoked when the dialog to choose the account is not available.
            /// </summary>
            void OnRequestHintNotAvailable();

            /// <summary>
            /// Invoked when the user chose the option to choose another account.
            /// </summary>
            void OnRequestHintOtherAccount();

            /// <summary>
            /// Invoked when the verification code could not be extracted automatically from the SMS.
            /// </summary>
            void OnCannotExtractCode();
        }

        private class CallbackWrapper : AndroidJavaProxy
        {
            private readonly Callback callback;

            public CallbackWrapper(Callback callback) : base("io.adjoe.sdk.AdjoePhoneVerification$Callback")
            {
                this.callback = callback;
            }

            public void onError(AndroidJavaObject exception)
            {
                if (callback != null)
                {
                    callback.OnError(new Exception(exception.Call<string>("getMessage")));
                }
            }

            public void onSmsTimeout()
            {
                if (callback != null)
                {
                    callback.OnSmsTimeout();
                }
            }

            public void onRequestHintFailure(AndroidJavaObject exception)
            {
                if (callback != null)
                {
                    callback.OnRequestHintFailure(new Exception(exception.Call<string>("getMessage")));
                }
            }

            public void onRequestHintNotAvailable()
            {
                if (callback != null)
                {
                    callback.OnRequestHintNotAvailable();
                }
            }

            public void onRequestHintOtherAccount()
            {
                if (callback != null)
                {
                    callback.OnRequestHintOtherAccount();
                }
            }

            public void onCannotExtractCode()
            {
                if (callback != null)
                {
                    callback.OnCannotExtractCode();
                }
            }
        }

        /// <summary>
        /// A callback for both the manual and the automatic phone verification process.
        /// </summary>
        public interface CheckCallback
        {
            /// <summary>
            /// Invoked when the verification succeeds.
            /// </summary>
            void OnSuccess();

            /// <summary>
            /// Invoked when the phone number is already verified.
            /// </summary>
            void OnAlreadyVerified();

            /// <summary>
            /// Invoked when the phone number is already taken by another device.
            /// </summary>
            void OnAlreadyTaken();

            /// <summary>
            /// Invoked when too many requests to verify the phone number have been made.
            /// </summary>
            void OnTooManyAttempts();

            /// <summary>
            /// Invoked when an error occurs.
            /// </summary>
            /// <param name="exception">The exception.</param>
            void OnError(Exception exception);

            /// <summary>
            /// Invoked when the country code of the phone number is invalid.
            /// </summary>
            void OnInvalidCountryCode();
        }

        private class CheckCallbackWrapper : AndroidJavaProxy
        {
            private readonly CheckCallback callback;

            public CheckCallbackWrapper(CheckCallback callback) : base("io.adjoe.sdk.AdjoePhoneVerification$CheckCallback")
            {
                this.callback = callback;
            }

            public void onSuccess()
            {
                if (callback != null)
                {
                    callback.OnSuccess();
                }
            }

            public void onAlreadyVerified()
            {
                if (callback != null)
                {
                    callback.OnAlreadyVerified();
                }
            }

            public void onAlreadyTaken()
            {
                if (callback != null)
                {
                    callback.OnAlreadyTaken();
                }
            }

            public void onTooManyAttempts()
            {
                if (callback != null)
                {
                    callback.OnTooManyAttempts();
                }
            }

            public void onError(AndroidJavaObject exception)
            {
                if (callback != null)
                {
                    callback.OnError(new Exception(exception.Call<string>("getMessage")));
                }
            }

            public void onInvalidCountryCode()
            {
                if (callback != null)
                {
                    callback.OnInvalidCountryCode();
                }
            }
        }

        /// <summary>
        /// A callback that is used on both the manual and the automatic phone verification process.
        /// </summary>
        public interface VerifyCallback
        {
            /// <summary>
            /// Invoked when the phone number has been verified.
            /// </summary>
            void OnVerified();

            /// <summary>
            /// Invoked when the one-time verification code is invalid.
            /// </summary>
            void OnInvalidCode();

            /// <summary>
            /// Invoked when too many attempts to verify the verification code have been made.
            /// </summary>
            void OnTooManyAttempts();

            /// <summary>
            /// Invoked when the maximal number of allowed devices per user has been reached.
            /// </summary>
            void OnMaxAllowedDevicesReached();

            /// <summary>
            /// Invoked when an error occurs while verifying the verification code.
            /// </summary>
            /// <param name="exception">The exception.</param>
            void OnError(Exception exception);
        }

        private class VerifyCallbackWrapper : AndroidJavaProxy
        {
            private VerifyCallback callback;

            public VerifyCallbackWrapper(VerifyCallback callback) : base("io.adjoe.sdk.AdjoePhoneVerification$VerifyCallback")
            {
                this.callback = callback;
            }

            public void onVerified()
            {
                if (callback != null)
                {
                    callback.OnVerified();
                }
            }

            public void onInvalidCode()
            {
                if (callback != null)
                {
                    callback.OnInvalidCode();
                }
            }

            public void onTooManyAttempts()
            {
                if (callback != null)
                {
                    callback.OnTooManyAttempts();
                }
            }

            public void onMaxAllowedDevicesReached()
            {
                if (callback != null)
                {
                    callback.OnMaxAllowedDevicesReached();
                }
            }

            public void onError(AndroidJavaObject exception)
            {
                if (callback != null)
                {
                    callback.OnError(new Exception(exception.Call<string>("getMessage")));
                }
            }
        }

        /// <summary>
        /// A callback used for the result of <c>AdjoePhoneVerification.GetStatus</c>.
        /// </summary>
        public interface StatusCallback
        {
            /// <summary>
            /// Invoked when the phone number is verified.
            /// </summary>
            void OnVerified();

            /// <summary>
            /// Invoked when the phone number is not verified.
            /// </summary>
            void OnNotVerified();

            /// <summary>
            /// Invoked when an error occurs while checking the status.
            /// </summary>
            /// <param name="exception">The exception.</param>
            void OnError(Exception exception);
        }

        private class StatusCallbackWrapper : AndroidJavaProxy
        {
            private StatusCallback callback;

            public StatusCallbackWrapper(StatusCallback callback) : base("io.adjoe.sdk.AdjoePhoneVerification$StatusCallback")
            {
                this.callback = callback;
            }

            public void onVerified()
            {
                if (callback != null)
                {
                    callback.OnVerified();
                }
            }

            public void onNotVerified()
            {
                if (callback != null)
                {
                    callback.OnNotVerified();
                }
            }

            public void onError(AndroidJavaObject exception)
            {
                if (callback != null)
                {
                    callback.OnError(new Exception(exception.Call<string>("getMessage")));
                }
            }
        }
    }
}
