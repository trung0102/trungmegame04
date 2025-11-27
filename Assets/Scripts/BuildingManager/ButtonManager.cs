using UnityEngine;

using UnityEngine.UI;
using Mirror;

public class ButtonManager : NetworkBehaviour
{
    [Header("Buttons")]
    public GameObject buildSettlement; // Build Settlement
    public GameObject buildCity; // Build City
    public GameObject buildRoad; // Build Road

    private bool isVisible = false;
    public static ButtonManager instance;  
    private PlayerNetwork localPlayer;
    protected void Awake()
    {   
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        if(ButtonManager.instance != null) Debug.LogError("Only 1 ButtonManager allowed!");
        ButtonManager.instance = this;
        
        buildSettlement.SetActive(false);
        buildCity.SetActive(false);
        buildRoad.SetActive(false);
    }

    void Start()
    {
        // Ẩn các nút con lúc đầu
        buildSettlement.SetActive(false);
        buildCity.SetActive(false);
        buildRoad.SetActive(false);
    }
    public void SetLocalPlayer(PlayerNetwork player)
    {
        localPlayer = player;
    }
    public void ToggleButtons()
    {
        isVisible = !isVisible;

        buildSettlement.SetActive(isVisible);
        buildCity.SetActive(isVisible);
        buildRoad.SetActive(isVisible);
        if (!isVisible)
        {
            localPlayer.OnClickNone();
        }
    }

    public void OnBuildSettlement()
    {
        Debug.Log($"Player {localPlayer.playerIndex} -- Build Settlement clicked!");
        localPlayer.OnClickBuildSettlement();
        // HideButtons();
    }

    public void OnBuildCity()
    {
        Debug.Log($"Player {localPlayer.playerIndex} -- Build City clicked!");
        localPlayer.OnClickBuildCity();
        // HideButtons();
    }

    public void OnBuildRoad()
    {
        Debug.Log($"Player {localPlayer.playerIndex} -- Build Road clicked!");
        localPlayer.OnClickBuildRoad();
        // HideButtons();
    }

    public void HideButtons()
    {
        isVisible = false;
        buildSettlement.SetActive(false);
        buildCity.SetActive(false);
        buildRoad.SetActive(false);
    }
}
