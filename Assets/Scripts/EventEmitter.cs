using System;
using System.Collections.Generic;
using ArcanepadSDK.Models;

namespace ArcanepadSDK
{
    public class ArcaneEventEmitter
    {
        private Dictionary<string, Delegate> eventHandlers = new Dictionary<string, Delegate>();

        public Action On<T>(string name, Action<T> callback) where T : ArcaneBaseEvent
        {
            if (!eventHandlers.ContainsKey(name))
            {
                eventHandlers[name] = callback;
            }
            else
            {
                eventHandlers[name] = Delegate.Combine(eventHandlers[name], callback);
            }
            return () => Off(name, callback);
        }

        public void Off(string name, Delegate callback = null)
        {
            if (!eventHandlers.ContainsKey(name)) return;

            if (callback == null)
            {
                eventHandlers.Remove(name);
            }
            else
            {
                eventHandlers[name] = Delegate.Remove(eventHandlers[name], callback);
            }
        }

        public void Emit<T>(string name, T e) where T : ArcaneBaseEvent
        {
            if (eventHandlers.ContainsKey(name))
            {
                var delegateList = eventHandlers[name].GetInvocationList();
                foreach (Delegate singleDelegate in delegateList)
                {
                    var expectedType = singleDelegate.Method.GetParameters()[0].ParameterType;
                    if (e.GetType() == expectedType || e.GetType().IsSubclassOf(expectedType))
                    {
                        var method = singleDelegate.Method;
                        method.Invoke(singleDelegate.Target, new object[] { e });
                    }
                }
            }
        }

        public void OffAll()
        {
            eventHandlers.Clear();
        }
    }
}
