using UnityEngine;
using System.Collections.Generic;
using Mirror;
public class HexMath
{
    private static Vector3[] hexVertexOffsets = new Vector3[]
    {
        new Vector3(0f, 0.975f, 0f),    // Top
        new Vector3(0.8442f, 0.485f, 0f),
        new Vector3(0.8442f, -0.485f, 0f),
        new Vector3(0f, -0.975f, 0f),
        new Vector3(-0.8442f, -0.485f, 0f),
        new Vector3(-0.8442f, 0.485f, 0f),
    };

    public static List<Vector3> GetVertices(Vector3 center)
    {
        center += new Vector3(0.5f, -0.5f, 0);
        List<Vector3> result = new List<Vector3>();

       foreach (var offset in hexVertexOffsets)
        {
            Vector3 v = center + offset;
            result.Add(RoundVector3(v));
        }

        return result;
    }
    public static Vector3 RoundVector3(Vector3 v, float precision = 0.025f)
    {
        return new Vector3(
            Mathf.Round(v.x / precision) * precision,
            Mathf.Round(v.y / precision) * precision,
            Mathf.Round(v.z / precision) * precision
        );
    }
    public static Vector3 MidPosition(Vertex v1, Vertex v2)
    {
        return (v1.position + v2.position)/2f;
    }
}

public class Vertex
{   
    public Vector3 position;
    public Building building;
    public List<ResProduction> adjacentProduct;
    public List<Edge> connectedEdges;
    public Vertex(Vector3 pos)
    {
        position = pos;
        building = null;
        adjacentProduct = new List<ResProduction>();
        connectedEdges = new List<Edge>();
    }

    public string PrintInfo()
    {   
        string ret = "\n====================================================\n";
        ret += $"VERTEX: Position: {position} ";
        ret += !building? "BUILDING: None \n": building.PrintInfo();
        foreach (var item in adjacentProduct)
        {
            ret += item.PrintInfo() + "\n";
        }
        ret += "====================================================\n";
        foreach (var item in connectedEdges)
        {
            ret += item.PrintInfo() + "\n";
        }
        return ret;
    }

    public GameObject PlaceBuilding(BuildingType type, PlayerNetwork owner)
    {   
        Building build = null;
        if (!building)
        {
            if (type == BuildingType.Settlement)
            {
                build = Object.Instantiate(CatanMap.instance.settlementPrefab, position + new Vector3(-0.545f, 0.52f, -1f), Quaternion.identity).GetComponent<Settlement>();
            }
            else
            {
               Debug.Log("Chi được xay settlement"); 
               return null;
            }
        }
        else if (building.CanPlace(type))
        {
            
            if (type == BuildingType.City)
            {
                Object.Destroy(building.gameObject);
                build = Object.Instantiate(CatanMap.instance.cityPrefab, position+ new Vector3(-0.461f, 0.537f, -1f), Quaternion.identity).GetComponent<City>();
            }    
        }
        else return null;
        build.Initialize(owner);
        build.GetComponent<SpriteRenderer>().color = owner.color;
        building = build;  
        NetworkServer.Spawn(building.gameObject);  
        return build.gameObject;
    }

    public void CollectResources(int diceNumber)
    {   
        if(!building) return;
        foreach (var prod in adjacentProduct)
        {
            if(prod.information.numberToken == diceNumber)
            {
                this.building.GiveResource(prod);
            }
        }
    }

    public void AddEdge(Edge e)
    {
        if (!connectedEdges.Contains(e))
        {
            connectedEdges.Add(e);
        }
        return;
    }
}

public class Edge
{
    public Vector3 midPosition;
    public Road building = null;
    public Vertex v1, v2;

    public Edge(Vertex v1, Vertex v2)
    {
        midPosition = (v1.position + v2.position)/2f;
        this.v1 = v1;
        this.v2 = v2;
    }
    public GameObject PlaceBuilding(BuildingType type, PlayerNetwork owner)
    {   
        Road road = null;
        if (!building)
        {
            if (type == BuildingType.Road)
            {
                road = Object.Instantiate(CatanMap.instance.roadPrefab, midPosition + new Vector3(-0.5f, 0.55f, 0), Quaternion.identity).GetComponent<Road>();
            }
            else
            {
               Debug.Log("Chi được xay road"); 
               return null;
            }
        }
        else if (building.CanPlace(type))
        {
            return null;
        }
        else return null;
        road.Initialize(owner);
        road.GetComponent<SpriteRenderer>().color = owner.color;
        if(v1.position.x > v2.position.x && v1.position.y < v2.position.y)
        {
            road.transform.localScale = new Vector3(1.35f, 1.35f, 1f);  
        }
        else if(v1.position.x > v2.position.x && v1.position.y > v2.position.y)
        {
            road.transform.localScale = new Vector3(-1.35f, 1.35f, 1f);  
        }
        else if(v1.position.x == v2.position.x)
        {
            road.transform.localScale = new Vector3(1.35f, 1.35f, 1f);  
            road.transform.rotation = Quaternion.Euler(0, 0, 120f); 
        }
        building = road;
        NetworkServer.Spawn(building.gameObject);  
        return road.gameObject;
    }
    public string PrintInfo()
    {   
        string ret = $"EDGE: {midPosition} ";
        ret += !building ? "| [ROAD]: None": building.PrintInfo();
        return ret;
    }
}
