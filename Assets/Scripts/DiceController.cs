using UnityEngine;
using UnityEngine.InputSystem;

public class DiceController : MonoBehaviour
{
    public Dice dice1;     
    public Dice dice2;      
    private bool isRolling = false;
    public int finalValue  { get; private set; }

    private void Update()
    {
        // Kiểm tra click chuột trái
        if (!isRolling && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Chuyển tọa độ chuột sang world space
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0;

            // Kiểm tra có click trúng GameObject này không
            // Debug.Log("Clicked!");
            if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(mousePos))
            {
                StartCoroutine("RollBothDice");
            }
        }
    }

    private System.Collections.IEnumerator RollBothDice()
    {
        // Chờ cả 2 dice roll xong
        isRolling = true;
        Coroutine roll1 = dice1.StartCoroutine("RollTheDice");
        Coroutine roll2 = dice2.StartCoroutine("RollTheDice");

        yield return roll1;
        yield return roll2;

        finalValue = dice1.finalValue + dice2.finalValue;
        Debug.Log("Tổng 2 dice: " + finalValue);
        isRolling = false;
    }
}
