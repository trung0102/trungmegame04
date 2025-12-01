using UnityEngine;
using System.Collections.Generic;
public class Settlement : Building
{
    public override BuildingType Type => BuildingType.Settlement;

    public override Dictionary<ResourceType, int> Cost => new Dictionary<ResourceType, int>()
    {
        { ResourceType.Wood, 1 },
        { ResourceType.Ore, 1 },
        { ResourceType.Wheat, 1 },
        { ResourceType.Sheep, 1 }
    };

    public override bool CanPlace(Building build, bool gialap = false)
    {
        int numHouse = owner.PayCost(Cost, Type, gialap);
        if(numHouse != -1)
        {
            if(build == null) return true;
        }
        return false;
    }

    public override void GiveResource(ResProduction res)
    {   
        if (owner == null) return;
        var resource = res.GenerateResources();
        int amount = resource.num * GetProductionAmount();
        if (isServer || TurnManager.instance.is_Setup)
        {   
            Debug.Log($"[Server] Give Resource to Player {owner.playerIndex}: {resource.name} -- {amount}");
            owner.AddResource(resource.name, amount);
        }
    }
    public virtual int GetProductionAmount()
    {
        return 1;
    }

    public override string PrintInfo()
    {
        return $"| [Settlement] owner:{owner.playerIndex}";
    }
}
