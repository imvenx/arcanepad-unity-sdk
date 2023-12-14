
using ArcanepadSDK.Models;

namespace ArcanepadSDK.ACustomModels
{
    public class AttackEvent : ArcaneBaseEvent
    {
        public int damage;
        public AttackEvent(int damage) : base(ACustomEventNames.Attack)
        {
            this.damage = damage;
        }
    }

    public class ACustomEventNames
    {
        public static string Attack = "Attack";
    }
}