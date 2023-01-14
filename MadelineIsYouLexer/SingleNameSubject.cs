namespace MadelineIsYouLexer;

public class SingleNameSubject : ISubjectMarker
{
    public string EntityName { get; set; }

    public SingleNameSubject(string entityName)
    {
        EntityName = entityName;
    }

    public bool IsFeatured(IEntity entity)
        => EntityName == entity.Name;

    public override string ToString()
    {
        return EntityName;
    }
}
