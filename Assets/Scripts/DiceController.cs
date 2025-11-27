using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Mirror;

public class DiceController : NetworkBehaviour
{
    public Dice dice1;     
    public Dice dice2;
    public static DiceController instance; 
    private bool canRoll = false;
    private bool isRolling = false;
    [SyncVar(hook = nameof(OnDiceResultChanged))] public int finalValue = -1;
    // private Game game;

    protected void Awake()
    {
        if(DiceController.instance != null) Debug.LogError("On 1 Dice Controller");
        DiceController.instance = this;
    }
    private void Start()
    {
        // game = Game.instance;
    }

    [Server]
    public void ServerRollDice()
    {
        if(!canRoll || isRolling) return;
        StartCoroutine("RollBothDice");
    }
    [Server]
    private System.Collections.IEnumerator RollBothDice()
    {
        isRolling = true;
        dice1.ServerRollDice();
        dice2.ServerRollDice();
        yield return new WaitForSeconds(1.2f);
        finalValue = dice1.finalValue + dice2.finalValue;
        isRolling = false;
        canRoll = false;
        CatanMap.instance.CollectResources(finalValue);
    }

    [Server]
    public void SetCanRoll()
    {
        canRoll = true;
    }

    void OnDiceResultChanged(int oldValue, int newValue)
    {
        Debug.Log($"[Server] Tổng 2 dice: {newValue}");
    }
}
