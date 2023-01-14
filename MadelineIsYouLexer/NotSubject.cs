using System.Diagnostics;

namespace MadelineIsYouLexer;

[DebuggerDisplay("(not {WarppedSubject})")]
public class NotSubject : ISubjectMarker
{
    public ISubjectMarker WarppedSubject { get; set; }

    public NotSubject(ISubjectMarker warppedSubject)
    {
        this.WarppedSubject = warppedSubject;
    }

    public bool IsFeatured(IEntity entity) => !WarppedSubject.IsFeatured(entity);

    public override string ToString() => $"(not {WarppedSubject})";
}
