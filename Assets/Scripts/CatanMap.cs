using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System.Collections.Generic;

using Mirror;

public class CatanMap : NetworkBehaviour
{
    public Tilemap tileMap;

    public Tilemap numMap;
    
    public HexTileSet tileSet;

    public GameObject settlementPrefab;       // Prefab nhà
    public GameObject circlePrefab; 
    public GameObject cityPrefab;
    public GameObject roadPrefab;
    private bool changeTileAtPosition = false;
    private BuildingType building;
    public static CatanMap instance;

    protected List<ResProduction> productions = new List<ResProduction>();
    public Dictionary<Vector3, Vertex> vertexDict = new Dictionary<Vector3, Vertex>();
    public Dictionary<Vector3, Edge> edgeDict = new Dictionary<Vector3, Edge>();


    [ContextMenu("Toggle Change Tile")]
    private void ToggleChangeTile()
    {
        changeTileAtPosition = !changeTileAtPosition;
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
        building = BuildingType.None;
    }

    void Update(){
        if(Mouse.current.leftButton.wasPressedThisFrame){
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0;


            Vector3Int cellPos = tileMap.WorldToCell(mouseWorldPos); // Lấy ô hex
            TileBase clickedTile = tileMap.GetTile(cellPos); // Lấy Tile hiện tại

            Vector3 chosenVertex = FindClosestEdge(mouseWorldPos, 0.4f);
            if (chosenVertex == Vector3.zero) return;
            Edge edge = edgeDict[chosenVertex];
            Debug.Log($"[Edge] --- {edge.PrintInfo()}");
            chosenVertex = FindClosestVertex(mouseWorldPos, 0.4f);
            if (chosenVertex == Vector3.zero) return;
            Vertex vertex = vertexDict[chosenVertex];
            Debug.Log($"[Vertex] --- {vertex.PrintInfo()}");
            // if (clickedTile != null)
            // { 
            //     foreach (var product in productions)
            //     {
            //         if(product.position == cellPos)
            //         {
            //             Debug.Log($"Clicked Hex: {cellPos}, Information: {product.PrintInfo()}");
            //         }
            //     }
            //     // TODO: Hiển thị UI hoặc info khác
            // }  
            // if(building != BuildingType.None)
            // {   
            //     Player player = Game.instance.CurrentPlayer;
            //     if(building == BuildingType.Road)
            //     {
            //         Vector3 chosenVertex = FindClosestEdge(mouseWorldPos, 0.4f);
            //         if (chosenVertex == Vector3.zero)
            //         {
            //         Debug.Log($"Vị trí đặt đường không phù hợp"); return;
            //         } 
            //         Edge vertex = edgeDict[chosenVertex];
            //         edgeDict[chosenVertex].PlaceBuilding(building, player);
            //     }
            //     else
            //     {
            //         Vector3 chosenVertex = FindClosestVertex(mouseWorldPos);
            //         if (chosenVertex == Vector3.zero)
            //         {
            //         Debug.Log($"Vị trí đặt nhà không phù hợp"); return;
            //         } 
            //         Vertex vertex = vertexDict[chosenVertex];
            //         vertexDict[chosenVertex].PlaceBuilding(building, player);
            //     }

            // }
            // if(changeTileAtPosition) ChangeTileAtPosition(cellPos, ResourceType.Wood);    
        }
    }

    // public void ChangeTileAtPosition(Vector3Int position, ResourceType resource)
    // {   
    //     foreach (var production in productions)
    //     {
    //         if(production.position == position)
    //         {
    //             production.ChangeTile(tileMap, tileSet, resource);
    //             return;
    //         }
    //     }
    //     // if (!tilemap.GetTile(position))
    //     // {
    //     Debug.Log($"Vị trí hiện tại không có tile");
    //     return;
        
    // }
    // sự kiện khi nhấn vào button Building
    [Server]
    public void OnClickBuilding(BuildingType build)
    {
        building = build;
    }

    [Server]
    public bool Build(PlayerNetwork player, Vector3 mouseWorldPos)
    {
        if(building != BuildingType.None)
        {
            if(building == BuildingType.Road)
            {
                Vector3 chosenEdge = FindClosestEdge(mouseWorldPos, 0.4f);
                if (chosenEdge == Vector3.zero)
                {
                    Debug.Log($"Vị trí đặt đường không phù hợp"); return false;
                } 
                Edge edge = edgeDict[chosenEdge];
                GameObject roadObj = edge.PlaceBuilding(building, player);
            }
            else
            {
                Vector3 chosenVertex = FindClosestVertex(mouseWorldPos);
                if (chosenVertex == Vector3.zero)
                {
                Debug.Log($"Vị trí đặt nhà không phù hợp"); return false;
                } 
                Vertex vertex = vertexDict[chosenVertex];
                GameObject houseObj = vertex.PlaceBuilding(building, player);
            }
            return true;
        }
        return false;
    }
    // // sự kiện khi đổ xúc xắc
    // public void GetResourcesByDiceNumber(int diceNumber)
    // {   
    //     foreach (var kvp in vertexDict)
    //     {
    //         kvp.Value.CollectResources(diceNumber);
    //     }
    // }

    private void GenerateBuildableVertices(ResProduction res)
    {
        Vector3 centerTile = tileMap.GetCellCenterWorld(res.position);

        List<Vector3> vertices = HexMath.GetVertices(centerTile);
        int length = vertices.Count;
        foreach (var v in vertices)
        {
            Vector3 key = v;
            
            if (!vertexDict.ContainsKey(key))
            {
                vertexDict[key] = new Vertex(key);
            }
            vertexDict[key].adjacentProduct.Add(res);
        }
        for(int i=0; i< length; ++i)
        {
            Vertex v1 = vertexDict[vertices[i]];
            Vertex v2 = vertexDict[vertices[(i+1)%length]];
            Vector3 key = HexMath.MidPosition(v1,v2);
            if (!edgeDict.ContainsKey(key))
            {
                edgeDict[key] = new Edge(v1,v2);
            }
            v1.AddEdge(edgeDict[key]);
            v2.AddEdge(edgeDict[key]);
        }
    }

    private Vector3 FindClosestVertex(Vector3 clickPos, float vertexClickRadius = 0.3f)
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
        return bestDistance < vertexClickRadius? bestVertex: Vector3.zero;
    }
    private Vector3 FindClosestEdge(Vector3 clickPos, float vertexClickRadius = 0.3f)
    {
        float bestDistance = float.MaxValue;
        Vector3 bestVertex = Vector3.zero;
        clickPos += new Vector3(0.5f, -0.5f, 0);
        foreach (var kvp in edgeDict)
        {
            float d = Vector3.Distance(kvp.Key, clickPos);

            if (d < bestDistance)
            {
                bestDistance = d;
                bestVertex = kvp.Key;
            }
        }
        return bestDistance < vertexClickRadius? bestVertex: Vector3.zero;
    }

    private void OnVertexChanged(SyncList<Resource>.Operation op, int index, Resource oldItem, Resource newItem)
    {
        
    }

}
