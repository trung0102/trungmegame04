using UnityEngine;
using UnityEngine.InputSystem;  // cần namespace của Input System

public class Dice : MonoBehaviour
{
    private Sprite[] diceSides;
    private SpriteRenderer rend;
    public int finalValue { get; private set; }

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("Dice/");
    }

    private System.Collections.IEnumerator RollTheDice()
    {
        int randomDiceSide = 0;
        for (int i = 0; i <= 20; i++)
        {
            randomDiceSide = Random.Range(0, 6);
            rend.sprite = diceSides[randomDiceSide];
            yield return new WaitForSeconds(0.05f);
        }

        finalValue = randomDiceSide + 1;
        // Debug.Log(finalSide);
    }
}
