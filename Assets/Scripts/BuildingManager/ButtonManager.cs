using UnityEngine;

using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("Buttons")]
    public GameObject button1; // Build Settlement
    public GameObject button2; // Build City
    // public GameObject button3; // Trade

    private bool isVisible = false;

    void Start()
    {
        // Ẩn các nút con lúc đầu
        button1.SetActive(false);
        button2.SetActive(false);
        // button3.SetActive(false);
    }
    public void ToggleButtons()
    {
        isVisible = !isVisible;

        button1.SetActive(isVisible);
        button2.SetActive(isVisible);
        if (!isVisible)
        {
            CatanMap.instance.OnClickBuilding(BuildingType.None);
        }
        // button3.SetActive(isVisible);
    }

    public void OnBuildSettlement()
    {
        Debug.Log("Build Settlement clicked!");
        CatanMap.instance.OnClickBuilding(BuildingType.Settlement);
        // HideButtons();
    }

    public void OnBuildCity()
    {
        Debug.Log("Build City clicked!");
        CatanMap.instance.OnClickBuilding(BuildingType.City);
        // HideButtons();
    }

    public void OnTrade()
    {
        Debug.Log("Trade clicked!");
        HideButtons();
    }

    private void HideButtons()
    {
        isVisible = false;
        button1.SetActive(false);
        button2.SetActive(false);
        // button3.SetActive(false);
    }
}
