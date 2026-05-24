using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Analytics;

public static class AnalyticsManager
{
    public static void TrackAbilityPurchase(string heroName, string abilityName, int cost, int level)
    {
        AbilityPurchaseEvent myEvent = new AbilityPurchaseEvent
        {
            HeroName = heroName,
            AbilityName = abilityName,
            Cost = cost,
            Level = level
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    public static void TrackBossBattle(string bossName, string dungeonName, int floor, int level, string result)
    {
        BossBattleEvent myEvent = new BossBattleEvent
        {
            BossName = bossName,
            DungeonName = dungeonName,
            Floor = floor,
            Level = level,
            Result = result
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
    }
}

public class AbilityPurchaseEvent : Unity.Services.Analytics.Event
{
    public AbilityPurchaseEvent() : base("abilityPurchaseEvent")
    {
    }

    public string HeroName { set { SetParameter("heroName", value); } }
    public string AbilityName { set { SetParameter("abilityName", value); } }
    public int Cost { set { SetParameter("cost", value); } }
    public int Level { set { SetParameter("level", value); } }
}

public class BossBattleEvent : Unity.Services.Analytics.Event
{
    public BossBattleEvent() : base("bossBattleEvent")
    {
    }

    public string BossName { set { SetParameter("bossName", value); } }
    public string DungeonName { set { SetParameter("dungeonName", value); } }
    public int Floor { set { SetParameter("floor", value); } }
    public int Level { set { SetParameter("level", value); } }
    public string Result { set { SetParameter("result", value); } }
}
