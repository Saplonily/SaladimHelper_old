using Monocle;

namespace Celeste.Mod.SaladimHelper;

public static class ModuleInput
{
    public static VirtualButton DoTp { get; set; }

    public static bool DoTpPressed
    {
        get => Module.Settings.UseCustomKeys ? DoTp.Pressed : Input.Talk;
    }
}