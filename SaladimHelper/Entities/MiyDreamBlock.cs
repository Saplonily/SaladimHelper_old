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
        return false;
    }

    public override void Update()
    {
        base.Update();
        MiyHelper.DoBasicIsCheck(this, this);
    }
}
