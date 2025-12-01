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

    public void PlaceBuilding(BuildingType type, PlayerNetwork owner)
    {   
        Building build = CanBuild(type, owner);
        if(build == null) return;
        build.GetComponent<SpriteRenderer>().color = owner.color;
        building = build;  
        if(owner.numHouse == 2 && type == BuildingType.Settlement)
        {   
            string ret = "Add setup resource: ";
            foreach (var prod in adjacentProduct)
            {
                ret += $"{prod.information.resource} - ";
                this.building.GiveResource(prod);
            }
            Debug.Log(ret);
        }
        NetworkServer.Spawn(building.gameObject);  
    }
    public Building CanBuild(BuildingType type, PlayerNetwork owner, bool gialap=false)
    {
        if(!CheckValid(owner)) return null;
        if(TurnManager.instance.is_Setup && !TurnManager.instance.reverseOrder && owner.numHouse >= 1) return null;
        else if(TurnManager.instance.is_Setup && TurnManager.instance.reverseOrder && owner.numHouse >= 2) return null;
        Building build = null;
        if (type == BuildingType.Settlement)
        {   
            build = Object.Instantiate(CatanMap.instance.settlementPrefab, position + new Vector3(-0.545f, 0.52f, -1f), Quaternion.identity).GetComponent<Settlement>();
            build.Initialize(owner);
            if (!build.CanPlace(building, gialap))
            {
                Object.Destroy(build.gameObject);
                return null;
            }      
        }
        else
        {
            build = Object.Instantiate(CatanMap.instance.cityPrefab, position+ new Vector3(-0.461f, 0.537f, -1f), Quaternion.identity).GetComponent<City>();  
            build.Initialize(owner);
            if (!build.CanPlace(building, gialap))
            {
                Object.Destroy(build.gameObject);
                return null;
            }
        }
        if(gialap && build != null) Object.Destroy(build.gameObject);
        return build;
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

    private bool CheckValid(PlayerNetwork owner)
    {   
        if(TurnManager.instance.is_Setup) return true;
        foreach (var edge in connectedEdges)
        {
            if(edge.building != null)
            {
                if(edge.building.owner == owner)
                {   
                    if(CheckVertex(edge.v1, owner) && CheckVertex(edge.v2, owner))
                    return true;
                }
            }
        }
        Debug.Log("Xây nhà cách ít nhất 2 road và phải gần với công trình!!!"); 
        return false;
    }

    private bool CheckVertex(Vertex v, PlayerNetwork owner)
    {
        if(v != this)
        {
            if(v.building == null || v.building.owner != owner)
            {   
                return true;
            }
        }
        Debug.Log("Xây nhà cách ít nhất 2 road!!!");
        return false;
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
    public void PlaceBuilding(BuildingType type, PlayerNetwork owner)
    {   
        Road road = CanBuild(type, owner);
        if(road == null) return;
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
    }

    public Road CanBuild(BuildingType type, PlayerNetwork owner, bool gialap=false)
    {   
        if(!CheckValid(owner)) return null;
        if(TurnManager.instance.is_Setup && !TurnManager.instance.reverseOrder && owner.numRoad >= 1) return null;
        else if(TurnManager.instance.is_Setup && TurnManager.instance.reverseOrder && owner.numRoad >= 2) return null;
        Road road = null;
        if (type == BuildingType.Road)
        {
            road = Object.Instantiate(CatanMap.instance.roadPrefab, midPosition + new Vector3(-0.5f, 0.55f, 0), Quaternion.identity).GetComponent<Road>();
            road.Initialize(owner);
            if (!road.CanPlace(building, gialap))
            {
                Object.Destroy(road.gameObject);
                return null;
            }
        }
        else return null;
        if(gialap && road != null) Object.Destroy(road.gameObject);
        return road;
    }
    public string PrintInfo()
    {   
        string ret = $"EDGE: {midPosition} ";
        ret += !building ? "| [ROAD]: None": building.PrintInfo();
        return ret;
    }

    private bool CheckValid(PlayerNetwork owner)
    {   
        if(v1.building!= null && v1.building.owner == owner) return true;
        if(v2.building!= null && v2.building.owner == owner) return true;
        foreach (var edge in v1.connectedEdges)
        {
            if(edge.building != null)
            {
                if(edge.building.owner == owner)
                {
                    return true;
                }
            }
        }
        foreach (var edge in v2.connectedEdges)
        {
            if(edge.building != null)
            {
                if(edge.building.owner == owner)
                {
                    return true;
                }
            }
        }
        Debug.Log("Chỉ được xây road ở gần công trình của mình!!!"); 
        return false;
    }
}
