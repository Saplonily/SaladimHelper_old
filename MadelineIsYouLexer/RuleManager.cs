using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MadelineIsYouLexer;

public class RuleManager
{
    public const string Not = "not";
    public const string And = "and";

    private readonly List<Rule> rules;
    private readonly List<string> entityNames;
    private readonly List<string> entityAdvs;
    private readonly List<string> actions;
    private readonly List<string> features;
    private readonly Dictionary<Rule, List<LocatedWord>> wordsOfRule;
    private readonly List<LocatedWord> lastWords;

    public IReadOnlyCollection<Rule> Rules => rules;

    public RuleManager(IEnumerable<string> entityNames, IEnumerable<string> entityAdvs, IEnumerable<string> actions, IEnumerable<string> features)
    {
        rules = new();
        this.entityNames = new();
        this.entityAdvs = new();
        this.actions = new();
        this.features = new();

        this.entityNames.AddRange(entityNames);
        this.entityAdvs.AddRange(entityAdvs);
        this.actions.AddRange(actions);
        this.features.AddRange(features);

        this.wordsOfRule = new();
        this.lastWords = new();
    }

    public WordType GetWordType(string str)
    {
        if(entityNames.Contains(str)) return WordType.EntityName;
        if(entityAdvs.Contains(str)) return WordType.Adv;
        if(actions.Contains(str)) return WordType.Action;
        if(features.Contains(str)) return WordType.Feature;
        if(str == Not) return WordType.Not;
        if(str == And) return WordType.And;
        return WordType.Invalid;
    }

    public bool HasAdjFeature(IEntity entity, string action, string feature)
    {
        bool rightSide = false;
        bool leftSide = false;
        foreach(var rule in rules)
        {
            CheckPassive(rule, rule.Passive);
        }
        if(rightSide && !leftSide)
            return true;
        return false;
        bool CheckAdjPassive(Rule rule, AdjPassive adjPassive)
            => rule.Action == action && adjPassive.AdjName == feature && rule.Subject.IsFeatured(entity);
        void CheckPassive(Rule rule, IPassiveMarker passiveMarker)
        {
            var rootPassive = passiveMarker;
            if(rootPassive is AdjPassive adjPassive)
            {
                rightSide |= CheckAdjPassive(rule, adjPassive);
            }
            else if(rootPassive is NotPassive notPassive && notPassive.WarppedPassive is AdjPassive adjPassive2)
            {
                leftSide |= CheckAdjPassive(rule, adjPassive2);
            }
            else if(rootPassive is AndPassive andPassive)
            {
                CheckPassive(rule, andPassive.PassiveMarkerA);
                CheckPassive(rule, andPassive.PassiveMarkerB);
            }
        }
    }

    public bool HasBeEntityFeature(IEntity entity, string action, string entityName)
    {
        bool rightSide = false;
        bool leftSide = false;
        foreach(var rule in rules)
        {
            CheckPassive(rule, rule.Passive);
        }
        if(rightSide && !leftSide)
            return true;
        return false;
        bool CheckBeEntityPassive(Rule rule, BeEntityPassive beEntityPassive)
            => rule.Action == action && beEntityPassive.EntityName == entityName && rule.Subject.IsFeatured(entity);
        void CheckPassive(Rule rule, IPassiveMarker passiveMarker)
        {
            var rootPassive = passiveMarker;
            if(rootPassive is BeEntityPassive beEntityPassive)
            {
                rightSide |= CheckBeEntityPassive(rule, beEntityPassive);
            }
            else if(rootPassive is NotPassive notPassive && notPassive.WarppedPassive is BeEntityPassive beEntityPassive1)
            {
                leftSide |= CheckBeEntityPassive(rule, beEntityPassive1);
            }
            else if(rootPassive is AndPassive andPassive)
            {
                CheckPassive(rule, andPassive.PassiveMarkerA);
                CheckPassive(rule, andPassive.PassiveMarkerB);
            }
        }
    }

    public static IPassiveMarker RootNotOf(IPassiveMarker passive)
    {
        var curPassive = passive;
        if(curPassive is not NotPassive notPassive)
        {
            if(curPassive is AndPassive andPassive)
            {
                return new AndPassive(RootNotOf(andPassive.PassiveMarkerA), RootNotOf(andPassive.PassiveMarkerB));
            }
            return curPassive;
        }
        curPassive = notPassive.WarppedPassive;
        if(curPassive is NotPassive notPassiveDeeper)
            return RootNotOf(notPassiveDeeper.WarppedPassive);
        else
            return passive;
    }

    public static ISubjectMarker RootNotOf(ISubjectMarker subject)
    {
        var curSubject = subject;
        if(curSubject is not NotSubject notSubject)
        {
            if(curSubject is AdvSubject advSubject)
            {
                return new AdvSubject(RootNotOf(advSubject.Subject), advSubject.Adv, RootNotOf(advSubject.Passive));
            }
            else if(curSubject is AndSubject andSubject)
            {
                return new AndSubject(RootNotOf(andSubject.SubjectMarkerA), RootNotOf(andSubject.SubjectMarkerB));
            }
            return curSubject;
        }
        curSubject = notSubject.WarppedSubject;
        if(curSubject is NotSubject notSubjectDeeper)
            return RootNotOf(notSubjectDeeper.WarppedSubject);
        else
            return subject;
    }

    public void ParseAndUse(IEnumerable<LocatedWord> locatedWords)
    {
        Dictionary<Point, LocatedWord> dic = locatedWords.ToDictionary(word => new Point(word.X, word.Y));
        rules.Clear();
        wordsOfRule.Clear();
        lastWords.Clear();
        lastWords.AddRange(locatedWords);
        foreach(var word in dic.Values)
        {
            for(int dir = 0; dir <= 1; dir++)
            {
                int xUnit = dir == 0 ? 1 : 0;
                int yUnit = dir == 1 ? 1 : 0;
                bool TryGetAt(int index, out LocatedWord outWord) => dic.TryGetWord(word.X + xUnit * index, word.Y + yUnit * index, out outWord);
                if(GetWordType(word) == WordType.EntityName)
                {
                    ISubjectMarker subject;
                    int verbIndex = 0;
                    //获取自己被not后的subject
                    TryParseNotSubject(0, true, out int nots);
                    //通过判断前面还是不是带了(adv或and)和entityName看看自己是不是作为adv的参数, 是则退出
                    if(TryGetAt(-1 - nots, out var testWord1))
                    {
                        if(GetWordType(testWord1) is WordType.Adv or WordType.And)
                        {
                            if(TryGetAt(-2 - nots, out var testWord2))
                            {
                                if(GetWordType(testWord2) == WordType.EntityName)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    ISubjectMarker nottedSubject = TryParseAndSubjectChain(-nots, out int firstSubjectChainLength);
                    subject = nottedSubject;
                    //解析其后所跟的adv
                    if(TryGetAt(firstSubjectChainLength - nots, out var wordx1))
                    {
                        //falling_blokc be_crushed_by is madeline

                        //madeline climb not falling_block has two_dashes
                        //0        1     2   3             4   5
                        switch(GetWordType(wordx1))
                        {
                            //后一个位置是 adv, 开始解析为一个subject
                            case WordType.Adv:
                            ISubjectMarker marker = TryParseAndSubjectChain(2, out int chainLength);
                            //后跟一个adv但是没有第二个subject, 直接退出这次解析, 因为不可能这个subject会解析出规则
                            if(marker is null)
                                continue;
                            subject = new AdvSubject(nottedSubject, wordx1, marker);
                            verbIndex = firstSubjectChainLength - nots + 1 + chainLength;

                            break;
                            case WordType.Action:
                            verbIndex = firstSubjectChainLength - nots;
                            break;
                            default: continue;
                        }
                    }
                    //解析词附近的not, ind处为entityName时且before是true检查前面的not, isBefore为false时一直检查到后面的第一个entityName
                    ISubjectMarker TryParseNotSubject(int ind, bool isBefore, out int nots)
                    {
                        if(TryGetAt(ind, out var wordx0))
                        {
                            switch(GetWordType(wordx0))
                            {
                                case WordType.EntityName:
                                {
                                    if(isBefore)
                                    {
                                        int notCounts = ParseCountBeforeNotSubject(ind);
                                        nots = notCounts;
                                        SingleNameSubject singleNameSubject = new(wordx0);
                                        ISubjectMarker rstMarker = singleNameSubject;
                                        while(notCounts > 0)
                                        {
                                            notCounts--;
                                            rstMarker = new NotSubject(rstMarker);
                                        }
                                        return rstMarker;
                                    }
                                    else
                                    {
                                        nots = 0;
                                        return new SingleNameSubject(wordx0);
                                    }
                                }
                                //是not, 表示期望解析后面的一串not
                                case WordType.Not:
                                {
                                    if(!isBefore)
                                    {
                                        int i = 0;
                                        while(true)
                                        {
                                            if(TryGetAt(ind + i, out var wordxi))
                                            {
                                                if(GetWordType(wordxi) != WordType.Not)
                                                    break;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                            i++;
                                        }
                                        nots = i;
                                        int notCounts = nots;
                                        if(TryGetAt(ind + i, out var wordxi1))
                                        {
                                            if(GetWordType(wordxi1) == WordType.EntityName)
                                            {
                                                SingleNameSubject singleNameSubject = new(wordxi1);
                                                ISubjectMarker rstMarker = singleNameSubject;
                                                while(notCounts > 0)
                                                {
                                                    notCounts--;
                                                    rstMarker = new NotSubject(rstMarker);
                                                }
                                                return rstMarker;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        nots = int.MinValue;
                                        return null;
                                    }
                                }
                                break;
                            }
                        }
                        nots = int.MinValue;
                        return null;
                        int ParseCountBeforeNotSubject(int ind)
                        {
                            int count = 0;
                            for(int i = -1; ; i--)
                            {
                                if(TryGetAt(ind + i, out var wordx0))
                                {
                                    switch(GetWordType(wordx0))
                                    {
                                        case WordType.Not: count++; break;
                                        default: return count;
                                    }
                                }
                                else
                                {
                                    return count;
                                }
                            }
                        }
                    }

                    IPassiveMarker TryParseNotPassive(int ind, out int nots)
                    {
                        if(TryGetAt(ind, out var wordx0))
                        {
                            switch(GetWordType(wordx0))
                            {
                                case WordType.EntityName:
                                {
                                    nots = 0;
                                    return new BeEntityPassive(wordx0);
                                }
                                case WordType.Feature:
                                {
                                    nots = 0;
                                    return new AdjPassive(wordx0);
                                }
                                //是not, 表示期望解析后面的一串not
                                case WordType.Not:
                                {
                                    int i = 0;
                                    while(true)
                                    {
                                        if(TryGetAt(ind + i, out var wordxi))
                                        {
                                            if(GetWordType(wordxi) != WordType.Not)
                                                break;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        i++;
                                    }
                                    nots = i;
                                    int notCounts = nots;
                                    if(TryGetAt(ind + i, out var wordxi1))
                                    {
                                        IPassiveMarker passive = null;
                                        switch(GetWordType(wordxi1))
                                        {
                                            case WordType.EntityName:
                                            passive = new BeEntityPassive(wordxi1);
                                            break;
                                            case WordType.Feature:
                                            passive = new AdjPassive(wordxi1);
                                            break;
                                        }
                                        if(passive != null)
                                        {
                                            IPassiveMarker rstMarker = passive;
                                            while(notCounts > 0)
                                            {
                                                notCounts--;
                                                rstMarker = new NotPassive(rstMarker);
                                            }
                                            return rstMarker;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                        nots = int.MinValue;
                        return null;
                    }

                    //解析词后面的and, 从此词开始解析后面的and链
                    ISubjectMarker TryParseAndSubjectChain(int ind, out int chainLength)
                    {
                        int nots = int.MinValue;
                        ISubjectMarker marker1 = TryParseNotSubject(ind, false, out nots);
                        if(marker1 != null)
                        {
                            if(TryGetAt(ind + nots + 1, out var wordx1))
                            {
                                if(GetWordType(wordx1) == WordType.And)
                                {
                                    ISubjectMarker marker2 = TryParseAndSubjectChain(ind + nots + 2, out int subChainLength);
                                    if(marker2 != null)
                                    {
                                        chainLength = nots + 1 + 1 + subChainLength;
                                        // 一堆not AEntityName And chain长度
                                        return new AndSubject(marker1, marker2);
                                    }
                                }
                                else
                                {
                                    chainLength = nots + 1;
                                    return marker1;
                                }
                            }
                            else
                            {
                                chainLength = nots + 1;
                                return marker1;
                            }
                        }
                        chainLength = 0;
                        return null;
                    }

                    IPassiveMarker TryParseAndPassiveChain(int ind, out int chainLength)
                    {
                        int nots = int.MinValue;
                        IPassiveMarker marker1 = TryParseNotPassive(ind, out nots);
                        if(marker1 != null)
                        {
                            if(TryGetAt(ind + nots + 1, out var wordx1))
                            {
                                if(GetWordType(wordx1) == WordType.And)
                                {
                                    IPassiveMarker marker2 = TryParseAndPassiveChain(ind + nots + 2, out int subChainLength);
                                    if(marker2 != null)
                                    {
                                        chainLength = nots + 1 + 1 + subChainLength;
                                        // 一堆not AEntityName And chain长度
                                        return new AndPassive(marker1, marker2);
                                    }
                                }
                                else
                                {
                                    chainLength = nots + 1;
                                    return marker1;
                                }
                            }
                            else
                            {
                                chainLength = nots + 1;
                                return marker1;
                            }
                        }
                        chainLength = 0;
                        return null;
                    }

                    //解析action以及后面的passive
                    if(TryGetAt(verbIndex, out var word1))
                    {
                        if(GetWordType(word1) == WordType.Action)
                        {
                            IPassiveMarker passiveMarker = TryParseAndPassiveChain(verbIndex + 1, out int passiveLength);
                            if(passiveMarker is not null)
                            {
                                Rule newRule;
                                rules.Add(newRule = new Rule(subject, word1, passiveMarker));
                                List<LocatedWord> wordForRule = new();
                                for(int i = -nots; i <= verbIndex + passiveLength; i++)
                                {
                                    if(TryGetAt(i, out var w))
                                        wordForRule.Add(w);
                                }
                                wordsOfRule.Add(newRule, wordForRule);
                            }
                        }
                    }
                }
            }
        }
        CheckRules();
    }

    private void CheckRules()
    {
        Dictionary<Rule, ParseResult> ruleStateMarker = new();
        // 简化所有not语句
        foreach(var rule in rules)
        {
            ruleStateMarker[rule] = ParseResult.Right;
            rule.Subject = RootNotOf(rule.Subject);
            rule.Passive = RootNotOf(rule.Passive);

            if(rule.Subject is SingleNameSubject s1)
            {
                Rule someRule = null;
                if(rule.Passive is AdjPassive pa1)
                {
                    someRule = rules.FirstOrDefault(
                       r => r.Subject is SingleNameSubject s2 && s2.EntityName == s1.EntityName &&
                           r.Passive is NotPassive p2n && p2n.WarppedPassive is AdjPassive p2 && pa1.AdjName == p2.AdjName
                       );
                }
                else if(rule.Passive is BeEntityPassive pe1)
                {
                    someRule = rules.FirstOrDefault(
                       r => r.Subject is SingleNameSubject s2 && s2.EntityName == s1.EntityName &&
                           r.Passive is NotPassive p2n && p2n.WarppedPassive is BeEntityPassive p2 && pe1.EntityName == p2.EntityName
                       );
                }
                if(someRule is not null)
                {
                    ruleStateMarker[rule] = ParseResult.Reject;
                }
            }
        }
        Dictionary<LocatedWord, ParseResult> locatedWordParseResults = new();
        foreach(var word in lastWords) locatedWordParseResults[word] = ParseResult.Invalid;
        foreach(var rule in rules)
        {
            wordsOfRule[rule].ForEach(w => locatedWordParseResults[w] = ruleStateMarker[rule]);
        }
        foreach(var keyValuePair in locatedWordParseResults)
        {
            keyValuePair.Key.OnParsed?.Invoke(keyValuePair.Value);
        }
    }
}

public static class Extensions
{
    public static bool TryGetWord(this Dictionary<Point, LocatedWord> dic, int x, int y, out LocatedWord word)
        => dic.TryGetValue(new Point(x, y), out word);
}

public enum WordType
{
    Invalid,
    EntityName,
    Adv,
    Action,
    Feature,
    Not,
    And
}

[DebuggerDisplay("{X,nq}, {Y,nq}")]
public struct Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}

[DebuggerDisplay("{Content,nq} ({X,nq}, {Y,nq})")]
public struct LocatedWord
{
    public int X { get; set; }

    public int Y { get; set; }

    public string Content { get; set; }

    public Action<ParseResult> OnParsed { get; set; }

    public LocatedWord(int x, int y, string content, Action<ParseResult> onParsed)
    {
        this.X = x;
        this.Y = y;
        this.Content = content;
        this.OnParsed = onParsed;
    }

    public LocatedWord(int x, int y, string content)
    {
        this.X = x;
        this.Y = y;
        this.Content = content;
        this.OnParsed = null;
    }

    public static implicit operator string(LocatedWord word) => word.Content;
}

public enum ParseResult
{
    Invalid,
    Right,
    Reject
}