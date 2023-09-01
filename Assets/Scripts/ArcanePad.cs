using System;
using System.Collections.Generic;
using ArcanepadSDK.Models;
using Newtonsoft.Json;

namespace ArcanepadSDK
{
    public class ArcanePad
    {
        public string IframeId { get; set; }
        public string DeviceId { get; set; }
        public string InternalId { get; set; }
        public bool IsConnected { get; set; }
        private EventEmitter events = new EventEmitter();
        private WebSocketService<string> Msg;

        public ArcanePad(string deviceId, string internalId, string iframeId, bool isConnected)
        {
            {
                IframeId = iframeId;
                DeviceId = deviceId;
                InternalId = internalId;
                IsConnected = isConnected;
            }
            Msg = Arcane.Msg;
            Msg.On(AEventName.GetRotationVector, (GetRotationVectorEvent e, string clientId) =>
            {
                ProxyEvent(AEventName.GetRotationVector, e, clientId);
            });

            Msg.On(AEventName.IframePadConnect, (IframePadConnectEvent e, string from) =>
            {
                ProxyEvent(AEventName.IframePadConnect, e, e.IframeId);
            });

            Msg.On(AEventName.IframePadDisconnect, (IframePadConnectEvent e, string from) =>
            {
                ProxyEvent(AEventName.IframePadDisconnect, e, e.IframeId);
            });
        }

        private void ProxyEvent(string eventName, ArcaneBaseEvent e, string padId)
        {
            string fullEventName = $"{eventName}_{padId}";
            events.Emit(fullEventName, e);
        }

        public void StartGetRotationVector()
        {
            Msg.Emit(new StartGetRotationVectorEvent(), new List<string> { InternalId });
        }

        public void StopGetRotationVector()
        {
            Msg.Emit(new StopGetRotationVectorEvent(), new List<string> { InternalId });
            events.Off($"{AEventName.GetRotationVector}_{InternalId}");
        }

        public void OnGetRotationVector(Action<GetRotationVectorEvent> callback)
        {
            events.On($"{AEventName.GetRotationVector}_{InternalId}", callback);
        }

        public void OnConnect(string padId, Action<IframePadConnectEvent> callback)
        {
            events.On($"{AEventName.IframePadConnect}_{padId}", callback);
        }

        public void OnDisconnect(string padId, Action<IframePadDisconnectEvent> callback)
        {
            events.On($"{AEventName.IframePadDisconnect}_{padId}", callback);
        }

        public Action On<T>(string eventName, Action<T> callback) where T : ArcaneBaseEvent
        {
            string fullEventName = $"{eventName}_{IframeId}";
            events.On(fullEventName, callback);

            Action<T, string> proxyCallback = (T e, string from) =>
            {
                if (from == IframeId)
                {
                    events.Emit(fullEventName, e);
                }
            };

            var unsubscribe = Msg.On(eventName, proxyCallback);

            return () =>
            {
                events.Off(fullEventName);
                unsubscribe();
            };
        }

        // public void Off<T>(string eventName, Action<T> callback = null) where T : ArcaneBaseEvent
        // {
        //     string fullEventName = $"{eventName}_{IframeId}";
        //     events.Off(fullEventName, callback);
        // }
    }
}
