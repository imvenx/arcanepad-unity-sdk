using ArcanepadSDK.Models;
namespace ArcanepadExample
{
    public abstract class PadEvent : ArcaneBaseEvent
    {
        public PadEvent(string name) : base(name) { }
    }

    public class LeftEvent : PadEvent
    {
        public LeftEvent() : base(AEventName.Left) { }
    }

    public class RightEvent : PadEvent
    {
        public RightEvent() : base(AEventName.Right) { }
    }

    public class UpEvent : PadEvent
    {
        public UpEvent() : base(AEventName.Up) { }
    }

    public class DownEvent : PadEvent
    {
        public DownEvent() : base(AEventName.Down) { }
    }

    public class EnterEvent : PadEvent
    {
        public EnterEvent() : base(AEventName.Enter) { }
    }

    public class InfoEvent : PadEvent
    {
        public InfoEvent() : base(AEventName.Info) { }
    }

    public class ExtraEvent : PadEvent
    {
        public ExtraEvent() : base(AEventName.Extra) { }
    }

    public class BackEvent : PadEvent
    {
        public BackEvent() : base(AEventName.Back) { }
    }
}