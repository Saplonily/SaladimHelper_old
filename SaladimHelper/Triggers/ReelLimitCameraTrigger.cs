using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SaladimHelper.Triggers;

[CustomEntity("SaladimHelper/ReelLimitCameraTrigger")]
public class ReelLimitCameraTrigger : Trigger
{
    public ReelLimitActionInfo Info = null;
    public bool tweenEnd = false;
    public Vector2 CurrentProgress = new();
    public Tween readyTween = null;

    public ReelLimitCameraTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Info = new()
        {
            Start = new(data.Float("startX"), data.Float("startY")),
            End = new(data.Float("endX"), data.Float("endY")),
            Step = data.Float("step"),
            SquashVertival = data.Bool("squash_vertival_area"),
            SquashHorizontal = data.Bool("squash_horizontal_area")
        };
    }

    public override void OnEnter(Player player)
    {
        Module.Session.CurrentReelLimit = this.Info;
        var tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, 1, true);
        readyTween = tween;
        var lvl = player.SceneAs<Level>();
        var cameraStartPosition = lvl.Camera.Position - lvl.Session.LevelData.Position;
        tween.OnUpdate = t =>
        {
            lvl.Camera.Position = Calc.LerpSnap(cameraStartPosition, Info.Start, t.Percent) + lvl.Session.LevelData.Position;
        };
        tween.OnComplete = t =>
        {
            tweenEnd = true;
        };
        this.Add(tween);
        CurrentProgress = Info.Start;
    }

    public override void OnLeave(Player player)
    {
        Module.Session.CurrentReelLimit = null;
        tweenEnd = false;
        readyTween.RemoveSelf();
    }

    public override void Update()
    {
        base.Update();
        Level lvl = Engine.Scene as Level;
        var player = lvl.Tracker.GetEntity<Player>();
        if (tweenEnd)
        {
            CurrentProgress = Calc.Approach(CurrentProgress, Info.End, Info.StepVec.Length());
            if (Module.Session.CurrentReelLimit == this.Info)
            {
                lvl.Camera.Position = lvl.Session.LevelData.Position + CurrentProgress;
            }
        }
        if (player is not null)
        {
            if (Info.SquashHorizontal)
            {
                player.Position.X = Math.Min(player.Position.X, lvl.Camera.Right + 5);
                player.Position.X = Math.Max(player.Position.X, lvl.Camera.Left - 5);
            }
            else
            {
                if (player.Position.X > lvl.Camera.Right + 8)
                    player.Die(-Vector2.UnitX);
                if (player.Position.X < lvl.Camera.Left - 8)
                    player.Die(Vector2.UnitX);
            }

            if (Info.SquashVertival)
            {
                player.Position.Y = Math.Min(player.Position.Y, lvl.Camera.Bottom + 5);
                player.Position.Y = Math.Max(player.Position.Y, lvl.Camera.Top - 5);
            }
            else
            {
                if (player.Position.Y > lvl.Camera.Bottom + 8)
                    player.Die(-Vector2.UnitY);
                if (player.Position.Y < lvl.Camera.Top - 8)
                    player.Die(Vector2.UnitY);
            }

            if (player.CollideCheck<Solid>()) player.Die(Vector2.Zero);
        }

    }
}
