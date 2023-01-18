using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.SaladimHelper.Triggers;

[CustomEntity("SaladimHelper/LightSwitchField")]
public class LightSwitchField : Trigger
{
    public LightSwitchField(EntityData data, Vector2 offset) : base(data, offset)
    {

    }

    public override void OnStay(Player player)
    {
        ButtonBinding b = Module.Settings.DoATeleportOrLightSwitch;
        if (b.Pressed)
        {
            Module.Session.SwitchedLight = !Module.Session.SwitchedLight;
            b.ConsumeBuffer();
        }
    }
}
