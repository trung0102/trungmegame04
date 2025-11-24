using UnityEngine;
using System.Collections.Generic;

public abstract class Building : MonoBehaviour
{
    public Player owner;
    public abstract BuildingType Type { get; }
    public abstract Dictionary<ResourceType, int> Cost { get; }

    public virtual void Initialize(Player owner)
    {
        this.owner = owner;
    }

    public abstract bool CanPlace(BuildingType type);

    public virtual string PrintInfo()
    {
        return "Buildinggggg";
    }


}
