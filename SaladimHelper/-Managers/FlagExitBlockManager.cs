using Celeste.Mod.SaladimHelper.Entities;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.SaladimHelper;

[Manager]
public class FlagExitBlockManager
{
    public static void Load()
    {
        IL.Celeste.ExitBlock.Update += ExitBlock_Update;
    }

    private static void ExitBlock_Update(ILContext il)
    {
        ILCursor cursor = new(il);
        if (cursor.TryGotoNext(ins => ins.MatchLdarg(0) && ins.Next.MatchLdfld<Entity>("Collidable")))
        {
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate((ExitBlock self) =>
            {
                if (self is FlagExitBlock feb)
                {
                    bool doSkip = feb.GetIsSkipOrigUpdateCheck();
                    return !doSkip;
                }
                return true;
            });
            cursor.TryFindNext(out var cursors, ins => ins.MatchRet());
            cursor.Emit(OpCodes.Brfalse_S, cursors[0].Next);
        }
    }

    public static void Unload()
    {
        IL.Celeste.ExitBlock.Update -= ExitBlock_Update;
    }
}
