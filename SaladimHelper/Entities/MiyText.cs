using Celeste.Mod.Entities;
using Celeste.Mod.SaladimHelper.Triggers;
using MadelineIsYouLexer;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/MiyText"), Tracked(false)]
public class MiyText : Solid
{
    private bool HasTexture = false;

    public string Text { get; protected set; }


    public ParseResult ParseState;

    public MiyText(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Width, data.Height, data.Attr("text", "baba"), data.Attr("texture", ""))
    {

    }

    public MiyText(Vector2 position, float width, float height, string text, string textureName)
        : base(position, width, height, true)
    {
        Text = text;
        if(!string.IsNullOrEmpty(textureName))
        {
            Add(new Image(GFX.Game[$"objects/MIY/{textureName}"]));
            HasTexture = true;
        }
        OnDashCollide = (p, dir) =>
        {
            Level level = SceneAs<Level>();
            Vector2 from = Position;
            Vector2 to = Position + new Vector2(dir.X * Width, dir.Y * Height);
            if(!CollideCheck<Solid>(to) && Get<Tween>() is null)
            {
                if(CollideCheck<MiyRuleManagedField>(to))
                {
                    float time = 0.2f;
                    Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.ExpoOut, time);
                    tween.OnUpdate = t =>
                    {
                        Vector2 lerp = Calc.LerpSnap(from, to, t.Eased, 0.1f);
                        MoveHCollideSolidsAndBounds(level, lerp.X - Position.X, false);
                        MoveVCollideSolidsAndBounds(level, lerp.Y - Position.Y, false, checkBottom: false);
                    };
                    tween.OnComplete = t =>
                    {
                        CollideFirst<MiyRuleManagedField>()?.UpdateRuleManager();
                    };
                    Add(tween);
                    tween.Start();
                }
                return DashCollisionResults.Rebound;
            }
            return DashCollisionResults.NormalCollision;
        };
    }

    public override void Render()
    {
        base.Render();
        if(ParseState == ParseResult.Invalid)
            Draw.HollowRect(X, Y, Width, Height, Color.Gray);
        if(ParseState == ParseResult.Right)
            Draw.HollowRect(X, Y, Width, Height, Color.Gold);
        if(ParseState == ParseResult.Reject)
            Draw.HollowRect(X, Y, Width, Height, Color.Red);
        if(!HasTexture)
            ActiveFont.Draw(Text, Position, Vector2.Zero, new Vector2(0.2f, 0.2f), Color.White);
    }

    public void OnParsed(ParseResult parseResult)
    {
        Level level = SceneAs<Level>();
        if(parseResult != ParseState)
        {
            switch(parseResult)
            {
                case ParseResult.Right:
                level.ParticlesFG.Emit(Player.P_Split, 30, Center, new Vector2(Width, Height) / 2, Color.Green);
                break;
                case ParseResult.Reject:
                level.ParticlesFG.Emit(Player.P_Split, 80, Center, new Vector2(Width, Height) / 2, Color.Red);
                break;
            }
        }
        ParseState = parseResult;
    }
}
