using System.Collections.Generic;
using System.Reflection;
using Celeste.Editor;

namespace Celeste.Mod.SaladimHelper;

[Manager]
public class HiddenerManager
{
    private static FieldInfo mapEditorLevelList = typeof(MapEditor).GetField("levels", BindingFlags.Instance | BindingFlags.NonPublic);

    public static void Load()
    {
        On.Celeste.Editor.MapEditor.ctor += MapEditor_ctor;
    }

    private static void MapEditor_ctor(On.Celeste.Editor.MapEditor.orig_ctor orig, Editor.MapEditor self, AreaKey area, bool reloadMapData)
    {
        orig(self, area, reloadMapData);
        /*copied from jungle*/
        List<LevelTemplate> mapList = (List<LevelTemplate>)mapEditorLevelList.GetValue(self);
        for (int i = mapList.Count - 1; i >= 0; i--)
        {
            if (mapList[i].Name.EndsWith("_HideInMap_SH"))
            {
                mapList.Remove(mapList[i]);
            }
        }

    }

    public static void Unload()
    {
        On.Celeste.Editor.MapEditor.ctor -= MapEditor_ctor;
    }
}
