
using System.Collections.Generic;
using ArcanepadSDK;
using ArcanepadSDK.Models;
using UnityEngine;

namespace ArcanepadSDK.Models
{
    public interface IWebSocketService { }

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
        public ArcaneClientType clientType;
        public ClientDisconnectEvent(string clientId, ArcaneClientType clientType) : base(AEventName.ClientDisconnect)
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

        public IframePadConnectEvent(string clientId, string internalId, string deviceId) : base(AEventName.IframePadConnect)
        {
            iframeId = clientId;
            this.internalId = internalId;
            this.deviceId = deviceId;
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

    public class CalibrateQuaternion : ArcaneBaseEvent { public CalibrateQuaternion() : base(AEventName.CalibrateQuaternion) { } }

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

    public class CalibratePointer : ArcaneBaseEvent { public CalibratePointer() : base(AEventName.CalibratePointer) { } }

    public class VibrateEvent : ArcaneBaseEvent
    {
        public int milliseconds;
        public VibrateEvent(int milliseconds) : base(AEventName.Vibrate)
        {
            this.milliseconds = milliseconds;
        }
    }

    //#endregion 

    public class ArcaneClient
    {
        public string id;
        public string clientType;

        public ArcaneClient(string id, string clientType)
        {
            this.id = id;
            this.clientType = clientType;
        }
    }

    public class ArcaneClientType
    {
        public static string @internal = "internal";
        public static string iframe = "iframe";
        public static string external = "external";
    }

    public class ArcaneDevice
    {
        public string id { get; set; }
        public IList<ArcaneClient> clients { get; set; }
        public string deviceType { get; set; }
        public ArcaneUser user { get; set; } = null;
    }

    public class ArcaneDeviceType
    {
        public static readonly string pad = "pad";
        public static readonly string view = "view";
        public static readonly string none = "none";
    }

    public enum ArcaneDeviceTypeEnum
    {
        view, pad
    }

    public class ArcaneClientInitData
    {
        public string clientType;
        public string deviceType;
        public string deviceId;
    }

    public class AssignedDataInitEvent
    {
        public string eventTag = "init";
        public string assignedClientId;
        public string assignedDeviceId;
    }

    public interface InitIframeQueryParams
    {
        string deviceId { get; set; }
    }

    public class AEventName
    {
        public static string None = "None";

        public static string Left = "Left";
        public static string Right = "Right";
        public static string Up = "Up";
        public static string Down = "Down";
        public static string Enter = "Enter";
        public static string Info = "Info";
        public static string Extra = "Extra";
        public static string Back = "Back";

        public static string SetDeviceType = "SetDeviceType";

        public static string EnterDevMode = "EnterDevMode";

        public static string EnterApp = "EnterApp";
        public static string ExitApp = "ExitApp";

        public static string RefreshGlobalState = "RefreshGlobalState";

        public static string Initialize = "Initialize";

        public static string ClientConnect = "ClientConnect";
        public static string ClientDisconnect = "ClientDisconnect";

        public static string IframePadConnect = "IframePadConnect";
        public static string IframePadDisconnect = "IframePadDisconnect";

        public static string StartGetQuaternion = "StartGetQuaternion";
        public static string StopGetQuaternion = "StopGetQuaternion";
        public static string GetQuaternion = "GetQuaternion";
        public static string CalibrateQuaternion = "CalibrateQuaternion";

        public static string StartGetRotationEuler = "StartGetRotationEuler";
        public static string StopGetRotationEuler = "StopGetRotationEuler";
        public static string GetRotationEuler = "GetRotationEuler";
        public static string CalibrateRotationEuler = "CalibrateRotationEuler";

        public static string StartGetPointer = "StartGetPointer";
        public static string StopGetPointer = "StopGetPointer";
        public static string GetPointer = "GetPointer";
        public static string CalibratePointer = "CalibratePointer";

        public static string Vibrate = "Vibrate";

        public static string UpdateUser = "UpdateUser";

        public static string OpenArcaneMenu = "OpenArcaneMenu";
        public static string CloseArcaneMenu = "CloseArcaneMenu";
    }
}

public class RefreshGlobalStateEvent : ArcaneBaseEvent
{
    public GlobalState refreshedGlobalState { get; set; }
    public RefreshGlobalStateEvent(GlobalState refreshedGlobalState) : base(AEventName.RefreshGlobalState)
    {
        this.refreshedGlobalState = refreshedGlobalState;
    }
}

public class GlobalState
{
    public IList<ArcaneDevice> devices { get; set; }
    public GlobalState(IList<ArcaneDevice> devices)
    {
        this.devices = devices;
    }
}

public class InitialState
{
    public List<ArcanePad> pads { get; }
    public InitialState(List<ArcanePad> pads)
    {
        this.pads = pads;
    }
}

public class ArcaneUser
{
    public string id { get; }
    public string name { get; }
    public string color { get; }
    public ArcaneUser(string id, string name, string color)
    {
        this.id = id;
        this.name = name;
        this.color = color;
    }
}
