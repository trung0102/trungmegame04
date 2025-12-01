using UnityEngine;
using System.Collections.Generic;
using Mirror;
using UnityEngine.InputSystem;

public class BotNetwork : PlayerNetwork
{   
    public override void OnStartLocalPlayer(){return;}

    protected override void Update()
    {   
        return;
        
    }
    
    protected override void Awake()
    {
        resources.Callback += OnHandUpdated;
        this.isBot = true;
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        TurnManager.instance.RegisterPlayer(this);
        Debug.Log("[BOT] Registered on server");
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        if (TurnManager.instance != null)
            TurnManager.instance.UnregisterPlayer(this);
        Debug.Log("[BOT] Unregistered from server");
    }

    protected override void OnHandUpdated(SyncList<Resource>.Operation op, int index, Resource  oldItem, Resource newItem)
    {
        if (isServer && isBot)
        {
            currentResources.Clear();
            foreach (var res in resources)
            {
                currentResources.Add(new Resource { resourceName = res.resourceName, number = res.number });
            }
        }
        
    }
}
