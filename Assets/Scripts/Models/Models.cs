
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

    public class StartGetRotationVectorEvent : ArcaneBaseEvent
    {
        public StartGetRotationVectorEvent() : base(AEventName.StartGetRotationVector) { }
    }

    public class StopGetRotationVectorEvent : ArcaneBaseEvent
    {
        public StopGetRotationVectorEvent() : base(AEventName.StopGetRotationVector) { }
    }

    public class GetRotationVectorEvent : ArcaneBaseEvent
    {
        public float azimuth;
        public float pitch;
        public float roll;
        public GetRotationVectorEvent(float azimuth, float pitch, float roll) : base(AEventName.GetRotationVector)
        {
            this.azimuth = azimuth;
            this.pitch = pitch;
            this.roll = roll;
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
    }

    public class ArcaneDeviceType
    {
        public static readonly string pad = "pad";
        public static readonly string view = "view";
        public static readonly string none = "none";
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
        string deviceId { get; }
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

        public static string StartGetRotationVector = "StartGetRotationVector";
        public static string StopGetRotationVector = "StopGetRotationVector";
        public static string GetRotationVector = "GetRotationVector";
        public static string IframePadConnect = "IframePadConnect";
        public static string IframePadDisconnect = "IframePadDisconnect";
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
