using System;
using System.Collections;
using System.Reflection;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/FlagOnlyBerry"), TrackedAs(typeof(Strawberry)), Tracked]
[RegisterStrawberry(true, true)]
public class FlagOnlyBerry : Strawberry
{
    protected float flySpeed = 0;
    protected static MethodInfo FlyAwayCoroutineMethodInfo =
        typeof(Strawberry).GetMethod("FlyAwayRoutine", BindingFlags.Instance | BindingFlags.NonPublic);
    protected static FieldInfo SpriteFieldInfo =
        typeof(Strawberry).GetField("sprite", BindingFlags.Instance | BindingFlags.NonPublic);

    protected bool disappearing = false;
    protected string expectedFlag = "saladimhelper_null_flag";

    public FlagOnlyBerry(EntityData data, Vector2 offset, EntityID gid)
        : base(data, offset, gid)
    {
        expectedFlag = data.Attr("expected_flag", expectedFlag);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        (SpriteFieldInfo.GetValue(this) as Sprite).Play("flap");
    }

    public override void Update()
    {
        base.Update();
        if (!disappearing)
        {
            if (!SceneAs<Level>().Session.GetFlag(expectedFlag))
            {
                disappearing = true;
                if (!WaitingOnSeeds)
                {
                    Depth = -1000000;
                    Add(new Coroutine((IEnumerator)FlyAwayCoroutineMethodInfo.Invoke(this, new object[] { })));
                }
            }

        }
        else
        {
            Y -= flySpeed * Engine.DeltaTime;
            flySpeed += 40 * Engine.DeltaTime;
            if (Y < SceneAs<Level>().Bounds.Top - 16)
            {
                RemoveSelf();
            }
        }
    }
}
