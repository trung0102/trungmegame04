using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "HexTileSet", menuName = "Catan/TileSet")]
public class HexTileSet : ScriptableObject
{
    public TileBase Tile2;
    public TileBase Tile3;
    public TileBase Tile4;
    public TileBase Tile5;
    public TileBase Tile6;
    public TileBase desertTile;
    public TileBase Tile8;
    public TileBase Tile9;
    public TileBase Tile10;
    public TileBase Tile11;
    public TileBase Tile12;

     public TileBase GetTile(int num) =>
        num switch
        {
            2 => Tile2,
            3 => Tile3,
            4 => Tile4,
            5 => Tile5,
            6 => Tile6,
            7 => desertTile,
            8 => Tile8,
            9 => Tile9,
            10 => Tile10,
            11 => Tile11,
            12 => Tile12,
            _ => desertTile
        };
}
