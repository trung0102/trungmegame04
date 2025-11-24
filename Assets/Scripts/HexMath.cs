using UnityEngine;
using System.Collections.Generic;

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
            result.Add(v);
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
}

public class Vertex
{   
    public Vector3 position;
    public Building building;
    public List<ResProduction> adjacentProduct;
    public Vertex(Vector3 pos)
    {
        position = pos;
        building = null;
        adjacentProduct = new List<ResProduction>();
    }

    public string PrintInfo()
    {   
        string ret = "\n====================================================\n";
        ret += $"VERTEX: Position: {position} \n";
        ret += !building? "BUILDING: None \n": building.PrintInfo();
        foreach (var item in adjacentProduct)
        {
            ret += item.PrintInfo() + "\n";
        }
        ret += "====================================================\n";
        return ret;
    }

    public void PlaceBuilding(BuildingType type, Player owner)
    {   
        Building build = null;
        if (!building)
        {
            if (type == BuildingType.Settlement)
            {
                build = Object.Instantiate(CatanMap.instance.settlementPrefab, position+ new Vector3(0, 1f, 0), Quaternion.identity).GetComponent<Settlement>();
            }
            else
            {
               Debug.Log("Chi được xay settlement or road"); 
               return;
            }
        }
        else if (building.CanPlace(type))
        {
            
            if (type == BuildingType.City)
            {
                Object.Destroy(building.gameObject);
                build = Object.Instantiate(CatanMap.instance.cityPrefab, position, Quaternion.identity).GetComponent<City>();
            }    
        }
        else return;
        build.Initialize(owner);
        build.GetComponent<SpriteRenderer>().color = owner.color;
        building = build;    
    }
}
