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

    public override bool CanPlace(BuildingType type)
    {
        Debug.Log($"Khong the dat gi them");
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
