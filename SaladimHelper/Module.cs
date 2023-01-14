using System;
using System.Collections.Generic;
using System.Reflection;
using Celeste.Mod.SaladimHelper.Triggers;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.SaladimHelper;

public class Module : EverestModule
{
    #region for everest regists
    public static Module Instance { get; private set; }

    public static string Name => "Saladim.CelesteHelper";

    public override Type SettingsType => typeof(ModuleSettings);
    public static ModuleSettings Settings => (ModuleSettings)Instance._Settings;

    public override Type SessionType => typeof(ModuleSession);
    public static ModuleSession Session => (ModuleSession)Instance._Session;

    public override Type SaveDataType => typeof(ModuleSaveData);
    public static ModuleSaveData SaveData => (ModuleSaveData)Instance._SaveData;
    #endregion

    internal List<MethodInfo> loadMethods = new();
    internal List<MethodInfo> unloadMethods = new();

    public Module()
    {
        Instance = this;
        Logger.SetLogLevel(nameof(Module), LogLevel.Verbose);

        Logger.Log(LogLevel.Info, Name, "Searching assembly for managers...");
        var types = typeof(Module).Assembly.GetTypes();
        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<ManagerAttribute>();
            if (attr is not null)
            {
                var ins = Activator.CreateInstance(type);
                if (ins is null) continue;
                var loadMethod = type.GetMethod("Load");
                var unloadMethod = type.GetMethod("UnLoad");
                var prop = type.GetProperty("Instance");
                if (loadMethod is null) throw new Exception("load null");
                if (unloadMethod is null) throw new Exception("unload null");
                if (prop is null) throw new Exception("prop null");
                prop.SetValue(null, ins);
                loadMethods.Add(loadMethod);
                unloadMethods.Add(unloadMethod);
            }
        }
    }

    #region Load and Unload
    public override void Load()
    {
        Logger.Log(LogLevel.Info, Name, "Hook methods on Load().");

        On.Celeste.Player.Update += Player_Update;
        IL.Celeste.Player.NormalUpdate += Player_NormalUpdate;
        Everest.Events.Input.OnInitialize += Input_OnInitialize;
        foreach (var d in loadMethods)
            d.Invoke(null, new object[] { });
    }

    public override void Unload()
    {
        On.Celeste.Player.Update -= Player_Update;
        IL.Celeste.Player.NormalUpdate -= Player_NormalUpdate;
        Everest.Events.Input.OnInitialize -= Input_OnInitialize;
        foreach (var d in unloadMethods)
            d.Invoke(null, new object[] { });
    }
    #endregion


    #region Hooks

    private void Input_OnInitialize()
    {
        ModuleInput.DoTp = new VirtualButton(Settings.DoTp.Binding, Input.Gamepad, 0.08f, 0.2f);
        Logger.Log(LogLevel.Info, Name, "Input buttons initialized");
    }
    private void Player_Update(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);

        KeyTeleField.CheckAndTele(self);
    }

    private void Player_NormalUpdate(ILContext il)
    {
        ILCursor cur = new(il);

        Logger.Log(LogLevel.Info, Module.Name, "Try hooking acc Muilt...");

        while (cur.TryGotoNext(MoveType.After, ins => ins.MatchLdcR4(400.0f) || ins.MatchLdcR4(1000.0f)))
        {
            cur.EmitDelegate(() => Session.AccStep / 1000.0f);
            cur.Emit(OpCodes.Mul);
            Logger.Log(LogLevel.Info, Module.Name, "Hooked normalUpdate of acc Muilt");
        }
    }

    #endregion

}