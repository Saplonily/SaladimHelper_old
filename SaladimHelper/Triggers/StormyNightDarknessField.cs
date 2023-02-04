using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SaladimHelper.Triggers;

[CustomEntity("SaladimHelper/StromyNightDarknessField")]
public class StormyNightDarknessField : Trigger
{
    protected float lightingTimer = 3.0f;
    protected Random random;

    public StormyNightDarknessField(EntityData data, Vector2 offset) : base(data, offset)
    {
        random = new(data.Position.GetHashCode());
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.BounceOut, 0.5f);
        Add(tween);
        tween.Start();
        Level level = SceneAs<Level>();
        tween.OnUpdate = t =>
        {
            level.Lighting.Alpha = t.Eased;
        };
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        foreach (var com in this)
        {
            if (com is Tween t)
            {
                t.Stop();
            }
        }
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.BounceOut, 0.5f);
        Add(tween);
        tween.Start();
        Level level = SceneAs<Level>();
        tween.OnUpdate = t =>
        {
            level.Lighting.Alpha = 1 - t.Eased;
        };
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        lightingTimer -= Engine.DeltaTime;
        if (lightingTimer <= 0.0f)
        {
            bool doDouble = random.Next(0, 2) is 1;
            Tween tween = Tween.Create(Tween.TweenMode.YoyoOneshot, Ease.BounceOut, ((float)random.NextDouble()) + 0.5f);
            Add(tween);
            tween.Start();
            Level level = SceneAs<Level>();
            tween.OnUpdate = t =>
            {
                level.Lighting.Alpha = 1 - t.Eased;
                if (doDouble && t.Percent == 1.0f)
                {
                    tween.RemoveSelf();
                    Tween tween2 = Tween.Create(Tween.TweenMode.Oneshot, Ease.QuintIn, 0.4f);
                    Add(tween2);
                    tween2.Start();
                    float startAlpha = level.Lighting.Alpha;
                    tween2.OnUpdate = t =>
                    {
                        level.Lighting.Alpha = Calc.LerpClamp(level.Lighting.Alpha, 1.0f, t.Eased);
                    };
                }
            };
            lightingTimer = (float)random.NextDouble() * 5.0f + 3.0f;
        }
    }
}
