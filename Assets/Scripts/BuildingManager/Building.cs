using UnityEngine;
using System.Collections.Generic;
using Mirror;

public abstract class Building : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnPlayerChanged))] public PlayerNetwork owner;
    public abstract BuildingType Type { get; }
    public abstract Dictionary<ResourceType, int> Cost { get; }

    [SyncVar(hook = nameof(OnColorChanged))] 
    public Color color = Color.blue;

    private SpriteRenderer sr;

    void Awake() {
        sr = GetComponent<SpriteRenderer>();
    }

    public virtual void Initialize(PlayerNetwork owner)
    {
        this.owner = owner;
        color = owner.color;
    }

    public abstract bool CanPlace(Building build, bool gialap = false);
    public abstract void GiveResource(ResProduction res);

    public virtual string PrintInfo()
    {
        return "Building";
    }

    void OnPlayerChanged(PlayerNetwork oldPlayer, PlayerNetwork newPlayer)
    {
         Debug.Log($"[Building] Player index: {newPlayer.playerIndex}");
    }
    void OnColorChanged(Color oldColor, Color newColor) {
        if (sr != null) sr.color = newColor;
    }

}
