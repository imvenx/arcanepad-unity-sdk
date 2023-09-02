using System;
using System.Collections.Generic;
using ArcanepadSDK.Models;

namespace ArcanepadSDK
{
    public class ArcanePad
    {
        public string deviceId { get; set; }
        public string internalId { get; set; }
        public string iframeId { get; set; }
        public bool isConnected { get; set; }
        private ArcaneEventEmitter events = new ArcaneEventEmitter();
        private WebSocketService<string> Msg;

        public ArcanePad(string deviceId, string internalId, string iframeId, bool isConnected)
        {
            this.iframeId = iframeId;
            this.deviceId = deviceId;
            this.internalId = internalId;
            this.isConnected = isConnected;

            Msg = Arcane.msg;
            Msg.On(AEventName.GetRotationVector, (GetRotationVectorEvent e, string clientId) =>
            {
                ProxyEvent(AEventName.GetRotationVector, e, clientId);
            });

            Msg.On(AEventName.IframePadConnect, (IframePadConnectEvent e, string from) =>
            {
                ProxyEvent(AEventName.IframePadConnect, e, e.iframeId);
            });

            Msg.On(AEventName.IframePadDisconnect, (IframePadConnectEvent e, string from) =>
            {
                ProxyEvent(AEventName.IframePadDisconnect, e, e.iframeId);
            });
        }

        private void ProxyEvent(string eventName, ArcaneBaseEvent e, string padId)
        {
            string fullEventName = $"{eventName}_{padId}";
            events.Emit(fullEventName, e);
        }

        public void StartGetRotationVector()
        {
            Msg.Emit(new StartGetRotationVectorEvent(), new List<string> { internalId });
        }

        public void StopGetRotationVector()
        {
            Msg.Emit(new StopGetRotationVectorEvent(), new List<string> { internalId });
            events.Off($"{AEventName.GetRotationVector}_{internalId}");
        }

        public void OnGetRotationVector(Action<GetRotationVectorEvent> callback)
        {
            events.On($"{AEventName.GetRotationVector}_{internalId}", callback);
        }

        public void OnConnect(Action<IframePadConnectEvent> callback)
        {
            events.On($"{AEventName.IframePadConnect}_{iframeId}", callback);
        }

        public void OnDisconnect(Action<IframePadDisconnectEvent> callback)
        {
            events.On($"{AEventName.IframePadDisconnect}_{iframeId}", callback);
        }

        public void Emit(ArcaneBaseEvent e)
        {
            Msg.Emit(e, new List<string> { iframeId });
        }

        public Action On<T>(string eventName, Action<T> callback) where T : ArcaneBaseEvent
        {
            string fullEventName = $"{eventName}_{iframeId}";
            events.On(fullEventName, callback);

            Action<T, string> proxyCallback = (T e, string from) =>
            {
                if (from == iframeId)
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

        public void Off()
        {
            events.UnsubscribeAll();
        }
    }
}