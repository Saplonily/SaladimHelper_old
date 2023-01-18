using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.SaladimHelper.Triggers;

[CustomEntity("SaladimHelper/KeyTeleField")]
public class KeyTeleField : Trigger
{
    public bool CrossRoom = false;
    public bool CanPlayerFuckTheWall = true;
    public bool AbsoluteDisplacement = false;
    public string AudioToPlay = "";
    public string TargetRoomId = "";

    public Vector2 Aim = new(0.0f, 0.0f);
    public KeyTeleField(EntityData data, Vector2 offset) : base(data, offset)
    {
        float f1 = data.Float("aim_x", 0);
        float f2 = data.Float("aim_y", 0);
        CrossRoom = data.Bool("cross_room", false);
        TargetRoomId = data.Attr("target_room_id", "");
        AudioToPlay = data.Attr("audio_to_play", string.Empty);
        CanPlayerFuckTheWall = !data.Bool("kill_player_on_collide_wall", true);
        AbsoluteDisplacement = data.Bool("absolute_displacement", false);
        Aim = new(f1, f2);
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        Module.Session.TeleFieldIn = this;
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        Module.Session.TeleFieldIn = null;
    }

    public static void CheckAndTele(Player player)
    {
        if (!TryGetCurrentField(out var field)) return;
        if (!player.CollideCheck(field))
        {
            Module.Session.TeleFieldIn = null; return;
        }
        if (ModuleInput.DoATeleportOrLightSwitch.Pressed)
        {
            var str = $"Pressed DoTp input,start teleport player...,target room sid={field.TargetRoomId},aim={field.Aim}";
            Logger.Log(LogLevel.Verbose, Module.Name, str);

            if (!field.CrossRoom)
            {
                TeleportHelper.DoNoneCrossRoomTeleport(player, field.Aim, field);
            }
            else
            {
                TeleportHelper.DoCrossRoomTeleport(player, field.TargetRoomId, field.Aim, field);
            }

            ModuleInput.DoATeleportOrLightSwitch.ConsumeBuffer();
        }
    }

    public static bool TryGetCurrentField(out KeyTeleField field)
    {
        var f = Module.Session.TeleFieldIn;
        field = f;
        return f is not null;
    }
}