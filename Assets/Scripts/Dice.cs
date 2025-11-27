using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class Dice : NetworkBehaviour
{
    private Sprite[] diceSides;
    private SpriteRenderer rend;
    [SyncVar(hook = nameof(OnDiceValueChanged))] public int finalValue = -1;

    private void Start()
    {           
        rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("Dice/");
    }

    [Server]
    public void ServerRollDice()
    {
        StartCoroutine("RollTheDice");
    }
    [Server]
    private System.Collections.IEnumerator RollTheDice()
    {
        int randomDiceSide = 0;
        for (int i = 0; i <= 20; i++)
        {
            randomDiceSide = Random.Range(0, 6);
            RpcUpdateDiceSprite(randomDiceSide);
            yield return new WaitForSeconds(0.05f);
        }

        finalValue = randomDiceSide + 1;
        // Debug.Log(finalSide);
    }

    [ClientRpc]
    private void RpcUpdateDiceSprite(int value)
    {
        if (rend != null && diceSides.Length >= 5)
            rend.sprite = diceSides[value];
    }

    private void OnDiceValueChanged(int oldVal, int newVal)
    {
        if (newVal > 0 && rend != null && diceSides.Length >= 6)
            rend.sprite = diceSides[newVal - 1];
    }
}
