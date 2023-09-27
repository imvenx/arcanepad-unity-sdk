
using System.Collections.Generic;
using ArcanepadSDK;
using ArcanepadSDK.Models;
using UnityEngine;

namespace ArcanepadSDK.Models
{
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


public class ArcaneInitParams
{
    public string deviceType;
    public string port;
    public string reverseProxyPort;
    public string arcaneCode;
    public ArcaneInitParams(
        string deviceType = "view",
        string port = "3005",
        string reverseProxyPort = "3009",
        string arcaneCode = ""
    )
    {
        this.deviceType = deviceType;
        this.port = port;
        this.reverseProxyPort = reverseProxyPort;
        this.arcaneCode = arcaneCode;
    }
}