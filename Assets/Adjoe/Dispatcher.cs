using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace io.adjoe.sdk
{
    /// <summary>
    /// Dispatches actions to run in separate or the main thread.
    /// </summary>
    public class Dispatcher : MonoBehaviour
    {

        /// <summary>
        /// Dispatches an action to run in separate thread.
        /// </summary>
        /// <param name="action"> The action</param>
        public static void RunAsync(Action action)
        {
            ThreadPool.QueueUserWorkItem(o => action());
        }

        /// <summary>
        /// Dispatches an action to run in separate thread.
        /// </summary>
        /// <param name="action"> The action</param>
        /// <param name="state"> The action state</param>
        public static void RunAsync(Action<object> action, object state)
        {
            ThreadPool.QueueUserWorkItem(o => action(o), state);
        }

        /// <summary>
        /// Dispatches an action to run in main thread.
        /// </summary>
        /// <param name="action"> The action</param>
        public static void RunOnMainThread(Action action)
        {
            lock (_backlog)
            {
                _backlog.Add(action);
                _queued = true;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_instance == null)
            {
                _instance = new GameObject("Dispatcher").AddComponent<Dispatcher>();
                DontDestroyOnLoad(_instance.gameObject);
            }
        }

        private void Update()
        {
            if (_queued)
            {
                lock (_backlog)
                {
                    var tmp = _actions;
                    _actions = _backlog;
                    _backlog = tmp;
                    _queued = false;
                }

                foreach (var action in _actions)
                    action();

                _actions.Clear();
            }
        }

        static Dispatcher _instance;
        static volatile bool _queued = false;
        static List<Action> _backlog = new List<Action>(8);
        static List<Action> _actions = new List<Action>(8);
    }
}
