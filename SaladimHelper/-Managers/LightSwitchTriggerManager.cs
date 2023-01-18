using MonoMod.Utils;
using System.Linq;

namespace Celeste.Mod.SaladimHelper;

[Manager]
public class LightSwitchTriggerManager
{
    public static LightSwitchTriggerManager Instance { get; private set; }

    public static void Load()
    {
        On.Celeste.LightingRenderer.BeforeRender += LightingRenderer_BeforeRender;
    }

    private static void LightingRenderer_BeforeRender(
        On.Celeste.LightingRenderer.orig_BeforeRender orig, LightingRenderer self, Monocle.Scene scene
        )
    {
        if (Module.Session.SwitchedLight)
        {
            orig(self, scene);
            return;
        }

        // copy from the ExtendedVariantMode

        float origSpotlightAlpha = 0f;

        Player player = scene?.Tracker.GetEntity<Player>();
        PlayerDeadBody deadPlayer = null;
        if (player is null)
        {
            deadPlayer = scene?.Entities?.OfType<PlayerDeadBody>().FirstOrDefault();
            if (deadPlayer is not null)
            {
                VertexLight light = new DynData<PlayerDeadBody>(deadPlayer).Get<VertexLight>("light");
                origSpotlightAlpha = light.Alpha;
                light.Alpha = 0f;
            }
        }
        else
        {
            origSpotlightAlpha = player.Light.Alpha;
            player.Light.Alpha = 0f;
        }
        
        orig(self, scene);

        if (player is not null)
        {
            player.Light.Alpha = origSpotlightAlpha;
        }
        else if (deadPlayer is not null)
        {
            VertexLight light = new DynData<PlayerDeadBody>(deadPlayer).Get<VertexLight>("light");
            light.Alpha = origSpotlightAlpha;
        }
    }

    public static void UnLoad()
    {
        On.Celeste.LightingRenderer.BeforeRender -= LightingRenderer_BeforeRender;
    }
}
