using Celeste.Mod.Entities;
using Celeste.Mod.SaladimHelper.Entities;
using Celeste.Mod.SaladimHelper.Extensions;
using MadelineIsYouLexer;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.SaladimHelper.Triggers;

[CustomEntity("SaladimHelper/MiyRuleManagedField"), Tracked(false)]
public class MiyRuleManagedField : Trigger
{
    public RuleManager RuleManager;
    public int GridSize;

    public MiyRuleManagedField(EntityData data, Vector2 offset) : base(data, offset)
    {
        RuleManager = new(
            new string[] { "madeline", "dashes", "falling_block", "dream_block", "refill" },
            new string[] { "be_crushed_by" },
            new string[] { "has", "is" },
            new string[] { "two", "one" }
            );
        GridSize = data.Int("grid_size", 16);
    }

    public void UpdateRuleManager()
    {
        IEnumerable<MiyText> miyTexts = CollideAll<MiyText>().Cast<MiyText>();
        List<LocatedWord> locatedWords = new();
        foreach(var miyText in miyTexts)
        {
            int x = (int)Math.Round((miyText.X - X) / GridSize);
            int y = (int)Math.Round((miyText.Y - Y) / GridSize);
            locatedWords.Add(new(x, y, miyText.Text, miyText.OnParsed));
        }
        RuleManager.ParseAndUse(locatedWords);
        if(RuleManager.Rules.Count != 0)
        {
            Logger.Log(LogLevel.Info, "MiySaladimHelper", "Rules contains:");
            foreach(var rule in RuleManager.Rules)
            {
                Logger.Log(LogLevel.Info, "MiySaladimHelper", rule.ToString());
            }
            Logger.Log(LogLevel.Info, "MiySaladimHelper", "Rules end.");
        }
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        UpdateRuleManager();
    }

    public override void Update()
    {
        Level level = SceneAs<Level>();
        if(RuleManager.HasAdjFeature(Module.Session.GetNoneAdvAbleEntity("dashes"), "is", "two"))
        {
            level.Session.Inventory.Dashes = 2;
        }
        if(RuleManager.HasAdjFeature(Module.Session.GetNoneAdvAbleEntity("dashes"), "is", "one"))
        {
            level.Session.Inventory.Dashes = 1;
        }
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        Module.Session.CurrentMiyField = this;
    }
}
