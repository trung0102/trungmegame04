using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CatanMap : MonoBehaviour
{
    public Tilemap tileMap;

    public Tilemap numMap;
    
    public HexTileSet tileSet;

    public GameObject settlementPrefab;       // Prefab nhà
    public GameObject circlePrefab; 
    public GameObject cityPrefab;
    public GameObject roadPrefab;
    private bool changeTileAtPosition = false;
    public static CatanMap instance;

    protected List<ResProduction> productions = new List<ResProduction>();
    public Dictionary<Vector3, Vertex> vertexDict = new Dictionary<Vector3, Vertex>();

    [ContextMenu("Toggle Change Tile")]
    private void ToggleChangeTile()
    {
        changeTileAtPosition = !changeTileAtPosition;
    }
    private bool xaynha1 = false;
    private bool xaynha2 = false;

    [ContextMenu("Xay nha 1")]
    private void XayNha1()
    {
        xaynha1 = !xaynha1;
    }
    [ContextMenu("Xay nha 2")]
    private void XayNha2()
    {
        xaynha2 = !xaynha2;
    }

    protected void Awake()
    {
        if(CatanMap.instance != null) Debug.LogError("On 1 CatanMap");
        CatanMap.instance = this;
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
            GenerateBuildableVertices(res);
        }
        // int i = 1;
        // foreach (var kvp in vertexDict)
        // {
        //     Vertex v = kvp.Value;
        //     Debug.Log($"--- {i} --- {v.PrintInfo()}");
        //     i++;
        // }
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
            if(xaynha1 || xaynha2)
            {
                Vector3 chosenVertex = FindClosestVertex(mouseWorldPos);
                if (chosenVertex == Vector3.zero)
                {
                   Debug.Log($"Vị trí không phù hợp"); return;
                } 
                Vertex vertex = vertexDict[chosenVertex];
                if (xaynha1)
                {
                    vertexDict[chosenVertex].PlaceBuilding(BuildingType.Settlement, Player.instance);
                }
                else if (xaynha2)
                {
                    vertexDict[chosenVertex].PlaceBuilding(BuildingType.City, Player.instance);
                }

            }
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

    private void GenerateBuildableVertices(ResProduction res)
    {
        Vector3 centerTile = tileMap.GetCellCenterWorld(res.position);

        List<Vector3> vertices = HexMath.GetVertices(centerTile);
        foreach (var v in vertices)
        {
            Vector3 key = HexMath.RoundVector3(v);
            
            if (!vertexDict.ContainsKey(key))
            {
                vertexDict[key] = new Vertex(key);
            }
            vertexDict[key].adjacentProduct.Add(res);
        }
    }

    private Vector3 FindClosestVertex(Vector3 clickPos)
    {
        float bestDistance = float.MaxValue;
        Vector3 bestVertex = Vector3.zero;
        clickPos += new Vector3(0.5f, -0.5f, 0);

        foreach (var kvp in vertexDict)
        {
            float d = Vector3.Distance(kvp.Key, clickPos);

            if (d < bestDistance)
            {
                bestDistance = d;
                bestVertex = kvp.Key;
            }
        }
        return bestDistance < 0.3f? bestVertex: Vector3.zero;
    }
}
