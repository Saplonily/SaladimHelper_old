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
    public static ReelLimitManager Instance { get; private set; }

    public static void Load()
    {
        On.Celeste.Player.Update += Player_Update;
    }

    private static void Player_Update(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);
        var reelCamera = Module.Session.CurrentReelCamera;
        if(reelCamera != null)
        {
            Level level = self.SceneAs<Level>();
            level.CameraOffset = reelCamera.CameraPosition - self.Position;
        }
    }

    public static void UnLoad()
    {
        On.Celeste.Player.Update -= Player_Update;

    }
}
