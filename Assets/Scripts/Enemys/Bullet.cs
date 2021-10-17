using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private float _speed;
    [SerializeField] private int _takedDamage;
    
    public void startFly(Vector2 direction, Vector2 startPosition) {
        transform.position = startPosition;
        _renderer.flipX = direction.x < 0 ? true : false;
        _rigidbody.velocity = _speed * direction;
        Invoke(nameof(DestroyBullet), 2f);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var player = other.GetComponent<PlayerController>();
        player?.takeDamage(_takedDamage);
        DestroyBullet();
    }

    private void DestroyBullet() {
        Destroy(gameObject);
    }
}
