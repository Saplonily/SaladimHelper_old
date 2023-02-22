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

    public static string Name => "SaladimHelper";

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
        foreach(var type in types)
        {
            var attr = type.GetCustomAttribute<ManagerAttribute>();
            if(attr is not null)
            {
                var loadMethod = type.GetMethod("Load");
                var unloadMethod = type.GetMethod("Unload");
                if(loadMethod is null) throw new Exception($"Manager {type} hasn't Load Method.");
                if(unloadMethod is null) throw new Exception($"Manager {type} hasn't Unload Method.");
                loadMethods.Add(loadMethod);
                unloadMethods.Add(unloadMethod);
            }
        }
    }

    #region Load and Unload
    public override void Load()
    {
        Logger.Log(LogLevel.Info, Name, "Hook methods on Load()...");

        On.Celeste.Player.Update += Player_Update;
        Everest.Events.Input.OnInitialize += Input_OnInitialize;
        foreach(var d in loadMethods)
            d.Invoke(null, new object[] { });
    }

    public override void Unload()
    {
        On.Celeste.Player.Update -= Player_Update;
        Everest.Events.Input.OnInitialize -= Input_OnInitialize;
        foreach(var d in unloadMethods)
            d.Invoke(null, new object[] { });
    }
    #endregion

    private void Input_OnInitialize()
    {
        ModuleInput.DoATeleportOrLightSwitch = new VirtualButton(Settings.DoATeleportOrLightSwitch.Binding, Input.Gamepad, 0.08f, 0.2f);
        Logger.Log(LogLevel.Info, Name, "Input buttons initialized");
    }
    private void Player_Update(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);

        KeyTeleField.CheckAndTele(self);
    }

}