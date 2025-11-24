using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
public class ResProduction : MonoBehaviour
{
    public Vector3Int position;
    public ResProductionInfo information;

    public void Initialize(Vector3Int position, ResProductionInfo information)
    {
        this.information = information;
        this.position = position;
    }
    public (int num, ResourceType name) GenerateResources()
    {
        return information.blocked? (0, information.resource) : (1, information.resource); 
    }
    
    public string PrintInfo()
    {
        return $"Position: {position} | Resource: {information.resource} | " +
                  $"Token: {information.numberToken} | Blocked: {information.blocked}";
    }

    public void ChangeTile(Tilemap tilemap, HexTileSet tileSet, ResourceType resource)
    {
        information.resource = resource;
        TileBase newTile = tileSet.GetTile(resource);
        tilemap.SetTile(position, newTile);
        tilemap.RefreshTile(position);
        Debug.Log($"Đã chuyển tile tại: {position}, Tile Name: {newTile.name}");
    }
}

public struct ResProductionInfo
{
    public ResourceType resource;

    public int numberToken;

    public bool blocked;

    public ResProductionInfo(ResourceType resource, int numberToken, bool blocked = false)
    {
        this.resource = resource;
        this.numberToken = numberToken;
        this.blocked = blocked;
    }
}

public static class TileDatabase
{
    public static Dictionary<Vector3Int, ResProductionInfo> tileDict = new Dictionary<Vector3Int, ResProductionInfo>()
    {
        { new Vector3Int(0, 2, 0), new ResProductionInfo(ResourceType.Brick, 10) },
        { new Vector3Int(1, 2, 0), new ResProductionInfo(ResourceType.Wood, 3) },
        { new Vector3Int(2, 2, 0), new ResProductionInfo(ResourceType.Ore, 4) },
        { new Vector3Int(-1, 1, 0), new ResProductionInfo(ResourceType.Wood, 2) },
        { new Vector3Int(0, 1, 0), new ResProductionInfo(ResourceType.Ore, 5) },
        { new Vector3Int(1, 1, 0), new ResProductionInfo(ResourceType.Wheat, 6) },
        { new Vector3Int(2, 1, 0), new ResProductionInfo(ResourceType.Sheep, 9) },
        { new Vector3Int(-1, 0, 0), new ResProductionInfo(ResourceType.Sheep, 8) },
        { new Vector3Int(0, 0, 0), new ResProductionInfo(ResourceType.Brick, 9) },
        { new Vector3Int(1, 0, 0), new ResProductionInfo(ResourceType.Sheep, 3) },
        { new Vector3Int(2, 0, 0), new ResProductionInfo(ResourceType.Desert, 0) },
        { new Vector3Int(3, 0, 0), new ResProductionInfo(ResourceType.Wood, 6) },
        { new Vector3Int(-1, -1, 0), new ResProductionInfo(ResourceType.Wheat, 12) },
        { new Vector3Int(0, -1, 0), new ResProductionInfo(ResourceType.Wood, 8) },
        { new Vector3Int(1, -1, 0), new ResProductionInfo(ResourceType.Ore, 11) },
        { new Vector3Int(2, -1, 0), new ResProductionInfo(ResourceType.Wheat, 4) },
        { new Vector3Int(0, -2, 0), new ResProductionInfo(ResourceType.Brick, 11) },
        { new Vector3Int(1, -2, 0), new ResProductionInfo(ResourceType.Wheat, 10) },
        { new Vector3Int(2, -2, 0), new ResProductionInfo(ResourceType.Sheep, 5) }
    };
}