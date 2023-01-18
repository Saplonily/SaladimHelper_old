using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.SaladimHelper;

[SettingName("Saladim Celeste Helper Settings")]
public class ModuleSettings : EverestModuleSettings
{
    [DefaultButtonBinding(Buttons.RightShoulder, Keys.X)]
    public ButtonBinding DoATeleportOrLightSwitch { get; set; } = new();
}
