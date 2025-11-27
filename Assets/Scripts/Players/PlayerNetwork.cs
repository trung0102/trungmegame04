using UnityEngine;
using System.Collections.Generic;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{   
    public static PlayerNetwork Local;
    public readonly SyncList<Resource> resources = new SyncList<Resource>();

    [SyncVar(hook = nameof(OnColorChanged))] public Color color = Color.red;
    [SyncVar(hook = nameof(OnIndexChanged))] public int playerIndex = -1;

    private BuildingType selectedBuilding = BuildingType.None;
    private void Update()
    {   
        if (!isLocalPlayer) return;
        if(!isMyTurn()) return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {   
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0;
            UpdateDiceController(mousePos);
            if (selectedBuilding != BuildingType.None)
            {
                CmdBuilding(mousePos, selectedBuilding);
            } 
        }
        
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        TurnManager.instance.RegisterPlayer(this);
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        if (TurnManager.instance != null)
            TurnManager.instance.UnregisterPlayer(this);
    }
    public override void OnStartLocalPlayer()
    {
        Local = this;
        GameButtonManager.instance.SetLocalPlayer(this);
        ButtonManager.instance.SetLocalPlayer(this);
        Debug.Log($"[Local Player {playerIndex}] Connected");
    }

    private bool isMyTurn()
    {   
        if (TurnManager.instance == null)
        return false;
        return playerIndex == TurnManager.instance.currentPlayerIndex;
    }

    [Server]
    public void AddResource(ResourceType resourcename, int number = 1)
    {
        Resource res = GetResByName(resourcename);
        res.number += number;
        Debug.Log($"[Server] Player {playerIndex} Add: {res.resourceName} --- {number}");
    }
    [Server]
    public Resource GetResByName(ResourceType resourcename)
    {
        Resource res = resources.Find((x) => x.resourceName == resourcename);
        if (res == null)
        {
            res = new Resource();
            res.resourceName = resourcename;
            res.number = 0;
            resources.Add(res);
        }
        return res;
    }

    [Command]
    public void CmdAddResource(ResourceType resourcename, int number)
    {
        AddResource(resourcename, number);
    }

//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
/////DICE/////
    [Command]
    public void CmdRollDice()
    {
        DiceController.instance.ServerRollDice();
        GameButtonManager.instance.TargetActiveButtons(connectionToClient);
    }
    private void UpdateDiceController(Vector3 mousePos)
    {
        // Kiểm tra có click trúng GameObject này không
        Collider2D col = DiceController.instance.GetComponent<Collider2D>();
        if (col != null && col.OverlapPoint(mousePos))
        {
            CmdRollDice();
        }
        
    }

//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
/////GAME BUTTON/////
    [Command]
    public void CmdEndTurn()
    {   
        if(!isMyTurn()) return;
        OnClickNone();
        TurnManager.instance.AdvanceTurn();
        GameButtonManager.instance.RpcUpdateUIAfterEndTurn();
    }
    [Command]
    public void CmdPause()
    {
        Debug.Log($"[Server] Player {playerIndex} PAUSEEEE");
        // GameButtonManager.instance.RpcUpdateUIAfterPause();
    }

    public void OnClickBuildSettlement()
    {   
        if(!isMyTurn()) return;
        selectedBuilding = BuildingType.Settlement;
    }
    public void OnClickBuildCity()
    {   
        if(!isMyTurn()) return;
        selectedBuilding = BuildingType.City;
    }
    public void OnClickBuildRoad()
    {   
        if(!isMyTurn()) return;
        selectedBuilding = BuildingType.Road;
    }
    public void OnClickNone()
    {   
        if(!isMyTurn()) return;
        selectedBuilding = BuildingType.None;
    }
    [Command]
    public void CmdBuilding(Vector3 mouseWorldPos, BuildingType buildType)
    {   
        Debug.Log($"Player {playerIndex} Building.....");
        CatanMap.instance.OnClickBuilding(buildType);
        CatanMap.instance.Build(this, mouseWorldPos);
    }


    void OnColorChanged(Color oldValue, Color newValue)
    {
        Debug.Log($"Player {newValue}");
    }
    void OnIndexChanged(int oldValue, int newValue)
    {
        Debug.Log($"Player index: {newValue}");
    }
}
