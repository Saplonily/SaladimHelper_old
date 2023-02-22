using System.Collections.Generic;
using System.Reflection;
using Celeste.Mod.SaladimHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Linq;
using Celeste.Mod.SaladimHelper.Extensions;

namespace Celeste.Mod.SaladimHelper;

[Manager]
public class MiyManager
{
    public static void Load()
    {
        On.Celeste.Player.ctor += Player_ctor;
        On.Celeste.Player.Die += Player_Die;
        On.Celeste.Player.Update += Player_Update;
        On.Celeste.PlayerDeadBody.End += PlayerDeadBody_End;
        On.Celeste.Level.NextLevel += Level_NextLevel;
    }

    private static void Level_NextLevel(On.Celeste.Level.orig_NextLevel orig, Level self, Vector2 at, Vector2 dir)
    {
        orig(self, at, dir);
        var players = self.Where(e => e is Player).OrderBy(e => Vector2.DistanceSquared(((Player)e).Position, at)).Cast<Player>();
        Player keyPlayer = players.First();
        players.Foreach(p =>
        {
            if(p != keyPlayer)
                p.RemoveSelf();
        });
        Module.Session.CurrentMiyField = null;
    }

    private static void PlayerDeadBody_End(On.Celeste.PlayerDeadBody.orig_End orig, PlayerDeadBody self)
    {
        if(!self.SceneAs<Level>().Any(e => e is Player))
        {
            orig(self);
        }
    }

    private static void Player_Update(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);
        var field = Module.Session.CurrentMiyField;
        if(field != null)
        {
            var component = self.Get<MiyMadelineComponent>();
            if(component != null)
                MiyHelper.DoBasicIsCheck(self, component);
            else
            {
                self.Add(component = new MiyMadelineComponent(true, false));
                MiyHelper.DoBasicIsCheck(self, component);
            }
        }
    }

    private static PlayerDeadBody Player_Die(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats)
    {
        if(Module.Session.CurrentMiyField != null)
        {
            PlayerDeadBody deadBody;
            if(!self.SceneAs<Level>().Any(e => e is Player p && p != self))
            {
                deadBody = orig(self, direction, evenIfInvincible, registerDeathInStats);
                Module.Session.CurrentMiyField = null;
            }
            else
            {
                deadBody = new(self, direction);
                self.SceneAs<Level>().Add(deadBody);
                self.RemoveSelf();
            }
            return deadBody;
        }
        else
        {
            return orig(self, direction, evenIfInvincible, registerDeathInStats);
        }
    }

    private static void Player_ctor(On.Celeste.Player.orig_ctor orig, Player self, Vector2 position, PlayerSpriteMode spriteMode)
    {
        orig(self, position, spriteMode);
        self.Add(new MiyMadelineComponent(true, false));
        Module.Session.CurrentMiyField = null;
    }

    public static void Unload()
    {
        On.Celeste.Player.ctor -= Player_ctor;
        On.Celeste.Player.Die -= Player_Die;
        On.Celeste.Player.Update -= Player_Update;
        On.Celeste.PlayerDeadBody.End -= PlayerDeadBody_End;
        On.Celeste.Level.NextLevel -= Level_NextLevel;
    }
}
