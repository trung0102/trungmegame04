using UnityEngine;
using System.Collections.Generic;
public class Road : Building
{
    public override BuildingType Type => BuildingType.Road;

    public override Dictionary<ResourceType, int> Cost => new Dictionary<ResourceType, int>()
    {
        { ResourceType.Wood, 1 },
        { ResourceType.Ore, 1 }
    };

    public override bool CanPlace(Building build)
    {   
        int numHouse = owner.PayCost(Cost);
        if(numHouse != -1)
        {
            if(build == null) return true;
        }
        Debug.Log("Không đủ Cost để xây Road");
        return false;
    }

    public override void GiveResource(ResProduction res)
    {   
        return;
    }

    public override string PrintInfo()
    {
        return $"| [Road] owner:{owner.playerIndex}";
    }
}
