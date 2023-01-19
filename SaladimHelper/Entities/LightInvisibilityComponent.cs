using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SaladimHelper.Entities;

public class LightInvisibilityComponent : Component
{
    public IInvisible InvisibleEntity { get => (IInvisible)Entity; }

    public bool Invisibled { get; protected set; }

    protected Tween opacityTween;
    protected readonly bool fearLight;

    public LightInvisibilityComponent(bool fearLight) : base(true, false)
    {
        Invisibled = fearLight;
        this.fearLight = fearLight;
    }

    public override void EntityAwake()
    {
        SceneAs<Level>().OnEndOfFrame += () =>
        {
            if (Entity != null)
            {
                InvisibleEntity.CurrentAlpha = fearLight ? 0.6f : 0.0f;
                InvisibleEntity.SetCollideAble(fearLight);
            }
        };
    }

    public override void Update()
    {
        base.Update();
        bool pInv = Invisibled;
        Invisibled = false;

        Invisibled = InvisibleEntity.LightCheck();
        if (Invisibled != pInv)
        {
            float cur = InvisibleEntity.CurrentAlpha;
            float target = Invisibled ? fearLight ? 0.0f : 0.6f : fearLight ? 0.6f : 0.0f;
            if (Entity.Contains(opacityTween))
            {
                Entity.Remove(opacityTween);
            }
            Entity.Add(opacityTween = Tween.Create(Tween.TweenMode.Persist, Ease.SineInOut, 0.3f));
            opacityTween.OnUpdate = t =>
            {
                InvisibleEntity.CurrentAlpha = Calc.LerpClamp(cur, target, t.Eased);
            };
            opacityTween.Start();
        }

        if (InvisibleEntity.CurrentAlpha <= 0.2f)
        {
            InvisibleEntity.SetCollideAble(false);
        }
        else
        {
            InvisibleEntity.SetCollideAble(true);
        }
    }
}
