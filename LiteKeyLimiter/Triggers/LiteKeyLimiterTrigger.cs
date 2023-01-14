using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LiteKeyLimiter.Triggers;

[CustomEntity("LiteKeyLimiter/LiteKeyLimiterHelper")]
public class LiteKeyLimiterTrigger : Trigger
{
    public bool LimitMoveX = false;
    public bool LimitMoveY = false;
    public bool[][] dirLimitBitMask;

    public LiteKeyLimiterTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        dirLimitBitMask = new bool[][]
        {
            new bool[]{ false, false, false},
            new bool[]{ false, false, false},
            new bool[]{ false, false, false}
        };
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
