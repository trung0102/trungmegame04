using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.Splines;

public class HandManager : MonoBehaviour
{
    [SerializeField] private int maxHandsize;
    [SerializeField] private GameObject[] cardPrefabs;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;

    private List<GameObject> handCards = new List<GameObject>();
    private List<ResourceType> typeCards = new List<ResourceType>();

    public static HandManager instance;

    protected void Awake()
    {
        if(HandManager.instance != null) Debug.LogError("On 1 HandManager");
        HandManager.instance = this;
    }
    public void UpdateHandUI(IReadOnlyList<int> dataHand)
    {   
        for (int i = handCards.Count - 1; i >= 0; i--)
        {
            if (handCards[i] != null)
            {
                DOTween.Kill(handCards[i].transform);
                Destroy(handCards[i]);
            }
        }
        handCards.Clear();
        
        foreach (int cardID in dataHand)
        {
            GameObject prefab = cardPrefabs[cardID];
            float p_z = -1f - cardID*0.1f;
            GameObject g = Instantiate(prefab, spawnPoint.position + new Vector3(0f, 0f, p_z), spawnPoint.rotation);

            g.transform.localScale = new Vector3(1f, -1f, 1f);
            handCards.Add(g);
        }

        UpdateCardPosition();
        Debug.Log($"In Hand: {handCards.Count}");
    }

    private void UpdateCardPosition()
    {
        if(handCards.Count == 0) return;
        float cardSpacing = 1f/maxHandsize;
        float firstCardPosition = 0.5f - (handCards.Count -1) * cardSpacing/2;
        Spline spline = splineContainer.Spline;
        for (int i = 0; i < handCards.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up,forward).normalized);
            handCards[i].transform.DOMove(splinePosition, 0.25f);
            handCards[i].transform.DOLocalRotateQuaternion(rotation,0.25f);
        }
    }
}
