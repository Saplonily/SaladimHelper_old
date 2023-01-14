using System.Collections.Generic;
using Celeste.Mod.SaladimHelper.Triggers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SaladimHelper;

public static class TeleportHelper
{
    public static void DoNoneCrossRoomTeleport(Player player, Vector2 vec2, KeyTeleField field)
    {
        LevelData levelData = (Engine.Scene as Level).Session.LevelData;

        if (field.AbsoluteDisplacement)
        {
            player.Position = vec2;
            player.Position += levelData.Position;
        }
        else
        {
            player.Position += vec2;
        }
        FuckTheWallCheck(player, field);
    }

    private static void FuckTheWallCheck(Player player, KeyTeleField field)
    {
        if (player.CollideCheck<Solid>())
        {
            if (field.CanPlayerFuckTheWall)
                return;
            else
                player.Die(Vector2.Zero);
        }
    }

    public static void DoCrossRoomTeleport(Player player, string targetRoomId, Vector2 offset, KeyTeleField field)
    {
        var aim = field.Aim;
        Level level = Engine.Scene as Level;
        LevelData levelData = level.Session.LevelData;
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, null, 0.1f, true);
        tween.OnUpdate = TweenOnUpdate;

        void TweenOnUpdate(Tween t)
        {
            Glitch.Value = 0.5f * t.Eased;
        };
        tween.OnComplete = TweenOnComplete;

        void TweenOnComplete(Tween _)
        {
            level.OnEndOfFrame += LevelOnEndOfFrame;
            void LevelOnEndOfFrame()
            {
                Vector2 position = player.Position;
                player.Position -= levelData.Position;
                level.Camera.Position -= levelData.Position;
                level.Session.Level = targetRoomId;
                player.CleanUpTriggers();

                var dashes = player.Dashes;
                var facing = player.Facing;
                var leader = player.Get<Leader>();
                foreach (var follower in leader.Followers)
                {
                    if (follower.Entity is null) continue;

                    follower.Entity.AddTag(Tags.Global);
                    level.Session.DoNotLoad.Add(follower.ParentEntityID);
                }

                /*core*/
                level.Remove(player);
                level.UnloadLevel();
                level.Add(player);
                level.LoadLevel(Player.IntroTypes.Transition, false);

                //播放音效
                if (field.AudioToPlay != string.Empty)
                {
                    Audio.Play(field.AudioToPlay);
                }

                levelData = level.Session.LevelData;

                if (field.AbsoluteDisplacement)
                {
                    player.Position = aim + levelData.Position;
                }
                else
                {
                    player.Position += levelData.Position;
                    player.Position += offset;
                }

                player.Facing = facing;
                level.Camera.Position += levelData.Position;
                level.Session.RespawnPoint = new Vector2?(level.Session.LevelData.Spawns.ClosestTo(player.Position));
                player.Dashes = dashes;

                Vector2 vector2 = player.Position - position;
                foreach (Follower follower in leader.Followers)
                {
                    if (follower.Entity != null)
                    {
                        follower.Entity.Position += vector2;
                        follower.Entity.RemoveTag(Tags.Global);
                        level.Session.DoNotLoad.Remove(follower.ParentEntityID);
                    }
                }
                for (int i = 0; i < leader.PastPoints.Count; i++)
                {
                    List<Vector2> pastPoints = leader.PastPoints;
                    int num = i;
                    pastPoints[num] += vector2;
                }
                leader.TransferFollowers();
                Tween tween = Tween.Create(Tween.TweenMode.Oneshot, null, 0.05f, true);
                tween.OnUpdate = t =>
                {
                    Glitch.Value = 0.5f * (1f - t.Eased);
                };
                player.Add(tween);
            };
        };

        player.Add(tween);
        FuckTheWallCheck(player, field);
    }
}