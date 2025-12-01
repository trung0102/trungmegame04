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
    public GameObject menu;
    public GameObject background;
    // public GameObject pausebutton;
    public GameObject menutext;
    public GameObject pausetext;

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
        // continuebutton.SetActive(false);
    }
    public void SetLocalPlayer(PlayerNetwork player)
    {
        localPlayer = player;
    }

    [ClientRpc]
    public void RpcHideBackground()
    {   
        if (background != null)
            background.SetActive(false);
        menutext.SetActive(false);  
    }

    [Server]
    public void StartGame()
    {
        Vector3 spawnPos = Vector3.zero; 
        int num = TurnManager.instance.players.Count;
        for(int i=num; i < 4; ++i)
        {
            BotNetwork bot = Instantiate(AIManager.instance.botPrefab, spawnPos, Quaternion.identity).GetComponent<BotNetwork>();
            NetworkServer.Spawn(bot.gameObject);
            Debug.Log($"[Server] Spawn Bot");
        }

        RpcHideBackground();
    }

    public void OnPause()
    {
        if(localPlayer == null) return;
        localPlayer.CmdPause();
        continuebutton.SetActive(true);
    }
    [ClientRpc]
    public void RpcPause()
    {
        if (background != null)
            background.SetActive(true);
        pausetext.SetActive(true);
    }
    [ClientRpc]
    public void RpcContinue()
    {
        if (background != null)
            background.SetActive(false);
        pausetext.SetActive(false);
    }
    public void OnContinue()
    {
        // isVisible = true;
        // endturn.SetActive(true);
        // building.SetActive(true);
        // localPlayer.CmdContinue();
        // continuebutton.SetActive(false);
        if(localPlayer == null) return;
        localPlayer.CmdContinue();
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
