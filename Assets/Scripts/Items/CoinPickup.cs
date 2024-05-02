using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBody"))
        {
            int randomCoinCount = Random.Range(1, 6);
            PlayerController.Instance.UpdateCoinCounter(randomCoinCount);
            Destroy(gameObject);
        }
    }
}