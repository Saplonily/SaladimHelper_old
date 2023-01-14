using System.Diagnostics;

namespace MadelineIsYouLexer;

[DebuggerDisplay("{EntityName,nq}")]
public class BeEntityPassive : IPassiveMarker
{
    public string EntityName { get; set; }

    public BeEntityPassive(string entityName)
    {
        this.EntityName = entityName;
    }

    public override string ToString() => EntityName;
}
