using UnityEngine;
using System.Collections.Generic;
public class City : Settlement
{
    public override BuildingType Type => BuildingType.City;

    public override Dictionary<ResourceType, int> Cost => new Dictionary<ResourceType, int>()
    {
        { ResourceType.Wheat, 2 },
        { ResourceType.Ore, 3 }
    };

    public override int GetProductionAmount()
    {
        return 2;   // City produce gấp đôi
    }

    public override bool CanPlace(BuildingType type)
    {
        Debug.Log($"Khong the dat gi them");
        return false;
    }

    public override string PrintInfo()
    {
        return "Cityyyyy";
    }
}
