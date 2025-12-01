using UnityEngine;
using System.Collections.Generic;
public class City : Settlement
{
    public override BuildingType Type => BuildingType.City;

    public override Dictionary<ResourceType, int> Cost => new Dictionary<ResourceType, int>()
    {
        { ResourceType.Wheat, 2 },
        { ResourceType.Brick, 3 }
    };

    public override int GetProductionAmount()
    {
        return 2;   // City produce gấp đôi
    }

    public override bool CanPlace(Building build, bool gialap = false)
    {
        int numHouse = owner.PayCost(Cost, Type, gialap);
        if(numHouse != -1)
        {
            if(build.Type == BuildingType.Settlement) return true;
        }
        return false;
    }

    public override string PrintInfo()
    {
        return $"| [City] owner:{owner.playerIndex}";
    }
}
