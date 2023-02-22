using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/FlagExitBlock")]
public class FlagExitBlock : ExitBlock
{
    protected string expectedFlag = "saladimhelper_null_flag";

    public FlagExitBlock(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Width, data.Height, data.Char("tileType", '3'), data.Attr("expected_flag"))
    {
    }

    public FlagExitBlock(Vector2 position, float width, float height, char tileType, string expectedFlag)
        : base(position, width, height, tileType)
    {
        this.expectedFlag = expectedFlag;
    }

    public bool GetIsSkipOrigUpdateCheck()
    {
        Logger.Log(LogLevel.Info, "S", $"expected check: {expectedFlag}");
        return !SceneAs<Level>().Session.GetFlag(expectedFlag);
    }
}
