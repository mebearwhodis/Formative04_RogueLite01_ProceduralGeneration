using UnityEngine;

public class Chest : MonoBehaviour
{
    private SpriteRenderer _sr;
    [SerializeField] private Sprite _openedSprite;
    private bool _opened = false;
    
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerBody") && !_opened)
        {
            _sr.sprite = _openedSprite;
            PlayerController.Instance.UpdateCoinCounter(20);
            SoundManager.Instance.PlaySound("ChestOpen");
            _opened = true;
        }
    }
}
