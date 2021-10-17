using Enemys;
using UnityEngine;
using UnityEngine.UI;

public class PlantEnemy : MonoBehaviour , IDamageable{
   [SerializeField] private float _atackArea;
   [SerializeField] private LayerMask _whoIsEnemy;
   [SerializeField] private int _takedDamage;
   [SerializeField] private bool _isLookingRight;
   [SerializeField] private float _atackDelay;
   [SerializeField] private int _maxHealthAmount;
   [SerializeField] private Slider _healthBar;
   [SerializeField] private GameObject _enemySystem;

   [Header("Animation")]
   [SerializeField] private string _atackAnimationKey;
   [SerializeField] private Animator _animator;
   
   private int _curentHealthAmount;
   private float _lastAtackTime;
   private PlayerController _enemy;
   private Collider2D _enemyInAtackArea;

   private Vector3 _sizeOfAtackArea {
      get {
         return new Vector3(_atackArea, 1.5f, 0);
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

   private void Awake() {
      _healthBar.maxValue = _maxHealthAmount;
      CurentHealthAmount = _maxHealthAmount;
   }

   private void Update() {
      _enemyInAtackArea = Physics2D.OverlapBox(transform.position, _sizeOfAtackArea, 0, _whoIsEnemy);

      if (_enemyInAtackArea && !isLookingToEnemy()) {
         flip();
      }
   }

   private void FixedUpdate() {
      if (_enemy != null && Time.time - _lastAtackTime > _atackDelay) {
         _enemy.takeDamage(_takedDamage);
         _animator.SetTrigger(_atackAnimationKey);
         _lastAtackTime = Time.time;
      }
   }

   private void OnTriggerEnter2D(Collider2D other) {
      var player = other.GetComponent<PlayerController>();
      if (player != null) {
         _enemy = player;
      }
   }

   private void OnTriggerExit2D(Collider2D other) {
      var player = other.GetComponent<PlayerController>();
      if (_enemy == player) {
         _enemy = null;
      }
   }
   
   private bool isLookingToEnemy() {
      return _enemyInAtackArea.transform.position.x > transform.position.x && _isLookingRight
         ||  _enemyInAtackArea.transform.position.x < transform.position.x && !_isLookingRight;
   }
   
   private void flip() {
      _isLookingRight = !_isLookingRight;
      transform.Rotate(0, 180, 0);
   }
   
   private void OnDrawGizmos() {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireCube(transform.position, _sizeOfAtackArea);
   }


   public void takeDamage(int takedDamage) {
      CurentHealthAmount -= takedDamage;
   }
}
