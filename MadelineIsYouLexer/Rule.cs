using System.Collections.Generic;

namespace MadelineIsYouLexer;

public class Rule
{
    public ISubjectMarker Subject { get; set; }

    public string Action { get; set; }

    public IPassiveMarker Passive { get; set; }

    public Rule(ISubjectMarker subject, string action, IPassiveMarker passive)
    {
        this.Subject = subject;
        this.Action = action;
        this.Passive = passive;
    }

    public override string ToString()
    {
        return $"{Subject} {Action} {Passive}";
    }
}

public interface ISubjectMarker
{
    /// <summary>
    /// 这个主语是否指定了指定的实体
    /// </summary>
    public bool IsFeatured(IEntity entity);
}

public interface IPassiveMarker
{
}
