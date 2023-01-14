using System;
using System.Linq;
using System.Text;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/reelCamera", "SaladimHelper/ReelCamera")]
public class ReelCamera : Entity
{
    public Vector2[] Nodes;
    public float[] NodesDistance;
    public float NodesDistanceSum;
    public bool SquashHorizontalArea = true;

    public float[] MoveSpeeds;
    public float[] DelaySequence;

    public float Progress = 0.0f;
    public float LengthProgress = 0.0f;

    protected Tween readyTween;
    protected bool ready = false;
    protected bool delaying = false;
    protected float delayTimeLeft = 0.0f;

    public bool LeadingTheReel
    {
        get => Module.Session.CurrentReelCamera == this;
        set => Module.Session.CurrentReelCamera = (value ? this : null);
    }

    public ReelCamera(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Collider = new Hitbox(data.Width, data.Height);

        Nodes = data.NodesOffset(offset);
        NodesDistance = new float[Nodes.Length - 1];
        for(int i = 0; i < Nodes.Length - 1; i++)
        {
            var node1 = Nodes[i];
            var node2 = Nodes[i + 1];
            NodesDistance[i] = Vector2.Distance(node2, node1);
        }
        NodesDistanceSum = NodesDistance.Sum();

        try
        {
            var moveTimeSequenceStr = data.Attr("move_time_sequence");
            var moveTimeSequenceStrs = moveTimeSequenceStr.Split(',');
            if(moveTimeSequenceStrs.Length != Nodes.Length - 1)
                throw new Exception($"MoveEime sequence load failed. Except {Nodes.Length - 1}(Nodes.Length) " +
                    $"but got {moveTimeSequenceStrs.Length}");
            var moveTimes = (
                from moveTimeStr in moveTimeSequenceStrs
                let moveTime = float.Parse(moveTimeStr)
                select moveTime
                ).ToArray();
            MoveSpeeds = new float[moveTimes.Length];
            for(int i = 0; i < moveTimes.Length; i++)
            {
                MoveSpeeds[i] = NodesDistance[i] / (moveTimes[i] * 60.0f);
            }

            var delaySequenceStr = data.Attr("delay_sequence");
            var delaySequenceStrs = moveTimeSequenceStr.Split(',');
            if(delaySequenceStrs.Length != Nodes.Length - 1)
                throw new Exception($"Delay sequence load failed. Except {Nodes.Length - 1}(Nodes.Length) " +
                    $"but got {delaySequenceStrs.Length}");
            DelaySequence = (
                from delayTimeStr in delaySequenceStrs
                let delay = float.Parse(delayTimeStr)
                select delay
                ).ToArray();

            StringBuilder sb = new();
            sb.Append(string.Join("|", MoveSpeeds));
            sb.Append(@"\\\\");
            sb.Append(string.Join("|", DelaySequence));
            Logger.Log(LogLevel.Info, "SaladimHelper", $"Loaded ReelCamera: {sb}");
        }
        catch(Exception e)
        {
            throw new Exception($"Loading sequence failed. Maybe failed parsing numbers. Inner msg:{e.Message}");
        }

        SquashHorizontalArea = data.Bool("squash_horizontal_area", true);
    }

    public override void Update()
    {
        base.Update();
        Level lvl = Engine.Scene as Level;
        Player player = lvl.Tracker.GetEntity<Player>();
        Player colliderPlayer = CollideFirst<Player>();
        if(colliderPlayer is not null)
        {
            LeadingTheReel = true;
        }

        if(LeadingTheReel && player is not null)
        {
            if(readyTween is null)
            {
                readyTween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, 1.0f);
                Vector2 start = lvl.Camera.Position;
                Vector2 end = Nodes[0] -
                    new Vector2(lvl.Camera.Right - lvl.Camera.Left, lvl.Camera.Bottom - lvl.Camera.Top) / 2;
                readyTween.OnUpdate = t =>
                {
                    lvl.Camera.Position = Calc.LerpSnap(start, end, t.Percent);
                };
                readyTween.OnComplete = t =>
                {
                    ready = true;
                };
                readyTween.Start();
                Add(readyTween);
            }
            if(ready)
            {
                do
                {
                    int currentNodeIndex = (int)Progress;
                    if(currentNodeIndex == Nodes.Length - 1)
                    {
                        break;
                    }

                    lvl.Camera.Position = Calc.LerpSnap(
                        Nodes[currentNodeIndex],
                        Nodes[currentNodeIndex + 1],
                        Progress - currentNodeIndex
                        ) - new Vector2(lvl.Camera.Right - lvl.Camera.Left, lvl.Camera.Bottom - lvl.Camera.Top) / 2;

                    bool delayLiftedJustNow = false;
                    if(delaying)
                    {
                        delayTimeLeft -= Engine.DeltaTime;
                        if(delayTimeLeft <= 0.0f)
                        {
                            delaying = false;
                            LengthProgress += MoveSpeeds[currentNodeIndex];
                            delayLiftedJustNow = true;
                        }
                        else
                        {
                            if(player.X < lvl.Camera.Left - player.Width)
                            {
                                var afterX = lvl.Camera.Left - player.Width;
                                player.X = afterX;
                            }
                            else if(player.X > lvl.Camera.Right + player.Width)
                            {
                                var afterX = lvl.Camera.Right + player.Width;
                                player.X = afterX;
                            }
                            if(player.Y < lvl.Camera.Top - player.Height)
                            {
                                player.Y = lvl.Camera.Top - player.Height;
                            }
                            else if(player.Y > lvl.Camera.Bottom + player.Height)
                                player.Die(Vector2.UnitY);
                            break;
                        }
                    }

                    #region 一堆碰撞和死亡判断
                    Vector2 dir = (Nodes[currentNodeIndex + 1] - Nodes[currentNodeIndex]).SafeNormalize();
                    bool xPositive = dir.X > 0;
                    bool yPositive = dir.Y > 0;
                    if(player.X < lvl.Camera.Left - player.Width)
                    {
                        var afterX = lvl.Camera.Left - player.Width;
                        player.X = afterX;
                        if(player.CollideFirst<Solid>() is not null)
                            player.Die(Vector2.Zero);

                        if(!SquashHorizontalArea && xPositive)
                            player.Die(Vector2.UnitX);
                    }
                    else if(player.X > lvl.Camera.Right + player.Width)
                    {
                        var afterX = lvl.Camera.Right + player.Width;
                        player.X = afterX;
                        if(player.CollideFirst<Solid>() is not null)
                            player.Die(Vector2.Zero);

                        if(!SquashHorizontalArea && !xPositive)
                            player.Die(-Vector2.UnitX);
                    }
                    if(player.Y < lvl.Camera.Top - player.Height)
                    {
                        if(yPositive) player.Die(-Vector2.UnitY);
                        player.Y = lvl.Camera.Top - player.Height;
                    }
                    else if(player.Y > lvl.Camera.Bottom + player.Height)
                        player.Die(Vector2.UnitY);
                    #endregion

                    float allLengthNeedToPast = new ArraySegment<float>(NodesDistance, 0, currentNodeIndex + 1).Sum();
                    LengthProgress += MoveSpeeds[currentNodeIndex];

                    var curSubProgress = (NodesDistance[currentNodeIndex] - (allLengthNeedToPast - LengthProgress))
                        / NodesDistance[currentNodeIndex];
                    var preProgress = Progress;
                    Progress = currentNodeIndex + curSubProgress;

                    if(!delaying && !delayLiftedJustNow)
                    {
                        if((int)Progress != currentNodeIndex)
                        {
                            delaying = true;
                            delayTimeLeft = DelaySequence[currentNodeIndex];

                            LengthProgress -= MoveSpeeds[currentNodeIndex];
                            Progress = preProgress;
                        }
                    }


                }
                while(1 + 1 == 3);

            }
        }
    }
}