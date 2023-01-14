using System.Collections.Generic;
using System.Diagnostics;

namespace MadelineIsYouLexer;

[DebuggerDisplay("({PassiveMarkerA} and {PassiveMarkerB})")]
public class AndPassive : IPassiveMarker
{
    public IPassiveMarker PassiveMarkerA { get; set; }

    public IPassiveMarker PassiveMarkerB { get; set; }

    public AndPassive(IPassiveMarker passiveMarkerA, IPassiveMarker passiveMarkerB)
    {
        PassiveMarkerA = passiveMarkerA;
        PassiveMarkerB = passiveMarkerB;
    }

    public override string ToString()
    {
        return $"({PassiveMarkerA} and {PassiveMarkerB})";
    }
}
