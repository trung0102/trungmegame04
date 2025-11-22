using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CatanMap : MonoBehaviour
{
    public Tilemap tileMap;

    public Tilemap numMap;
    
    public HexTileSet tileSet;
    [SerializeField] private bool changeTileAtPosition = false;

    protected List<ResProduction> productions = new List<ResProduction>();

    [ContextMenu("Toggle Change Tile")]
    private void ToggleChangeTile()
    {
        changeTileAtPosition = !changeTileAtPosition;
    }
    
    void Start()
    {
        foreach (var kvp in TileDatabase.tileDict)
        {
            Vector3Int position = kvp.Key;
            ResProductionInfo info = kvp.Value;

            ResProduction res = gameObject.AddComponent<ResProduction>();
            res.Initialize(position, info);
            productions.Add(res);
        }
    }

    void Update(){
        if(Mouse.current.leftButton.wasPressedThisFrame){
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0;


            Vector3Int cellPos = tileMap.WorldToCell(mouseWorldPos); // Lấy ô hex
            TileBase clickedTile = tileMap.GetTile(cellPos); // Lấy Tile hiện tại

            // if (clickedTile != null)
            // {   
            //     // string info = "Khong co gi ca";
            //     foreach (var product in productions)
            //     {
            //         if(product.position == cellPos)
            //         {
            //             Debug.Log($"Clicked Hex: {cellPos}, Information: {product.PrintInfo()}");
            //         }
            //     }
            //     // TODO: Hiển thị UI hoặc info khác
            // }        
            if(changeTileAtPosition) ChangeTileAtPosition(cellPos, ResourceType.Wood);    
        }
    }

    public void ChangeTileAtPosition(Vector3Int position, ResourceType resource)
    {   
        foreach (var production in productions)
        {
            if(production.position == position)
            {
                production.ChangeTile(tileMap, tileSet, resource);
                return;
            }
        }
        // if (!tilemap.GetTile(position))
        // {
        Debug.Log($"Vị trí hiện tại không có tile");
        return;
        
    }

    public List<ResourceType> GetResourcesByDiceNumber(int diceNumber)
    {   
        List<ResourceType> listRes = new List<ResourceType>();
        foreach (var prod in productions)
        {
            if(prod.information.numberToken == diceNumber)
            {
                listRes.Add(prod.information.resource);
                Debug.Log($"Add: {prod.information.resource}");
            }
        }
        return listRes;
    }
}
