using Celeste.Mod.Entities;
using Celeste.Mod.SaladimHelper;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LiteKeyLimiter.Triggers;

[CustomEntity("SaladimHelper/LiteKeyLimiterTrigger")]
public class LiteKeyLimiterTrigger : Trigger
{
    public bool LimitMoveX = false;
    public bool LimitMoveY = false;

    public LiteKeyLimiterTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        LimitMoveX = data.Bool("limitMoveX", false);
        LimitMoveY = data.Bool("limitMoveY", false);
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        Module.Session.LimitedMoveX = LimitMoveX;
        Module.Session.LimitedMoveY = LimitMoveY;
    }
}
