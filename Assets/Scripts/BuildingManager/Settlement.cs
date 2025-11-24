using UnityEngine;
using System.Collections.Generic;
public class Settlement : Building
{
    public override BuildingType Type => BuildingType.Settlement;

    public override Dictionary<ResourceType, int> Cost => new Dictionary<ResourceType, int>()
    {
        { ResourceType.Wood, 1 },
        { ResourceType.Brick, 1 },
        { ResourceType.Wheat, 1 },
        { ResourceType.Sheep, 1 }
    };

    public override bool CanPlace(BuildingType type)
    {
        if(type == BuildingType.City) return true;
        Debug.Log($"Chi dat duoc city");
        return false;
    }

    public virtual int GetProductionAmount()
    {
        return 1;
    }

    public override string PrintInfo()
    {
        return "Settlementttttt";
    }
}
