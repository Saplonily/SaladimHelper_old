using MonoMod.ModInterop;
namespace Celeste.Mod.SaladimHelper
{
    /// <summary>
    /// Provides export functions for other mods to import.
    /// If you do not need to export any functions, delete this class and the corresponding call
    /// to ModInterop() in <see cref="Module.Load"/>
    /// </summary>
    [ModExportName("CelesteMod")]
    public static class Exports
    {
        // TODO: add your mod's exports, if required
    }
}
