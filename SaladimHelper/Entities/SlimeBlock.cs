using System;
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/SlimeBlock"), Tracked, TrackedAs(typeof(Solid))]
public class SlimeBlock : Solid
{
    protected PlayerCollider playerCollider;
    protected float bounceLevelValue;

    public SlimeBlock(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Width, data.Height, data.Float("level"))
    {

    }

    public SlimeBlock(Vector2 position, float width, float height, float bonuceLevel)
        : base(position, width, height, true)
    {
        bounceLevelValue = bonuceLevel;
        Collider = new Hitbox(width, height);
        OnDashCollide = (p, dir) =>
        {
            DoBounce(p);
            return DashCollisionResults.Bounce;
        };
    }

    public override void Update()
    {
        base.Update();
        var level = SceneAs<Level>();
        foreach (Player player in level.Tracker.GetEntities<Player>().Cast<Player>())
        {
            if (!player.IsRiding(this) && player.CollideCheck(this, player.Position + player.Speed * Engine.DeltaTime))
            {
                DoBounce(player);
            }
        }
    }

    public void DoBounce(Player player)
    {
        Vector2 normal = -Vector2.UnitY;
        if (player.Y >= Y + Height)
        {
            normal = Vector2.UnitY;
        }
        else if (player.Y <= Y)
        {
            normal = -Vector2.UnitY;
            if (!SceneAs<Level>().Session.Inventory.NoRefills)
                player.RefillDash();
            player.RefillStamina();
        }
        else if (player.Y <= Y + Height && player.Y >= Y)
        {
            if (player.X < X)
            {
                normal = -Vector2.UnitX;
            }
            else if (player.X > X + Width)
            {
                normal = Vector2.UnitX;
            }
        }
        player.Speed = Vector2.Reflect(player.Speed, normal);
        player.StateMachine.State = Player.StNormal;
        player.Speed *= bounceLevelValue;
    }

    public override void Render()
    {
        base.Render();
        Draw.Rect(X, Y, Width, Height, Color.Lime);
    }
}
