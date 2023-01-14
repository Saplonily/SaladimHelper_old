using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.SaladimHelper;

public class ReelLimitActionInfo
{
    public Vector2 Start { get; set; }
    public Vector2 End { get; set; }
    public float Step { get; set; }
    public bool SquashHorizontal { get; set; }
    public bool SquashVertival { get; set; }

    public Vector2 StepVec
    {
        get
        {
            var dir = End - Start;
            var length = dir.Length();
            var step = dir / length * Step;
            return step;
        }
    }
}
