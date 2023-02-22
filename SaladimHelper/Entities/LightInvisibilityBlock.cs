using System;
using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.SaladimHelper.Extensions;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/LightInvisibilityBlock"), TrackedAs(typeof(Solid))]
public class LightInvisibilityBlock : Solid, IInvisible
{
    protected float lightFearRadiusRadio = 1.0f;
    protected LightOcclude lightOcclude;
    protected TileGrid tiles;
    protected LightInvisibilityComponent lightInvisibilityComponent;

    public LightInvisibilityBlock(EntityData data, Vector2 offset)
        : this(data.Position + offset,
              data.Width,
              data.Height,
              data.Char("tile_type", '3'),
              data.Bool("fear_player_light"),
              data.Float("light_radius_radio", 1.0f)
              )
    { }

    public LightInvisibilityBlock(Vector2 position, float width, float height, char tile, bool fearPlayerLight, float radiusRadio = 1.0f)
        : base(position, width, height, true)
    {
        Add(tiles = GFX.FGAutotiler.GenerateBox(tile, (int)Math.Round(width / 8), (int)Math.Round(height / 8)).TileGrid);
        Add(lightInvisibilityComponent = new LightInvisibilityComponent(fearPlayerLight));
        Add(lightOcclude = new LightOcclude(1.0f));
    }

    public float CurrentAlpha { get => tiles.Alpha; set => tiles.Alpha = value; }

    public void SetCollideAble(bool value)
    {
        this.Collidable = value;
        if (value is true)
            Add(lightOcclude);
        else
            lightOcclude.RemoveSelf();
    }

    public bool LightCheck()
    {
        Level level = SceneAs<Level>();
        var players = level.Tracker.GetEntities<Player>().Cast<Player>();
        bool isInvisible = Module.Session.SwitchedLight && players.Any(p => Collide.CircleToRect(
            p.Light.Position + p.Position,
            p.Light.EndRadius * lightFearRadiusRadio,
            new Rectangle(
                (int)Position.X, (int)Position.Y,
                (int)Width, (int)Height
                )
            ));
        return isInvisible;
    }
}
