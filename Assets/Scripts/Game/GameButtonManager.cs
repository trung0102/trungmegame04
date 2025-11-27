using UnityEngine;

using UnityEngine.UI;

using Mirror;

public class GameButtonManager : NetworkBehaviour
{
    // Game game;
    [Header("Buttons")]
    public GameObject endturn;
    public GameObject building;
    public GameObject continuebutton;
    private bool isVisible = true;
    public static GameButtonManager instance;   
    private PlayerNetwork localPlayer;

    protected void Awake()
    {   
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        if(GameButtonManager.instance != null) Debug.LogError("Only 1 GameButtonManager allowed!");
        GameButtonManager.instance = this;
    }
    private void Start()
    {
        // game = Game.instance;
        // building.SetActive(false);
        continuebutton.SetActive(false);
    }
    public void SetLocalPlayer(PlayerNetwork player)
    {
        localPlayer = player;
    }

    public void OnPause()
    {
        if(localPlayer == null) return;
        localPlayer.CmdPause();
    }

    public void OnEndTurn()
    {   
        if(localPlayer == null) return;
        Debug.Log($"Player {localPlayer.playerIndex} -- End turn clicked!");
        localPlayer.CmdEndTurn();
    }

    [ClientRpc]
    public void RpcUpdateUIAfterEndTurn()
    {
        if(!TurnManager.instance.is_Setup) HideButtons();
    }

    [ClientRpc]
    public void RpcUpdateUIAfterPause()
    {   
        isVisible = !isVisible;
        endturn.SetActive(false);
        HideButtons();
    }
    public void HideButtons()
    {
        building.SetActive(false);
        building.GetComponent<ButtonManager>().HideButtons();
    }
    [TargetRpc]
    public void TargetActiveButtons(NetworkConnectionToClient target)
    {
        building.SetActive(true);
    }
}
