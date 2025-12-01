using UnityEngine;
using Mirror;

public class AIManager : NetworkBehaviour
{
    public static AIManager instance;
    public GameObject botPrefab;

    private void Awake()
    {
        instance = this;
    }

    [Server]
    public void TakeTurn(PlayerNetwork aiPlayer)
    {   
        for (int i = 0; i < 4; i++)
        {   
            MCTSNode root = new MCTSNode(null, aiPlayer);
            MCTS mcts = new MCTS(30);

            MCTSNode best = mcts.Run(root);

            if (best == null || best.action == null)
            {
                Debug.Log("Bot không còn action để làm.");
                break;
            }
            ExecuteAction(aiPlayer, best.action);
        }
    }

    [Server]
    private void ExecuteAction(PlayerNetwork ai, GameAction action)
    {
        if (action.type == ActionType.BuildSettlement)
        {
            CatanMap.instance.OnClickBuilding(BuildingType.Settlement);
            CatanMap.instance.BOTBuild(ai, action.position);
        }
        else if (action.type == ActionType.BuildRoad)
        {
            CatanMap.instance.OnClickBuilding(BuildingType.Road);
            CatanMap.instance.BOTBuild(ai, action.position);
        }
    }
}