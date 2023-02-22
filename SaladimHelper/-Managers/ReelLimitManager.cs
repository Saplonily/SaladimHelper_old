using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.SaladimHelper;

[Manager]
public class ReelLimitManager
{
    public static void Load()
    {
        On.Celeste.Player.Update += Player_Update;
        Everest.Events.Player.OnDie += _ => CameraReset();
        Everest.Events.Level.OnTransitionTo += (_, _, _) => CameraReset();
    }

    private static void CameraReset()
    {
        Module.Session.CurrentReelCamera = null;
    }

    private static void Player_Update(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);
        var reelCamera = Module.Session.CurrentReelCamera;
        if (reelCamera != null)
        {
            Level level = self.SceneAs<Level>();
            var c = level.Camera;
            level.CameraOffset = reelCamera.CameraPosition - self.Position;
            level.Camera.Position = reelCamera.CameraPosition - new Vector2(c.Right - c.Left, c.Bottom - c.Top) / 2;
        }
    }

    public static void Unload()
    {
        On.Celeste.Player.Update -= Player_Update;

    }
}
