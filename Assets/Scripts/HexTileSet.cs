using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "HexTileSet", menuName = "Catan/TileSet")]
public class HexTileSet : ScriptableObject
{
    public TileBase woodTile;
    public TileBase brickTile;
    public TileBase wheatTile;
    public TileBase sheepTile;
    public TileBase oreTile;
    public TileBase desertTile;

     public TileBase GetTile(ResourceType type) =>
        type switch
        {
            ResourceType.Wood => woodTile,
            ResourceType.Brick => brickTile,
            ResourceType.Wheat => wheatTile,
            ResourceType.Sheep => sheepTile,
            ResourceType.Ore => oreTile,
            ResourceType.Desert => desertTile,
            _ => desertTile
        };
}
