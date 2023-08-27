
using System.Collections.Generic;
using ArcanepadSDK.Models;
using UnityEngine;

namespace ArcanepadSDK.Models
{
    public interface IWebSocketService { }

    public class ArcaneBaseEvent
    {
        public string name;

        public ArcaneBaseEvent(string name)
        {
            this.name = name;
        }
    }

    public class ArcaneMessageTo
    {
        public ArcaneBaseEvent e;
        public object to;
        public ArcaneMessageTo(ArcaneBaseEvent e, string[] to)
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
        public InitializeEvent(string assignedClientId, string assignedDeviceId) : base(AEventName.Initialize)
        {
            this.assignedClientId = assignedClientId;
            this.assignedDeviceId = assignedDeviceId;
        }
    }

    public class ClientConnect : ArcaneBaseEvent
    {
        public string clientId;
        public string clientType;
        public ClientConnect(string clientId, string clientType) : base(AEventName.ClientConnect)
        {
            this.clientId = clientId;
            this.clientType = clientType;
        }
    }

    public class ClientDisconnect : ArcaneBaseEvent
    {
        public string clientId;
        public ArcaneClientType clientType;
        public ClientDisconnect(string clientId, ArcaneClientType clientType) : base(AEventName.ClientDisconnect)
        {
            this.clientId = clientId;
            this.clientType = clientType;
        }
    }

    //#endregion 

    public class ArcaneClient
    {
        public string id { get; set; }
        public string clientType { get; set; }

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