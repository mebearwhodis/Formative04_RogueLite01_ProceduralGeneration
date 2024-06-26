using UnityEngine;

public class Stairs : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Feet"))
        {
            SoundManager.Instance.PlaySound("GameWin");
            GameManager.Instance.SetGameState(GameManager.GameState.GameWonState);
        }
    }
}
