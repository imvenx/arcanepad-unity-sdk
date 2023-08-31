using System;
using System.Collections.Generic;
using ArcanepadSDK.Models;

namespace ArcanepadSDK
{
    public class ArcanePad
    {
        private EventEmitter events = new EventEmitter();
        private WebSocketService<string> msg;

        public ArcanePad(WebSocketService<string> _msg)
        {
            msg = _msg;
            msg.On(AEventName.GetRotationVector, (GetRotationVectorEvent e, string clientId) =>
            {
                ProxyEvent(AEventName.GetRotationVector, e, clientId);
            });

            msg.On(AEventName.IframePadConnect, (IframePadConnectEvent e, string from) =>
            {
                ProxyEvent(AEventName.IframePadConnect, e, e.IframeId);
            });

            msg.On(AEventName.IframePadDisconnect, (IframePadConnectEvent e, string from) =>
            {
                ProxyEvent(AEventName.IframePadDisconnect, e, e.IframeId);
            });
        }

        private void ProxyEvent(string eventName, ArcaneBaseEvent e, string padId)
        {
            string fullEventName = $"{eventName}_{padId}";
            events.Emit(fullEventName, e);
        }

        public void StartGetRotationVector(string internalId)
        {
            msg.Emit(new StartGetRotationVectorEvent(), new List<string> { internalId });
        }

        public void StopGetRotationVector(string internalId)
        {
            msg.Emit(new StopGetRotationVectorEvent(), new List<string> { internalId });
            events.Off($"{AEventName.GetRotationVector}_{internalId}");
        }

        public void OnGetRotationVector(string internalId, Action<GetRotationVectorEvent> callback)
        {
            events.On($"{AEventName.GetRotationVector}_{internalId}", callback);
        }

        public void OnConnect(string padId, Action<IframePadConnectEvent> callback)
        {
            events.On($"{AEventName.IframePadConnect}_{padId}", callback);
        }

        public void OnDisconnect(string padId, Action<IframePadDisconnectEvent> callback)
        {
            events.On($"{AEventName.IframePadDisconnect}_{padId}", callback);
        }

        public void On<T>(string eventName, string padId, Action<T> callback) where T : ArcaneBaseEvent
        {
            string fullEventName = $"{eventName}_{padId}";
            events.On(fullEventName, callback);

            Action<T, string> proxyCallback = (T e, string clientId) =>
            {
                if (clientId == padId)
                {
                    ProxyEvent(eventName, e, padId);
                }
            };

            msg.On(eventName, proxyCallback);
        }

        public void Off(string padId, string eventName, Action<ArcaneBaseEvent> callback = null)
        {
            string fullEventName = $"{eventName}_{padId}";
            events.Off(fullEventName, callback);
        }
    }
}
