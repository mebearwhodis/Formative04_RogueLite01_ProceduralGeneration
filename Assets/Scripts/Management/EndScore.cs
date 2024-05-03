using TMPro;
using UnityEngine;

public class EndScore : MonoBehaviour
{
    private void Start()
    {
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        text.text = PlayerController.Instance.CoinAmount.ToString();
    }
}