using Celeste.Mod.SaladimHelper.Entities;
using MadelineIsYouLexer;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SaladimHelper;

public static class MiyHelper
{
    public static void DoBasicIsCheck(Entity entity, IEntity marker)
    {
        Level level = entity.SceneAs<Level>();
        if(TryGetManager(out var ruleManager))
        {
            bool ised = false;
            if(!ruleManager.HasBeEntityFeature(marker, "is", marker.Name))
            {
                if(ruleManager.HasBeEntityFeature(marker, "is", "dream_block"))
                {
                    MiyDreamBlock block = new(entity.Position, entity.Width, entity.Height, null, false, false, false);
                    level.Add(block);
                    entity.RemoveSelf();
                    ised = true;
                }
                if(ruleManager.HasBeEntityFeature(marker, "is", "falling_block"))
                {
                    MiyFallingBlock block = new(entity.Position, 'a', (int)entity.Width, (int)entity.Height, false, false, true);
                    level.Add(block);
                    entity.RemoveSelf();
                    ised = true;
                }
                if(ruleManager.HasBeEntityFeature(marker, "is", "madeline"))
                {
                    Player p = new(entity.Center, PlayerSpriteMode.Madeline);
                    level.Add(p);
                    entity.RemoveSelf();
                    ised = true;
                }
                if(ruleManager.HasBeEntityFeature(marker, "is", "refill"))
                {
                    Refill refill = new(entity.Position, false, false);
                    level.Add(refill);
                    entity.RemoveSelf();
                    ised = true;
                }
                if(ised)
                {
                    MakeIsParticles(entity, level.ParticlesFG, 300);
                }
            }
        }
    }

    public static void MakeIsParticles(Entity entity, ParticleSystem particleSystem, int count)
    {
        particleSystem.Emit(Player.P_Split, count, entity.Center, new Vector2(entity.Width, entity.Height) / 2, Color.Yellow);
    }

    public static bool TryGetManager(out RuleManager ruleManager)
    {
        var miyField = Module.Session.CurrentMiyField;
        if(miyField is null)
        {
            ruleManager = null;
            return false;
        }
        else
        {
            ruleManager = miyField.RuleManager;
            return true;
        }
    }
}
