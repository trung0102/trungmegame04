using UnityEngine;

public enum ActionType
{   
    BuildSettlement,
    BuildRoad,
    MoveRobber
}

public class GameAction
{
    public ActionType type;
    public Vector3 position; // vị trí nhà / đường / tile robber

    public GameAction(ActionType type, Vector3 pos)
    {
        this.type = type;
        position = pos;
    }
}
