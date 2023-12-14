namespace ArcanepadSDK.Types
{

    public class ArcaneClientType
    {
        public static string @internal = "internal";
        public static string iframe = "iframe";
        public static string external = "external";
    }

    public class ArcaneDeviceType
    {
        public static readonly string pad = "pad";
        public static readonly string view = "view";
        public static readonly string none = "none";
    }


    public enum ArcaneDeviceTypeEnum { view, pad }

    public enum AOrientation { Landscape, Portrait }

}