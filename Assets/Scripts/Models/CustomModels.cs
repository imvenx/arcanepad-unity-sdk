
using ArcanepadSDK.Models;

namespace ArcanepadSDK.CustomModels
{
    public class AttackEvent : ArcaneBaseEvent
    {
        public int damage;
        public AttackEvent(int damage) : base(CustomEventNames.Attack)
        {
            this.damage = damage;
        }
    }

    public class CustomEventNames
    {
        public static string Attack = "Attack";
    }
}