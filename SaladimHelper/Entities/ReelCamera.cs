using System;
using System.Collections;
using System.Linq;
using System.Text;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/reelCamera", "SaladimHelper/ReelCamera"), Tracked]
public class ReelCamera : Entity
{
    public Vector2[] Nodes;
    public Vector2 CameraPosition;

    public bool SquashHorizontalArea = true;

    public float[] MoveTimes;
    public float[] DelaySequence;
    public float StartDelay;
    public float StartMoveTime;

    public bool Delaying = false;

    public Player DoingPlayer;

    public bool LeadingTheReel
    {
        get => Module.Session.CurrentReelCamera == this;
        set => Module.Session.CurrentReelCamera = (value ? this : null);
    }

    public ReelCamera(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Collider = new Hitbox(data.Width, data.Height);

        Nodes = data.NodesOffset(offset);
        for (int i = 0; i < Nodes.Length; i++)
        {
            Nodes[i] = Nodes[i] with { X = Nodes[i].X + Width / 2, Y = Nodes[i].Y + Height / 2 };
        }

        try
        {
            var moveTimeSequenceStr = data.Attr("move_time_sequence");
            var moveTimeSequenceStrs = moveTimeSequenceStr.Split(',');
            if (moveTimeSequenceStrs.Length != Nodes.Length - 1)
                throw new Exception($"MoveEime sequence load failed. Except {Nodes.Length - 1}(Nodes.Length) " +
                    $"but got {moveTimeSequenceStrs.Length}");
            MoveTimes = (
                from moveTimeStr in moveTimeSequenceStrs
                let moveTime = float.Parse(moveTimeStr)
                select moveTime
                ).ToArray();

            var delaySequenceStr = data.Attr("delay_sequence");
            var delaySequenceStrs = delaySequenceStr.Split(',');
            if (delaySequenceStrs.Length != Nodes.Length - 1)
                throw new Exception($"Delay sequence load failed. Except {Nodes.Length - 1}(Nodes.Length) " +
                    $"but got {delaySequenceStrs.Length}");
            DelaySequence = (
                from delayTimeStr in delaySequenceStrs
                let delay = float.Parse(delayTimeStr)
                select delay
                ).ToArray();

            StringBuilder sb = new(15);
            sb.Append(string.Join(",", MoveTimes));
            sb.Append(@"  |  ");
            sb.Append(string.Join(",", DelaySequence));
            Logger.Log(LogLevel.Info, "SaladimHelper", $"Loaded ReelCamera: {sb}");
        }
        catch (Exception e)
        {
            throw new Exception($"Loading sequence failed. Maybe failed parsing numbers. Inner msg:{e.Message}");
        }

        SquashHorizontalArea = data.Bool("squash_horizontal_area", true);
        StartDelay = data.Float("start_delay", 1.0f);
        StartMoveTime = data.Float("start_move_time", 1.0f);
    }

    public override void Update()
    {
        base.Update();

        Level level = SceneAs<Level>();
        var c = level.Camera;
        if (LeadingTheReel)
        {
            if (Delaying && DoingPlayer != null)
            {
                if (DoPlayerDieCheck(level, DoingPlayer, SquashHorizontalArea, SquashHorizontalArea, false))
                {
                    LeadingTheReel = false;
                }
            }
        }
        else
        {
            Player player = CollideFirst<Player>();
            if (player is not null)
            {
                DoingPlayer = player;
                LeadingTheReel = true;

                Add(new Coroutine(GetMovingCoroutine(level, player)));
                CameraPosition = c.Position + new Vector2(c.Right - c.Left, c.Bottom - c.Top) / 2;
            }
        }
    }

    public IEnumerator GetMovingCoroutine(Level level, Player player)
    {
        Logger.Log(LogLevel.Info, "S", $"coroutine : {player}");
        DoingPlayer = player;
        Camera c = level.Camera;
        bool startTweenCompleted = false;
        Vector2 startFrom = CameraPosition;
        Vector2 startTo = Nodes[0];
        Tween startTween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, StartMoveTime);
        startTween.OnUpdate = t => CameraPosition = Calc.LerpSnap(startFrom, startTo, t.Eased);
        startTween.OnComplete = _ => startTweenCompleted = true;
        Add(startTween);
        startTween.Start();
        Vector2 ps = startTo - startFrom;
        while (!startTweenCompleted)
        {
            if (DoPlayerDieCheck(level, player, SquashHorizontalArea, SquashHorizontalArea, ps.Y > 0))
                yield break;
            yield return null;
        }
        Delaying = true;
        if (StartDelay != 0.0f)
            yield return StartDelay;
        Delaying = false;

        for (int currentFromNode = 0; currentFromNode < Nodes.Length - 1; currentFromNode++)
        {
            Vector2 from = Nodes[currentFromNode];
            Vector2 to = Nodes[currentFromNode + 1];
            bool motionTweenCompleted = false;
            Tween motionTween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, MoveTimes[currentFromNode]);
            motionTween.OnUpdate = t => CameraPosition = Calc.LerpSnap(from, to, t.Eased);
            motionTween.OnComplete = _ => motionTweenCompleted = true;
            Add(motionTween);
            motionTween.Start();
            while (!motionTweenCompleted)
            {
                Vector2 p = to - from;
                if (DoPlayerDieCheck(level, player, SquashHorizontalArea, SquashHorizontalArea, p.Y > 0))
                    yield break;
                yield return null;
            }
            Delaying = true;
            if (DelaySequence[currentFromNode] != 0.0f)
                yield return DelaySequence[currentFromNode];
            Delaying = false;
        }
        LeadingTheReel = false;
        DoingPlayer = null;
        yield break;
    }

    public bool DoPlayerDieCheck(Level level, Player player, bool leftSquash, bool rightSquash, bool isYPositive)
    {
        if (level.Tracker.GetEntity<Player>() == null) return false;
        if (player is null) return false;
        bool died = false;
        try
        {
            float xx = level.Camera.Left + player.Width / 2;
            if (player.X < xx)
            {
                player.MoveH(xx - player.X, OnCollide);

                if (!leftSquash)
                {
                    player.Die(Vector2.UnitX);
                    died = true;
                }
            }
            xx = level.Camera.Right - player.Width / 2;
            if (player.X > xx)
            {
                player.MoveH(xx - player.X, OnCollide);

                if (!rightSquash)
                {
                    player.Die(-Vector2.UnitX);
                    died = true;
                }
            }
            float yy = level.Camera.Top - player.Height / 2;
            if (player.Y < yy)
            {
                if (isYPositive)
                {
                    player.Die(-Vector2.UnitY);
                    died = true;
                }
                player.MoveV(yy - player.Y, OnCollide);

            }
            if (player.Y > level.Camera.Bottom + player.Height)
            {
                player.Die(Vector2.UnitY);
                died = true;
            }
        }
        catch (Exception e)
        {
            Logger.Log(LogLevel.Error, Module.Name, "at ReelCamera die check:" + "\n" + e.Message + "\n" + e.StackTrace);
        }
        return died;
        void OnCollide(CollisionData data)
        {
            if (data.Moved.LengthSquared() == 0.0f)
            {
                if ((data.TargetPosition - player.Position).Length() <= 5.0f)
                {
                    player.Die(data.Direction);
                    died = true;
                }
            }
        }
    }
}