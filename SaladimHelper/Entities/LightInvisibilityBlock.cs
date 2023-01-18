using System;
using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.SaladimHelper.Extensions;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/LightInvisibilityBlock")]
public class LightInvisibilityBlock : Solid
{
    public bool Invisibled { get; protected set; }

    protected Tween opacityTween;
    protected TileGrid tiles;

    public LightInvisibilityBlock(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Width, data.Height, data.Char("tile_type", '3'))
    { }

    public LightInvisibilityBlock(Vector2 position, float width, float height, char tile) : base(position, width, height, true)
    {
        Invisibled = false;
        Add(tiles = GFX.FGAutotiler.GenerateBox(tile, (int)Math.Round(width / 8), (int)Math.Round(height / 8)).TileGrid);
    }

    public override void Update()
    {
        base.Update();
        bool pInv = Invisibled;
        Invisibled = false;
        Level level = SceneAs<Level>();
        var players = level.Tracker.GetEntities<Player>().Cast<Player>();
        bool isInvisible = Module.Session.SwitchedLight && players.Any(p => Collide.CircleToRect(
            p.Light.Position + p.Position, p.Light.EndRadius, new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height)
            ));
        Invisibled = isInvisible;
        if (Invisibled != pInv)
        {
            float cur = tiles.Alpha;
            float target = Invisibled ? 0.0f : 1.0f;
            if (this.Contains(opacityTween))
            {
                Remove(opacityTween);
            }
            Add(opacityTween = Tween.Create(Tween.TweenMode.Persist, Ease.SineInOut, 0.3f));
            opacityTween.OnUpdate = t =>
            {
                tiles.Alpha = Calc.LerpClamp(cur, target, t.Eased);
            };
            opacityTween.Start();
        }

        if (tiles.Alpha <= 0.2f)
        {
            this.Collidable = false;
        }
        else
        {
            this.Collidable = true;
        }
    }
}
