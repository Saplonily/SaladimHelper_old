using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SaladimHelper.Triggers;

[CustomEntity("SaladimHelper/FgOpacityTrigger")]
public class FgOpacityTrigger : Trigger
{
    public float TargetOpacity { get; set; }

    public FgOpacityTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        TargetOpacity = data.Float("opacity", 0.5f);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Level level = SceneAs<Level>();
        float fromA = level.SolidTiles.Tiles.Alpha;
        float fromB = level.SolidTiles.AnimatedTiles.Alpha;
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, 1.0f);
        tween.OnUpdate = t =>
        {
            level.SolidTiles.Tiles.Alpha = Calc.LerpClamp(fromA, TargetOpacity, t.Eased);
            level.SolidTiles.AnimatedTiles.Alpha = Calc.LerpClamp(fromB, TargetOpacity, t.Eased);
        };
        Add(tween);
        tween.Start();
    }
}
