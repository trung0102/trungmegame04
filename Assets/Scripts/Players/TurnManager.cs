using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager instance { get; private set; }
    [SyncVar(hook = nameof(OnTurnChanged))] public int currentPlayerIndex = 0;
    public List<PlayerNetwork> players = new List<PlayerNetwork>();
    public bool is_Setup = true;
    private int setupTurnCount = 0;
    public bool reverseOrder = false;

    private readonly Color[] playerColors = new Color[]{ Color.red, Color.white, Color.yellow, Color.black};
    private void Awake()
    {
        if(TurnManager.instance != null) Debug.LogError("On 1 TurnManager");
        TurnManager.instance = this;
    }
    
    [Server]
    public void RegisterPlayer(PlayerNetwork p)
    {
        if (!players.Contains(p))
        {
            players.Add(p);
            p.playerIndex = players.Count - 1;
            p.color = playerColors[p.playerIndex];
        }
    }

    [Server]
    public void UnregisterPlayer(PlayerNetwork p)
    {
        if (players.Contains(p))
        {
            players.Remove(p);
            ReassignIndexes();
        }
    }

    [Server]
    void ReassignIndexes()
    {
        for (int i = 0; i < players.Count; i++)
            players[i].playerIndex = i;
        currentPlayerIndex %= Mathf.Max(players.Count, 1);
    }

    [Server]
    public void AdvanceTurn()
    {
        if (players.Count == 0) return;

        if (is_Setup)
        {   
            setupTurnCount++;

            if (setupTurnCount == players.Count)
            {
                reverseOrder = true;
                currentPlayerIndex++;
            }

            // --- Setup phase ---
            if (!reverseOrder)
                currentPlayerIndex++;
            else
                currentPlayerIndex--;

            
            if (setupTurnCount >= players.Count * 2)
            {
                is_Setup = false;
                currentPlayerIndex = 0;
                DiceController.instance.SetCanRoll();
            }
        }
        else
        {
            // --- Normal turn phase ---
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            DiceController.instance.SetCanRoll();
            
        }
        if (players[currentPlayerIndex].isBot)
        {   
            Debug.Log($"BOT action");
            StartCoroutine("BotTakeTurn");
        }
    }

    [Server]
    private System.Collections.IEnumerator BotTakeTurn()
    {   
        DiceController.instance.ServerRollDice();
        yield return new WaitForSeconds(1.2f);        
        AIManager.instance.TakeTurn(players[currentPlayerIndex]);
        yield return new WaitForSeconds(1.2f);
        AdvanceTurn();
    }

    public PlayerNetwork getPlayer()
    {
        return players[currentPlayerIndex];
    }

    [Server]
    public void PauseGame()
    {
        
    }
    void OnTurnChanged(int oldVal, int newVal)
    {   
        Debug.Log($"[Server] Turn changed: Player {newVal}");
    }
}
