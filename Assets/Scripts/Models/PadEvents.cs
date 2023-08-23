using ArcanepadSDK.Models;

namespace ArcanepadSDK.PadEvents
{

    public abstract class PadEvent : ArcaneBaseEvent
    {
        public PadEvent(string name) : base(name) { }
    }

    public class LeftEvent : PadEvent
    {
        LeftEvent() : base(AEventName.Left) { }
    }

    public class RightEvent : PadEvent
    {
        RightEvent() : base(AEventName.Right) { }
    }

    public class UpEvent : PadEvent
    {
        UpEvent() : base(AEventName.Up) { }
    }

    public class DownEvent : PadEvent
    {
        DownEvent() : base(AEventName.Down) { }
    }

    public class EnterEvent : PadEvent
    {
        EnterEvent() : base(AEventName.Enter) { }
    }

    public class InfoEvent : PadEvent
    {
        InfoEvent() : base(AEventName.Info) { }
    }

    public class ExtraEvent : PadEvent
    {
        ExtraEvent() : base(AEventName.Extra) { }
    }

    public class BackEvent : PadEvent
    {
        BackEvent() : base(AEventName.Back) { }
    }

}