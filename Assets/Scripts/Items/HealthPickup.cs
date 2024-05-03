using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private Sprite _halfHeart;
    [SerializeField] private Sprite _fullHeart;
    private SpriteRenderer _sr;
    private int _value = 0;

    private void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _value = Random.Range(1, 3);
        if (_value == 1)
        {
            _sr.sprite = _halfHeart;
        }
        else
        {
            _sr.sprite = _fullHeart;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBody"))
        {
            PlayerController.Instance.UpdateHealth(_value);
            Destroy(gameObject);
        }
    }
}