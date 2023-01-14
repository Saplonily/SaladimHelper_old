namespace MadelineIsYouLexer;

public interface IEntity
{
    /// <summary>
    /// 实体昵称(标识符)
    /// </summary>
    public string Name { get; }

    public bool HasAdv(string adv, SingleNameSubject another);
}