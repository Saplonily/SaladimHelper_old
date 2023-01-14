using Celeste.Mod.Entities;
using MadelineIsYouLexer;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/MiyRefill"), TrackedAs(typeof(Refill)), Tracked]
public class MiyRefill : Refill, IEntity
{
    public string Name => "refill";
    public DynamicData ThisData;

    public MiyRefill(EntityData data, Vector2 offset) : base(data, offset)
    {
        ThisData = DynamicData.For(this);
    }

    public MiyRefill(Vector2 position, bool twoDashes, bool oneUse) : base(position, twoDashes, oneUse)
    {
        ThisData = DynamicData.For(this);
    }

    public override void Update()
    {
        base.Update();
        MiyHelper.DoBasicIsCheck(this, this);
        if(MiyHelper.TryGetManager(out var manager))
        {
            if(ThisData.Get<bool>("twoDashes") is false && manager.HasAdjFeature(this, "is", "two"))
            {
                MiyRefill refill = new(Position, true, false);
                Level level = SceneAs<Level>();
                level.Add(refill);
                RemoveSelf();
                MiyHelper.MakeIsParticles(this, level.ParticlesFG, 40);
            }
            if(ThisData.Get<bool>("twoDashes") is true && manager.HasAdjFeature(this, "is", "one"))
            {
                MiyRefill refill = new(Position, false, false);
                Level level = SceneAs<Level>();
                level.Add(refill);
                RemoveSelf();
                MiyHelper.MakeIsParticles(this, level.ParticlesFG, 40);

            }
        }
    }


    public bool HasAdv(string adv, SingleNameSubject another)
    {
        return false;
    }
}
