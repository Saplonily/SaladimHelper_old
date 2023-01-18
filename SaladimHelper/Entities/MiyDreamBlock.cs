using System.Linq;
using Celeste.Mod.Entities;
using MadelineIsYouLexer;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/MiyDreamBlock"), TrackedAs(typeof(DreamBlock)), Tracked]
public class MiyDreamBlock : DreamBlock, IEntity
{
    public string Name => "dream_block";

    public MiyDreamBlock(EntityData data, Vector2 offset) : base(data, offset)
    {

    }

    public MiyDreamBlock(Vector2 position, float width, float height, Vector2? node, bool fastMoving, bool oneUse, bool below)
        : base(position, width, height, node, fastMoving, oneUse, below)
    {

    }

    public bool HasAdv(string adv, SingleNameSubject another)
    {
        if (adv == "be_ridden_by")
        {
            return another.EntityName == "madeline"
                ? SceneAs<Level>()
                    .Tracker
                    .GetEntities<Player>()
                    .Any(p => (p as Player).IsRiding(this))
                : SceneAs<Level>()
                    .Tracker
                    .GetEntities<Actor>()
                    .Where(a => (a as Actor).IsRiding(this) && (a as IEntity)?.Name == another.EntityName)
                    .Any();
        }
        return false;
    }

    public override void Update()
    {
        base.Update();
        MiyHelper.DoBasicIsCheck(this, this);
    }
}
