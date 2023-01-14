namespace MadelineIsYouLexer;

public class AdjPassive : IPassiveMarker
{
    public string AdjName { get; set; }

    public AdjPassive(string adjName)
    {
        this.AdjName = adjName;
    }

    public override string ToString() => AdjName;
}
