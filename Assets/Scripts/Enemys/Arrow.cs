using UnityEngine;

public class Arrow : MonoBehaviour {
   [SerializeField] private Rigidbody2D _rigidbody;
   [SerializeField] private float _speed;
   [SerializeField] private int _takedDamage;
   
   public void startFly(Vector2 startPosition, bool sideIsRight) {
      transform.position = startPosition;
      if (!sideIsRight) {
         transform.Rotate(0, 180, 0);
      }
      Invoke(nameof(destroyArrow), 2f);
   }

   private void FixedUpdate() {
      _rigidbody.velocity = transform.right * _speed;
   }

   private void OnTriggerEnter2D(Collider2D other) {
      var player = other.GetComponent<PlayerController>();
      if (player != null) {
         player.takeDamage(_takedDamage);
         destroyArrow();
      }
   }

   private void destroyArrow() {
      Destroy(gameObject);
   }
}
