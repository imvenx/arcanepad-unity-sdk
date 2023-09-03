using System;
using System.Collections.Generic;
using ArcanepadSDK.Models;

namespace ArcanepadSDK
{
    public class ArcanePad
    {
        public string DeviceId { get; }
        public string InternalId { get; }
        private List<string> InternalIdList { get; }
        public string IframeId { get; }
        private List<string> IframeIdList { get; }
        public bool IsConnected { get; private set; }
        private ArcaneEventEmitter Events = new ArcaneEventEmitter();
        private WebSocketService<string> Msg;

        public ArcanePad(string deviceId, string internalId, string iframeId, bool isConnected)
        {
            DeviceId = deviceId;
            InternalId = internalId;
            InternalIdList = new List<string> { internalId };
            IframeId = iframeId;
            IframeIdList = new List<string> { iframeId };
            IsConnected = isConnected;

            Msg = Arcane.Msg;
            Msg.On(AEventName.GetQuaternion, (GetQuaternionEvent e, string clientId) =>
            {
                ProxyEvent(AEventName.GetQuaternion, e, clientId);
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
            Events.Emit(fullEventName, e);
        }

        public void StartGetQuaternion()
        {
            Msg.Emit(new StartGetQuaternionEvent(), InternalIdList);
        }

        public void StopGetQuaternion()
        {
            Msg.Emit(new StopGetQuaternionEvent(), InternalIdList);
            Events.Off($"{AEventName.GetQuaternion}_{InternalId}");
        }

        public void OnGetQuaternion(Action<GetQuaternionEvent> callback)
        {
            Events.On($"{AEventName.GetQuaternion}_{InternalId}", callback);
        }

        public void CalibrateQuaternion()
        {
            Msg.Emit(new CalibrateQuaternion(), InternalIdList);
        }

        public void Vibrate(int millisecconds)
        {
            Msg.Emit(new VibrateEvent(millisecconds), InternalIdList);
        }

        public void OnConnect(Action<IframePadConnectEvent> callback)
        {
            Events.On($"{AEventName.IframePadConnect}_{IframeId}", callback);
        }

        public void OnDisconnect(Action<IframePadDisconnectEvent> callback)
        {
            Events.On($"{AEventName.IframePadDisconnect}_{IframeId}", callback);
        }

        public void Emit(ArcaneBaseEvent e)
        {
            Msg.Emit(e, IframeIdList);
        }

        public Action On<T>(string eventName, Action<T> callback) where T : ArcaneBaseEvent
        {
            string fullEventName = $"{eventName}_{IframeId}";
            Events.On(fullEventName, callback);

            Action<T, string> proxyCallback = (T e, string from) =>
            {
                if (from == IframeId)
                {
                    Events.Emit(fullEventName, e);
                }
            };

            var unsubscribe = Msg.On(eventName, proxyCallback);

            return () =>
            {
                Events.Off(fullEventName);
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
            Events.UnsubscribeAll();
        }
    }
}