using System.Diagnostics;

namespace MadelineIsYouLexer;

[DebuggerDisplay("({SubjectMarkerA} and {SubjectMarkerB})")]
public class AndSubject : ISubjectMarker
{
    public ISubjectMarker SubjectMarkerA { get; set; }

    public ISubjectMarker SubjectMarkerB { get; set; }

    public AndSubject(ISubjectMarker subjectMarkerA, ISubjectMarker subjectMarkerB)
    {
        SubjectMarkerA = subjectMarkerA;
        SubjectMarkerB = subjectMarkerB;
    }

    public bool IsFeatured(IEntity entity)
    {
        return SubjectMarkerA.IsFeatured(entity) || SubjectMarkerB.IsFeatured(entity);
    }

    public override string ToString()
    {
        return $"({SubjectMarkerA} and {SubjectMarkerB})";
    }
}
