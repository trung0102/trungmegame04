using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class HousePlacement : MonoBehaviour
{
    public Tilemap tileMap;           // Gắn tilemap hex
    public GameObject housePrefab;       // Prefab nhà
    public GameObject circlePrefab; 
    public float vertexClickRadius = 0.1f;

    // Offset 6 đỉnh của hex (pointy top)
    private readonly Vector3[] hexVertexOffsets = new Vector3[]
    {
        new Vector3(0f, 0.97f, 0f),    // Top
        new Vector3(0.8342f, 0.485f, 0f),
        new Vector3(0.8342f, -0.485f, 0f),
        new Vector3(0f, -0.97f, 0f),
        new Vector3(-0.8342f, -0.485f, 0f),
        new Vector3(-0.8342f, 0.485f, 0f),
    };

    private void Update()
    {

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceHouse();
        }
    }

    void PlaceHouse()
    {
        // Lấy vị trí chuột
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0;

        // Gần tile nào nhất?
        Vector3Int cellPos = tileMap.WorldToCell(mouseWorldPos);

        // Nếu không có tile → không đặt
        if (tileMap.GetTile(cellPos) == null)
        {
            // Debug.Log($"Vị trí không phù hợp");
            return;
        }
            

        // Lấy vị trí world center của tile
        Vector3 tileWorld = tileMap.GetCellCenterWorld(cellPos);
        tileWorld += new Vector3(0.5f, -0.5f, 0);
        mouseWorldPos += new Vector3(0.5f, -0.5f, 0);

        // Tìm đỉnh gần nhất
        Vector3 chosenVertex = FindClosestVertex(tileWorld, mouseWorldPos);

        // Đặt prefab nhà
        if (chosenVertex == tileWorld)
        {
            Debug.Log($"Vị trí không phù hợp: {chosenVertex}");
            return;
        }
        Instantiate(housePrefab, chosenVertex + new Vector3(0, 1f, 0), Quaternion.identity);
    }

    Vector3 FindClosestVertex(Vector3 tileCenter, Vector3 clickPos)
    {
        float bestDistance = float.MaxValue;
        Vector3 bestVertex = tileCenter;

        foreach (var offset in hexVertexOffsets)
        {
            Vector3 v = tileCenter + offset;
            Debug.Log($"ClickPos: {clickPos}");
            // Debug.Log($"Vị trí có thể đặt nhà: {v}");
            // Instantiate(circlePrefab, v, Quaternion.identity);
            float d = Vector3.Distance(v, clickPos);

            if (d < bestDistance)
            {
                bestDistance = d;
                bestVertex = v;
            }
        }
        Instantiate(circlePrefab, bestVertex, Quaternion.identity);
        Debug.Log($"bestDistance: {bestDistance}");
        return bestDistance < vertexClickRadius? bestVertex: tileCenter;
    }
}
