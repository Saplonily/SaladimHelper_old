using Celeste.Mod.SaladimHelper.Entities;
using Celeste.Mod.SaladimHelper.Triggers;
using MadelineIsYouLexer;
using YamlDotNet.Serialization;

namespace Celeste.Mod.SaladimHelper;

public class ModuleSession : EverestModuleSession
{

    public int AccStep { get; set; } = 1000;

    public bool LimitedMoveX { get; set; } = false;

    public bool LimitedMoveY { get; set; } = false;

    #region do not save these
    [YamlIgnore]
    public KeyTeleField TeleFieldIn = null;

    [YamlIgnore]
    public ReelCamera CurrentReelCamera = null;

    [YamlIgnore]
    public NoneAdvAbleEntity NoneAdvAbleEntity = new();

    [YamlIgnore]
    public MiyRuleManagedField CurrentMiyField;

    [YamlIgnore]
    public bool SwitchedLight = false;
    #endregion


    public NoneAdvAbleEntity GetNoneAdvAbleEntity(string name)
    {
        NoneAdvAbleEntity.Name = name;
        return NoneAdvAbleEntity;
    }
}

public class NoneAdvAbleEntity : IEntity
{
    public string Name { get; set; }

    public bool HasAdv(string adv, SingleNameSubject another) => false;
}