using System.Diagnostics;

namespace MadelineIsYouLexer;

[DebuggerDisplay("({Subject,nq} {Adv,nq} {Passive,nq})")]
public class AdvSubject : ISubjectMarker
{
    public ISubjectMarker Subject { get; set; }

    public string Adv { get; set; }

    public ISubjectMarker Passive { get; set; }

    public AdvSubject(ISubjectMarker subject, string adv, ISubjectMarker passive)
    {
        this.Subject = subject;
        this.Adv = adv;
        this.Passive = passive;
    }

    public bool IsFeatured(IEntity entity)
    {
        return Subject.IsFeatured(entity) && IsEntityFeaturedComfusAdv(entity, Adv, Passive);
    }

    private bool IsEntityFeaturedComfusAdv(IEntity entity, string adv, ISubjectMarker subjectMarker)
    {
        if(subjectMarker is SingleNameSubject singleNameSubject)
        {
            return entity.HasAdv(adv, singleNameSubject);
        }
        else if(subjectMarker is NotSubject notSubject)
        {
            return !IsEntityFeaturedComfusAdv(entity, adv, notSubject.WarppedSubject);
        }
        else if(subjectMarker is AndSubject andSubject)
        {
            return IsEntityFeaturedComfusAdv(entity, adv, andSubject.SubjectMarkerA) &&
                IsEntityFeaturedComfusAdv(entity, adv, andSubject.SubjectMarkerB);
        }
        throw new System.Exception($"{subjectMarker} as a adv passive is not allowed");
    }

    public override string ToString() => $"({Subject} {Adv} {Passive})";
}
