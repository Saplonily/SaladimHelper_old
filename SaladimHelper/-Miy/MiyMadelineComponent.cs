using MadelineIsYouLexer;
using Monocle;

namespace Celeste.Mod.SaladimHelper;

public class MiyMadelineComponent : Component, IEntity
{
    public string Name => "madeline";

    public MiyMadelineComponent(bool active, bool visible) : base(active, visible)
    {
    }


    public bool HasAdv(string adv, SingleNameSubject another)
    {
        //TODO madeline的属性
        return false;
    }
}
