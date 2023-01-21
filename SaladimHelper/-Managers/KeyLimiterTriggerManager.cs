namespace Celeste.Mod.SaladimHelper;

[Manager]
public class KeyLimiterTriggerManager
{
    public static KeyLimiterTriggerManager Instance { get; private set; }

    public static void Load()
    {
        On.Celeste.Player.Update += ModPlayer_Update;
    }

    private static void ModPlayer_Update(On.Celeste.Player.orig_Update orig, Player self)
    {
        if (Module.Session.LimitedMoveX)
        {
            Input.MoveX.Value = 0;
        }
        if (Module.Session.LimitedMoveY)
        {
            Input.MoveY.Value = 0;
        }
        orig(self);
    }

    public static void UnLoad()
    {
        On.Celeste.Player.Update -= ModPlayer_Update;

    }
}
