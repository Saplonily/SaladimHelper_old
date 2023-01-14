using MadelineIsYouLexer;

namespace ConsoleTest;

public class Program
{
    static void Main(string[] args)
    {
        //Console.WriteLine(RuleManager.RootNotOfPassive(p1));
        RuleManager ruleManager = new
        (
            new string[] { "madeline", "jelly", "falling_block", "dream_block" },
            new string[] { "on", "climb" },
            new string[] { "is", "has" },
            new string[] { "two_dashes", "moving" }
        );
        List<LocatedWord> scene = new()
        {
            /*
            new(1, -2, "not"),
            new(1, -1, "not"),
            new(1, 0, "not"),
            new(1, 1, "madeline"),
            new(1, 2, "climb"),
            new(1, 3, "not"),
            new(1, 4, "falling_block"),
            new(1, 5, "has"),
            new(1, 6, "not"),
            new(1, 7, "two_dashes"),
            new(0, 5, "not"),
            new(0, 6, "jelly"),
            new(0, 7, "is" ),
            new(0, 8, "not"),
            new(0, 9, "not"),
            new(0, 10, "not"),
            new(0, 11, "not"),
            new(0, 12, "moving"),
            new(3,0,"madeline"),
            new(3,1,"is"),
            new(3,2,"not"),
            new(3,3,"madeline"),
            new(2,1,"madeline"),
            new(4,1,"madeline"),
            new(10,0,"madeline"),
            new(10,1,"and"),
            new(10,2,"jelly"),
            new(10,3,"and"),
            new(10,4,"not"),
            new(10,5,"jelly"),
            new(10,6,"is"),
            new(10,7,"dream_block"),
            new(10,8,"and"),
            new(10,9,"not"),
            new(10,10,"madeline"),
            new(10,11,"and"),
            new(10,12,"not"),
            new(10,13,"not"),
            new(10,14,"not"),
            new(10,15,"dream_block"),
            new(12,0,"jelly"),
            new(12,1,"is"),
            new(12,2,"moving"),
            new(14,0,"madeline"),
            new(14,1,"is"),
            new(14,2,"jelly"),
            new(17,0,"madeline"),
            new(17,1,"is"),
            new(17,2,"madeline"),
            new(17,3,"and"),
            new(17,4,"jelly"),
            new(18,0,"madeline"),
            new(18,1,"is"),
            new(18,2,"noton"),
            new(18,3,"madeline"),*/
            new(20,0,"madeline"),
            new(20,1,"climb"),
            new(20,2,"madeline")
        };

        ruleManager.ParseAndUse(scene);

        //Player p = new();
        foreach(var rule in ruleManager.Rules)
        {
            Console.WriteLine($"{rule.Subject} {rule.Action} {rule.Passive}");
        }
        Console.WriteLine(ruleManager.HasAdjFeature(new Jelly(), "has", "two_dashes"));
        Console.WriteLine(ruleManager.HasBeEntityFeature(new Player(), "is", "jelly"));
        Console.WriteLine(ruleManager.HasBeEntityFeature(new Player(), "is", "madeline"));
        Console.WriteLine(ruleManager.HasBeEntityFeature(new Player(), "is", "jelly"));
    }
}

public class Player : IEntity
{
    public string Name => "madeline";

    public bool HasAdv(string adv, SingleNameSubject another)
    {
        if(adv == "on" && another.EntityName == "tiles")
        {
            return true;
        }
        return false;
    }
}

public class Jelly : IEntity
{
    public string Name => "jelly";

    public bool HasAdv(string adv, SingleNameSubject another)
    {
        return false;
    }
}