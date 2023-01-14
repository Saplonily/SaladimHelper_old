using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using MonoMod.Utils;

namespace Celeste.Mod.SllCelesteHelper.Triggers;

[CustomEntity("SllCH/LimitedDashesTrigger")]
public class LimitedDashesTrigger : Trigger
{
    public OverEvent oEvent = OverEvent.Kill;
    public int usedDashes = 0;
    public int aimDashes = 42;

    public LimitedDashesTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        oEvent = data.Attr("overEvent", "kill") switch
        {
            "kill" => OverEvent.Kill,
            "noRefill" => OverEvent.NoRefill,
            _ => OverEvent.Kill
        };
        aimDashes = data.Int("dashes", 42);
    }
    public override void OnEnter(Player player)
    {
        On.Celeste.Player.DashBegin += Player_DashBegin;
        Everest.Events.Player.OnDie += Player_OnDie;
    }

    private void Player_OnDie(Player obj)
    {
        var pdata = new DynamicData(obj);
        var inv = pdata.Get<Level>("level").Session.Inventory;
        inv.NoRefills = false;
    }

    public override void OnLeave(Player player)
    {
        On.Celeste.Player.DashBegin -= Player_DashBegin;
        Everest.Events.Player.OnDie -= Player_OnDie;
        var pdata = new DynamicData(player);
        var inv = pdata.Get<Level>("level").Session.Inventory;
        inv.NoRefills = false;
    }

    private void Player_DashBegin(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        orig(self);
        OnPlayerDashed(self);
    }

    public void OnPlayerDashed(Player player)
    {
        usedDashes++;

        if (usedDashes == aimDashes)
        {
            switch (oEvent)
            {
                case OverEvent.Kill:
                    player.Die(Vector2.UnitX);
                    break;
                case OverEvent.NoRefill:
                    var pdata = new DynamicData(player);
                    var inv = pdata.Get<Level>("level").Session.Inventory;
                    inv.NoRefills = true;
                    break;
            }
        }
    }

    public enum OverEvent
    {
        Kill,
        NoRefill
    }
}