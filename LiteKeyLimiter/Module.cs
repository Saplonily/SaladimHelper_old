namespace Celeste.Mod.LiteKeyLimiter;

public class Module : EverestModule
{
    public static Module Instance { get; private set; }

    public override Type SessionType => typeof(ModuleSession);

    public static ModuleSession Session => (ModuleSession)Instance._Session;

    public Module()
    {
        Instance = this;
    }

    public override void Load()
    {
        Logger.Log(LogLevel.Info, "LiteKeyLimiter", $"Hooking all needing...");
        On.Celeste.Player.Update += this.ModPlayer_Update;
    }

    private void ModPlayer_Update(On.Celeste.Player.orig_Update orig, Player self)
    {
        if (Session.LimitedMoveX)
        {
            Input.MoveX.Value = 0;
        }
        if (Session.LimitedMoveY)
        {
            Input.MoveY.Value = 0;
        }
        orig(self);
    }

    public override void Unload()
    {
        On.Celeste.Player.Update -= this.ModPlayer_Update;
    }
}