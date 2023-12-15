
using System.Collections.Generic;
using ArcanepadSDK.Types;

namespace ArcanepadSDK.Models
{
    public class ArcaneBaseEvent
    {
        public string name { get; }

        public ArcaneBaseEvent(string name)
        {
            this.name = name;
        }
    }

    public class ArcaneMessageTo
    {
        public ArcaneBaseEvent e;
        public object to;
        public ArcaneMessageTo(ArcaneBaseEvent e, IList<string> to)
        {
            this.e = e;
            this.to = to;
        }

        public ArcaneMessageTo(ArcaneBaseEvent e, string to)
        {
            this.e = e;
            this.to = to;
        }
    }

    public class ArcaneMessageFrom
    {
        public Dictionary<string, object> e;
        public string from;
        public ArcaneMessageFrom(Dictionary<string, object> e, string from)
        {
            this.e = e;
            this.from = from;
        }
    }

    //#region  Events

    public class InitializeEvent : ArcaneBaseEvent
    {
        public string assignedClientId;
        public string assignedDeviceId;
        public GlobalState globalState;
        public InitializeEvent(string assignedClientId, string assignedDeviceId, GlobalState globalState) : base(AEventName.Initialize)
        {
            this.assignedClientId = assignedClientId;
            this.assignedDeviceId = assignedDeviceId;
            this.globalState = globalState;
        }
    }

    public class ClientConnectEvent : ArcaneBaseEvent
    {
        public string clientId;
        public string clientType;
        public ClientConnectEvent(string clientId, string clientType) : base(AEventName.ClientConnect)
        {
            this.clientId = clientId;
            this.clientType = clientType;
        }
    }

    public class UpdateUserEvent : ArcaneBaseEvent
    {
        public ArcaneUser user;
        public UpdateUserEvent(ArcaneUser user) : base(AEventName.UpdateUser)
        {
            this.user = user;
        }
    }

    public class OpenArcaneMenuEvent : ArcaneBaseEvent { public OpenArcaneMenuEvent() : base(AEventName.OpenArcaneMenu) { } }
    public class CloseArcaneMenuEvent : ArcaneBaseEvent { public CloseArcaneMenuEvent() : base(AEventName.CloseArcaneMenu) { } }

    public class ClientDisconnectEvent : ArcaneBaseEvent
    {
        public string clientId;
        public string clientType;
        public ClientDisconnectEvent(string clientId, string clientType) : base(AEventName.ClientDisconnect)
        {
            this.clientId = clientId;
            this.clientType = clientType;
        }
    }

    public class IframePadConnectEvent : ArcaneBaseEvent
    {
        public string deviceId { get; set; }
        public string internalId { get; set; }
        public string iframeId { get; set; }
        public ArcaneUser user { get; set; }

        public IframePadConnectEvent(string clientId, string internalId, string deviceId, ArcaneUser user) : base(AEventName.IframePadConnect)
        {
            iframeId = clientId;
            this.internalId = internalId;
            this.deviceId = deviceId;
            this.user = user;
        }
    }

    public class IframePadDisconnectEvent : ArcaneBaseEvent
    {
        public string IframeId { get; set; }
        public string DeviceId { get; set; }

        public IframePadDisconnectEvent(string iframeId, string deviceId) : base(AEventName.IframePadDisconnect)
        {
            IframeId = iframeId;
            DeviceId = deviceId;
        }
    }

    public class StartGetQuaternionEvent : ArcaneBaseEvent { public StartGetQuaternionEvent() : base(AEventName.StartGetQuaternion) { } }
    public class StopGetQuaternionEvent : ArcaneBaseEvent { public StopGetQuaternionEvent() : base(AEventName.StopGetQuaternion) { } }

    public class GetQuaternionEvent : ArcaneBaseEvent
    {
        public float w;
        public float x;
        public float y;
        public float z;
        public GetQuaternionEvent(float w, float x, float y, float z) : base(AEventName.GetQuaternion)
        {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class CalibrateQuaternionEvent : ArcaneBaseEvent { public CalibrateQuaternionEvent() : base(AEventName.CalibrateQuaternion) { } }

    public class StartGetRotationEulerEvent : ArcaneBaseEvent { public StartGetRotationEulerEvent() : base(AEventName.StartGetRotationEuler) { } }
    public class StopGetRotationEulerEvent : ArcaneBaseEvent { public StopGetRotationEulerEvent() : base(AEventName.StopGetRotationEuler) { } }

    public class GetRotationEulerEvent : ArcaneBaseEvent
    {
        public float x;
        public float y;
        public float z;
        public GetRotationEulerEvent(float x, float y, float z) : base(AEventName.GetRotationEuler)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class StartGetPointerEvent : ArcaneBaseEvent { public StartGetPointerEvent() : base(AEventName.StartGetPointer) { } }
    public class StopGetPointerEvent : ArcaneBaseEvent { public StopGetPointerEvent() : base(AEventName.StopGetPointer) { } }

    public class GetPointerEvent : ArcaneBaseEvent
    {
        public float x;
        public float y;
        public GetPointerEvent(float x, float y) : base(AEventName.GetPointer)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class CalibratePointerEvent : ArcaneBaseEvent
    {
        public bool isTopLeft;
        public CalibratePointerEvent(bool isTopLeft) : base(AEventName.CalibratePointer)
        {
            this.isTopLeft = isTopLeft;
        }
    }

    public class SetScreenOrientationPortraitEvent : ArcaneBaseEvent { public SetScreenOrientationPortraitEvent() : base(AEventName.SetScreenOrientationPortrait) { } }
    public class SetScreenOrientationLandscapeEvent : ArcaneBaseEvent { public SetScreenOrientationLandscapeEvent() : base(AEventName.SetScreenOrientationLandscape) { } }

    public class VibrateEvent : ArcaneBaseEvent
    {
        public int milliseconds;
        public VibrateEvent(int milliseconds) : base(AEventName.Vibrate)
        {
            this.milliseconds = milliseconds;
        }
    }

    //#endregion 

}
