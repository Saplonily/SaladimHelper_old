namespace MadelineIsYouLexer;

public class NotPassive : IPassiveMarker
{
    public IPassiveMarker WarppedPassive { get; set; }

    public NotPassive(IPassiveMarker warppedPassive)
    {
        this.WarppedPassive = warppedPassive;
    }

    public override string ToString()
    {
        return $"(not {WarppedPassive})";
    }
}
