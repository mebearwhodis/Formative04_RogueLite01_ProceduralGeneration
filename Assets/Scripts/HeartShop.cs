using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeartShop : MonoBehaviour
{
    private int _price = 0;
    private TextMeshProUGUI _priceTag;
    [SerializeField] private Vector3 textOffset = new Vector3(0f, -20f, 0f); // Adjust this offset as needed

    void Start()
    {
        _price = Random.Range(3, 7);
        _priceTag = GetComponentInChildren<TextMeshProUGUI>();
        _priceTag.text = _price.ToString();

        // Calculate the position of the TextMeshProUGUI below the sprite
        Vector3 newTextPosition = transform.position + textOffset;

        // Set the position of the TextMeshProUGUI
        _priceTag.gameObject.transform.position = newTextPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBody"))
        {
            if (PlayerController.Instance.CoinAmount >= _price)
            {
                PlayerController.Instance.UpdateHealth(2);
                PlayerController.Instance.UpdateCoinCounter(-1 * _price);
                Destroy(gameObject);
            }
            else{return;}
        }
    }
}