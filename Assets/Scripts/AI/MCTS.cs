using UnityEngine;
using System.Collections.Generic;
public class MCTS
{
    private int iterations;

    public MCTS(int iterations)
    {
        this.iterations = iterations;
    }

    public MCTSNode Run(MCTSNode root)
    {
        for (int i = 0; i < iterations; i++)
        {
            MCTSNode node = Select(root);
            MCTSNode expanded = node.Expand();
            float value = expanded.Simulate();
            expanded.Backpropagate(value);
        }

        return BestChild(root);
    }

    private MCTSNode Select(MCTSNode node)
    {
        while (node.IsFullyExpanded() && node.children.Count > 0)
        {
            node = UCTSelect(node);
        }
        return node;
    }

    private MCTSNode UCTSelect(MCTSNode node)
    {
        MCTSNode best = null;
        float bestVal = float.MinValue;

        foreach (var c in node.children)
        {
            float uct = (c.reward / (c.visits + 1)) +
                        Mathf.Sqrt(2 * Mathf.Log(node.visits + 1) / (c.visits + 1));

            if (uct > bestVal)
            {
                bestVal = uct;
                best = c;
            }
        }

        return best;
    }

    private MCTSNode BestChild(MCTSNode node)
    {
        MCTSNode best = null;
        float max = float.MinValue;

        foreach (var c in node.children)
        {
            float val = c.reward / (c.visits + 1);
            if (val > max)
            {
                max = val;
                best = c;
            }
        }
        return best;
    }
}


public class MCTSNode
{
    public MCTSNode parent;
    public List<MCTSNode> children = new List<MCTSNode>();

    public int visits = 0;
    public float reward = 0;

    public PlayerNetwork player;
    public CatanMap map;
    public GameAction action;

    private static readonly Dictionary<int, int> diceWeight = new Dictionary<int, int>()
    {
        { 2, 1 }, { 3, 2 }, { 4, 3 }, {0, -3},
        { 5, 4 }, { 6, 5 }, { 8, 5 },
        { 9, 4 }, { 10, 3 }, { 11, 2 }, { 12, 1 }
    };

    public MCTSNode(MCTSNode parent, PlayerNetwork player)
    {
        this.parent = parent;
        this.player = player;
        this.map = CatanMap.instance;
    }

    public bool IsFullyExpanded()
    {
        return children.Count >= GetPossibleActions().Count;
    }

    public List<GameAction> GetPossibleActions()
    {
        List<GameAction> acts = new List<GameAction>();

        foreach (var v in map.vertexDict)
        {
            if (v.Value.CanBuild(BuildingType.Settlement, this.player, true) != null)
            {
                acts.Add(new GameAction(ActionType.BuildSettlement, v.Key));
            }
        }

        foreach (var e in map.edgeDict)
        {
            if (e.Value.CanBuild(BuildingType.Road, this.player, true) != null)
            {
                acts.Add(new GameAction(ActionType.BuildRoad, e.Key));
            }
        }

        return acts;
    }

    public MCTSNode Expand()
    {
        List<GameAction> actions = GetPossibleActions();

        foreach (var act in actions)
        {
            bool exists = false;

            foreach (var c in children)
                if (c.action.position == act.position && c.action.type == act.type)
                {
                    exists = true;
                    break;
                }

            if (!exists)
            {
                MCTSNode child = new MCTSNode(this, player);
                child.action = act;
                children.Add(child);
                return child;
            }
        }
        return this;
    }

    public float Simulate()
    {
        int score = 0;
        if(action == null) return score;
        if (action.type == ActionType.BuildSettlement)
        {
            Vertex v = map.vertexDict[action.position];
            score += v.adjacentProduct.Count;
            foreach (var production in v.adjacentProduct)
            {
                int num = production.information.numberToken;
                score += diceWeight[num];
            }
        }
        else if (action.type == ActionType.BuildRoad)
        {
            score += 7;
            if(TurnManager.instance.is_Setup) score += 10;
        }

        return score;
    }

    public void Backpropagate(float value)
    {
        visits++;
        reward += value;
        parent?.Backpropagate(value);
    }

    public string PrintInfo()
    {
        string ret = "[BOT] thuc hien: ";
        ret += $"{action.type} -- {action.position}";
        return ret;
    }
}
