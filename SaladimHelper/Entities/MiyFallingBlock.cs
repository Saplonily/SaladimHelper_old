using System;
using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.SaladimHelper.Extensions;
using MadelineIsYouLexer;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/MiyFallingBlock"), Tracked(false), TrackedAs(typeof(FallingBlock))]
public class MiyFallingBlock : FallingBlock, IEntity
{
    public DynamicData ThisData;

    public bool BeCrushedJustNow;

    public string Name => "falling_block";

    public MiyFallingBlock(EntityData data, Vector2 offset) : base(data, offset)
    {
        ThisData = new(this);
        this.OnDashCollide = OnDashCollideHandler;
    }

    public MiyFallingBlock(Vector2 position, char tile, int width, int height, bool finalBoss, bool behind, bool climbFall)
        : base(position, tile, width, height, finalBoss, behind, climbFall)
    {
    }

    private DashCollisionResults OnDashCollideHandler(Player p, Vector2 dir)
    {
        var miyField = Module.Session.CurrentMiyField;
        if (miyField is not null)
        {
            BeCrushedJustNow = true;
        }
        return DashCollisionResults.NormalCollision;
    }

    public override void Update()
    {
        base.Update();
        if (MiyHelper.DoBasicIsCheck(this, this))
        {
            BeCrushedJustNow = false;
        }
    }

    public bool HasAdv(string adv, SingleNameSubject another)
    {
        if (adv == "be_crushed_by" && another.EntityName == "madeline" && BeCrushedJustNow)
        {
            return true;
        }
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
}
