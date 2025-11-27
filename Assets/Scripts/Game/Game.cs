using UnityEngine;

using System.Collections.Generic;

public class Game : MonoBehaviour
{
    protected List<Player> players = new List<Player>();
    private int currentPlayerIndex = 0;
    public GameObject playerPrefab;
    private CatanMap catanMap;
    private GameButtonManager gameButtonManager;
    private DiceController diceController;
    public static Game instance;

    protected void Awake()
    {
        if(Game.instance != null) Debug.LogError("On 1 Game");
        Game.instance = this;
    }
    private void Start()
    {
        catanMap = CatanMap.instance;
        gameButtonManager = GameButtonManager.instance;
        diceController = DiceController.instance;
        Color[] colors = { Color.red, Color.white, Color.pink, Color.yellow };
        // for (int i = 0; i < colors.Length; i++)
        // {
        //     GameObject go = Instantiate(playerPrefab);
        //     Player p = go.GetComponent<Player>();
        //     p.color = colors[i];
        //     players.Add(p);
        // }
    }
    private void Update()
    {
    }
    public Player CurrentPlayer => players[currentPlayerIndex];
    public void Setup(){}

    public void OnEndTurn()
    {
        // diceController.canRoll = true;
        currentPlayerIndex = (currentPlayerIndex + 1)%players.Count;
    }
    public void CollectResources(int finalValue)
    {   
        // gameButtonManager.ActiveButtons();
        if (finalValue == 7)
        {
            Debug.Log("Di chuyển Robber"); 
            return;
        }
        // catanMap.GetResourcesByDiceNumber(finalValue); 
        
    }
}
