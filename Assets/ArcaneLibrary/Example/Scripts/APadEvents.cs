using ArcanepadSDK.Models;

namespace ArcanepadSDK.APadEvents
{

    public abstract class APadEvent : ArcaneBaseEvent
    {
        public APadEvent(string name) : base(name) { }
    }

    public class ALeftEvent : APadEvent
    {
        public ALeftEvent() : base(AEventName.Left) { }
    }

    public class ARightEvent : APadEvent
    {
        public ARightEvent() : base(AEventName.Right) { }
    }

    public class AUpEvent : APadEvent
    {
        public AUpEvent() : base(AEventName.Up) { }
    }

    public class ADownEvent : APadEvent
    {
        public ADownEvent() : base(AEventName.Down) { }
    }

    public class AEnterEvent : APadEvent
    {
        public AEnterEvent() : base(AEventName.Enter) { }
    }

    public class AInfoEvent : APadEvent
    {
        public AInfoEvent() : base(AEventName.Info) { }
    }

    public class AExtraEvent : APadEvent
    {
        public AExtraEvent() : base(AEventName.Extra) { }
    }

    public class ABackEvent : APadEvent
    {
        public ABackEvent() : base(AEventName.Back) { }
    }

}