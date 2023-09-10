using System;
using System.Collections.Generic;
using System.Diagnostics;
using ArcanepadSDK.Models;
using Debug = UnityEngine.Debug;

namespace ArcanepadSDK
{
    public class ArcanePad : IDisposable
    {
        public ArcaneUser User { get; }
        public string DeviceId { get; }
        public string InternalId { get; }
        private List<string> InternalIdList { get; }
        public string IframeId { get; }
        private List<string> IframeIdList { get; }
        public bool IsConnected { get; private set; }
        private ArcaneEventEmitter Events = new ArcaneEventEmitter();
        private WebSocketService<string> Msg;

        public ArcanePad(string deviceId, string internalId, string iframeId, bool isConnected, ArcaneUser user = null)
        {
            User = user;
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

            Msg.On(AEventName.GetRotationEuler, (GetRotationEulerEvent e, string clientId) =>
            {
                ProxyEvent(AEventName.GetRotationEuler, e, clientId);
            });

            Msg.On(AEventName.GetPointer, (GetPointerEvent e, string clientId) =>
            {
                ProxyEvent(AEventName.GetPointer, e, clientId);
            });

            Msg.On(AEventName.IframePadConnect, (IframePadConnectEvent e, string from) =>
            {
                isConnected = true;
                ProxyEvent(AEventName.IframePadConnect, e, e.iframeId);
            });

            Msg.On(AEventName.IframePadDisconnect, (IframePadConnectEvent e, string from) =>
            {
                isConnected = false;
                ProxyEvent(AEventName.IframePadDisconnect, e, e.iframeId);
            });

            Msg.On(AEventName.OpenArcaneMenu, (OpenArcaneMenuEvent e, string fromId) =>
            {
                ProxyEvent(AEventName.OpenArcaneMenu, e, fromId);
            });

            Msg.On(AEventName.CloseArcaneMenu, (CloseArcaneMenuEvent e, string fromId) =>
            {
                ProxyEvent(AEventName.CloseArcaneMenu, e, fromId);
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

        public void StopGetQuaternion(bool offAllListeners = false)
        {
            Msg.Emit(new StopGetQuaternionEvent(), InternalIdList);
            if (offAllListeners) Events.Off($"{AEventName.GetQuaternion}_{InternalId}");
        }

        public void OnGetQuaternion(Action<GetQuaternionEvent> callback)
        {
            Events.On($"{AEventName.GetQuaternion}_{InternalId}", callback);
        }

        public void CalibrateQuaternion()
        {
            Msg.Emit(new CalibrateQuaternion(), InternalIdList);
        }
        public void StartGetRotationEuler()
        {
            Msg.Emit(new StartGetRotationEulerEvent(), InternalIdList);
        }

        public void StopGetRotationEuler(bool offAllListeners = false)
        {
            Msg.Emit(new StopGetRotationEulerEvent(), InternalIdList);
            if (offAllListeners) Events.Off($"{AEventName.GetRotationEuler}_{InternalId}");
        }

        public void OnGetRotationEuler(Action<GetRotationEulerEvent> callback)
        {
            Events.On($"{AEventName.GetRotationEuler}_{InternalId}", callback);
        }

        public void StartGetPointer()
        {
            Msg.Emit(new StartGetPointerEvent(), InternalIdList);
        }

        public void StopGetPointer(bool offAllListeners = false)
        {
            Msg.Emit(new StopGetPointerEvent(), InternalIdList);
            if (offAllListeners) Events.Off($"{AEventName.GetPointer}_{InternalId}");
        }

        public void OnGetPointer(Action<GetPointerEvent> callback)
        {
            Events.On($"{AEventName.GetPointer}_{InternalId}", callback);
        }

        public void CalibratePointer()
        {
            Msg.Emit(new CalibratePointer(), InternalIdList);
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

        public void OnOpenArcaneMenu(Action<OpenArcaneMenuEvent> callback)
        {
            Events.On($"{AEventName.OpenArcaneMenu}_{IframeId}", callback);
        }

        public void OnCloseArcaneMenu(Action<CloseArcaneMenuEvent> callback)
        {
            Events.On($"{AEventName.CloseArcaneMenu}_{IframeId}", callback);
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

        public void Off<T>(string eventName, Action<T> callback = null) where T : ArcaneBaseEvent
        {
            string fullEventName = $"{eventName}_{IframeId}";
            Events.Off(fullEventName, callback);
        }

        public void Dispose()
        {
            Events.OffAll();
        }
    }
}