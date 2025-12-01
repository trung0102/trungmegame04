using UnityEngine;
using System.Collections.Generic;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{   
    public static PlayerNetwork Local;
    public readonly SyncList<Resource> resources = new SyncList<Resource>();
    public List<Resource> currentResources = new List<Resource>();
    public int numHouse = 0;
    public int numRoad = 0;

    
    protected bool pause = false;
    public bool isBot = false;

    [SyncVar(hook = nameof(OnColorChanged))] public Color color = Color.red;
    [SyncVar(hook = nameof(OnIndexChanged))] public int playerIndex = -1;
    protected int index = 1;

    protected BuildingType selectedBuilding = BuildingType.None;
    protected virtual void Awake()
    {
        resources.Callback += OnHandUpdated;
    }
    protected virtual void Update()
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
        if(isServer && isLocalPlayer)
        {   
            GameButtonManager.instance.menu.SetActive(true);
        }
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if(GameButtonManager.instance != null)
            GameButtonManager.instance.menutext.SetActive(true);
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

    protected bool isMyTurn()
    {   
        if (TurnManager.instance == null)
        return false;
        return playerIndex == TurnManager.instance.currentPlayerIndex;
    }

    [Server]
    public void AddResource(ResourceType resourcename, int number = 1)
    {
        Resource res = GetResByName(resourcename);
        resources.Remove(res);
        res.number += number;
        resources.Add(res);
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

    [Server]
    public int PayCost(Dictionary<ResourceType, int> cost, BuildingType buildingType, bool gialap = false)
    {  
        if(!((numHouse < 2 && buildingType == BuildingType.Settlement) || (numRoad < 2 && buildingType == BuildingType.Road)))
        {
            foreach (var kv in cost)
            {
                Resource res = GetResByName(kv.Key);
                if (res.number < kv.Value)
                {
                    Debug.Log("Không đủ Cost để xây");
                    return -1;
                }

                    
            }
            if (!gialap)
            {
                foreach (var kv in cost)
                {
                    RemoveResource(kv.Key, kv.Value);
                }
            }
            
        }
        
        if(buildingType == BuildingType.Settlement && !gialap) numHouse ++;
        else if(buildingType == BuildingType.Road && !gialap) numRoad ++;
        return numHouse;
    }
    [Server]
    public void RemoveResource(ResourceType resourceName, int amount = 1)
    {
        Resource res = GetResByName(resourceName);
        resources.Remove(res);
        res.number -= amount;
        if(res.number < 0) res.number =0;
        resources.Add(res);
        Debug.Log($"[Server] Player {playerIndex} Remove: {res.resourceName} --- {amount}");
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
        if(!pause) GameButtonManager.instance.TargetActiveButtons(connectionToClient);
    }
    protected void UpdateDiceController(Vector3 mousePos)
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
        GameButtonManager.instance.RpcUpdateUIAfterEndTurn();
        TurnManager.instance.AdvanceTurn();
    }
    [Command]
    public void CmdPause()
    {
        Debug.Log($"[Server] Player {playerIndex} PAUSEEEE");
        pause = true;
        GameButtonManager.instance.RpcPause();
        // Pause();
    }

    [Command]
    public void CmdContinue()
    {
        Debug.Log($"[Server] Player {playerIndex} Continue");
        pause = false;
        GameButtonManager.instance.RpcContinue();
        // Continue();
    }

    [Server]
    public void Pause()
    {
        DiceController.instance.SetCanRoll(false);
        GameButtonManager.instance.RpcUpdateUIAfterPause();
    }
    [Server]
    public void Continue()
    {
        DiceController.instance.SetCanRoll(true);
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
    protected virtual void OnHandUpdated(SyncList<Resource>.Operation op, int index, Resource  oldItem, Resource newItem)
    {
        if (!isLocalPlayer) return;
        List<int> hand = new List<int>();
        foreach (var resource in resources)
        {
            for (int i = 0; i < resource.number; i++)
            {
                hand.Add((int)resource.resourceName);
            }
        }
        currentResources.Clear();
        foreach (var res in resources)
        {
            currentResources.Add(new Resource { resourceName = res.resourceName, number = res.number });
        }
        HandManager.instance.UpdateHandUI(hand);
    }
}
