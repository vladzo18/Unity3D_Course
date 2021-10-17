using Enemys;
using UnityEngine;
using UnityEngine.UI;

public class JellyEnemy : MonoBehaviour , IDamageable {
   [SerializeField] private float _walkRange;
   [SerializeField] private float _speed;
   [SerializeField] private int _takedDamage;
   [SerializeField] private float _takedPushPower;
   [SerializeField] private bool _isWalkingRight;
   [SerializeField] private Rigidbody2D _rigidbody;
   [SerializeField] private int _maxHealthAmount;
   [SerializeField] private Slider _healthBar;
   [SerializeField] private GameObject _enemySystem;
   
   private Vector2 _startPosition;
   private int _curentHealthAmount;

   private Vector2 DrawGizmosPosition {
      get {
         if (_startPosition == Vector2.zero) {
            return transform.position;
         } else {
            return _startPosition;
         }
      }
   }
   public int CurentHealthAmount {
      get => _curentHealthAmount;
      set {
         _curentHealthAmount = value;
         _healthBar.value = value;
         if (_curentHealthAmount <= 0) {
            Destroy(_enemySystem);
         }
      }
   }
   
   public void takeDamage(int takedDamage) {
      CurentHealthAmount -= takedDamage;
   }
   
   private void Start() {
      _startPosition = transform.position;
      _healthBar.maxValue = _maxHealthAmount;
      CurentHealthAmount = _maxHealthAmount;
   }

   private void FixedUpdate() {
      _rigidbody.velocity = transform.right * _speed;
   }

   private void Update() {
      if (isTouchingMovementBorder()) {
         flip();
      }
   }

   private bool isTouchingMovementBorder() {
      return transform.position.x > _startPosition.x + _walkRange && _isWalkingRight
             || transform.position.x < _startPosition.x - _walkRange && !_isWalkingRight;
   }

   private void flip() {
      _isWalkingRight = !_isWalkingRight;
      transform.Rotate(0, 180, 0);
   }
   
   private void OnDrawGizmos() {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireCube(DrawGizmosPosition, new Vector3(_walkRange * 2, 2, 0));
   }

   private void OnCollisionEnter2D(Collision2D other) {
      var player = other.collider.GetComponent<PlayerController>();
      player?.takeDamage(_takedDamage, _takedPushPower, transform.position.x);
   }
}
