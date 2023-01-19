using System.Linq;
using System.Reflection;
using Celeste.Mod.Entities;
using Celeste.Mod.SaladimHelper.Extensions;
using FMOD;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/LightInvisibilityFloatBlock"), Tracked, TrackedAs(typeof(FloatySpaceBlock))]
public class LightInvisibilityFloatBlock : FloatySpaceBlock, IInvisible
{
    protected LightOcclude lightOcclude;
    protected LightInvisibilityComponent lightComponent;
    protected DynamicData thisData;

    public LightInvisibilityFloatBlock(EntityData data, Vector2 offset)
        : this(
              data.Position + offset,
              data.Width,
              data.Height,
              data.Char("tiletype", '3'),
              data.Bool("disableSpawnOffset", false),
              data.Bool("fear_player_light", true)
              )
    {
    }

    public LightInvisibilityFloatBlock(Vector2 position, float width, float height, char tileType, bool disableSpawnOffset, bool fearLight)
        : base(position, width, height, tileType, disableSpawnOffset)
    {
        thisData = DynamicData.For(this);
        Add(lightComponent = new LightInvisibilityComponent(fearLight));
        lightOcclude = Get<LightOcclude>();
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (!MasterOfGroup)
            Remove(lightComponent);
    }

    public float CurrentAlpha
    {
        get => thisData.Get<TileGrid>("tiles").Alpha;
        set => thisData.Get<TileGrid>("tiles").Alpha = value;
    }

    public void SetCollideAble(bool value)
    {
        Group.Foreach(fb =>
        {
            var g = (LightInvisibilityFloatBlock)fb;
            g.Collidable = value;
            if (value is true)
            {
                if (Get<LightOcclude>() is null)
                    Add(g.lightOcclude);
            }
            else
            {
                g.lightOcclude.RemoveSelf();
            }
        });
    }

    public bool LightCheck()
    {
        bool result = false;
        foreach (LightInvisibilityFloatBlock g in Group.Cast<LightInvisibilityFloatBlock>())
        {
            if (g != this)
            {
                result |= g.SubLightCheck();
            }
        }
        return result;
    }

    public bool SubLightCheck()
    {
        Level level = SceneAs<Level>();
        var players = level.Tracker.GetEntities<Player>().Cast<Player>();
        bool isInvisible = Module.Session.SwitchedLight && players.Any(p => Collide.CircleToRect(
            p.Light.Position + p.Position,
            p.Light.EndRadius,
            new Rectangle(
                (int)Position.X, (int)Position.Y,
                (int)Width, (int)Height
                )
            ));
        return isInvisible;
    }
}
