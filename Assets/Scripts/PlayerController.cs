using System;
using System.Collections;
using Enemys;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
   [Header("Movement")]
   [SerializeField] private Rigidbody2D _rigidbody;

   [Header("Jump")]
   [SerializeField] private Transform _groundedTrigger;
   [SerializeField] private LayerMask _groundLayer;
   [SerializeField] private float _groundedTriggerRadius;
   
   [Header("Crouch")]
   [SerializeField] private Transform _topTrigger;
   [SerializeField] private LayerMask _topLayer;
   [SerializeField] private float _topTriggerRadius;
   
   [SerializeField] private Collider2D _headerCollider;

   [Header("Player characteristic")]
   [SerializeField] private float _jumpForce;
   [SerializeField] private float _movementSpead;
   [SerializeField] private int _maxHealyhAmount;
   [SerializeField] private int _maxManaAmount;
   [SerializeField] private bool _isLookingRight;

   [Header("Animation")]
   [SerializeField] private Animator _animator;
   [SerializeField] private string _jumpAnimationKey;
   [SerializeField] private string _runAnimationKey;
   [SerializeField] private string _crouchAnimationKey;
   [SerializeField] private string _hurtAnimationKey;
   [SerializeField] private string _atackAnimationKey;
   [SerializeField] private string _castAnimationKey;

   [Header("UI")] 
   [SerializeField] private TMP_Text _coinsAmountText;
   [SerializeField] private Slider _healthAmountSlider;
   [SerializeField] private Slider _manaAmountSlider;

   [Header("Atack")] 
   [SerializeField] private int _swordDamage;
   [SerializeField] private Transform _swordAtackPoint;
   [SerializeField] private float _swordAtackRange;
   [SerializeField] private LayerMask _enemyLayer;
   [SerializeField] private int _castDamage;
   [SerializeField] private Transform _castAtackPoint;
   [SerializeField] private float _castLength;
   [SerializeField] private LineRenderer _castLine;

   private float _horizontalDirection;
   private float _verticalDirection;
   private bool _isJumping;
   private bool _isCrouching;
   private bool _jumpLock = false;
   private int _coins;
   private float _lastPushTime;
   private bool _isGrounded;
   private int _currentHealthAmount;
   private int _currentManaAmount;
   private bool _needToAtack;
   private bool _needToCast;

   public bool canClimb { get; set; }
   public int Coins {
      get => _coins;
      set {
         _coins = value;
         _coinsAmountText.text = value.ToString();
      }
   }
   private int CurrentHealthAmount {
      get => _currentHealthAmount;
      set {
         if (_currentHealthAmount < value && _currentHealthAmount != 0) {
            StartCoroutine(lerpIncreseBar(value - _currentHealthAmount, _healthAmountSlider));
         } else {
            _healthAmountSlider.value = value;
         }
         _currentHealthAmount = value;
      }
   }
   private int CurrentManaAmount {
      get => _currentManaAmount;
      set {
         if (_currentManaAmount < value && _currentManaAmount != 0) {
            StartCoroutine(lerpIncreseBar(value - _currentManaAmount, _manaAmountSlider));
         } else {
            _manaAmountSlider.value = value;
         }
         _currentManaAmount = value;
      }
   }

   private void Awake() {
      _healthAmountSlider.maxValue = _maxHealyhAmount;
      CurrentHealthAmount = _maxHealyhAmount;
      _manaAmountSlider.maxValue = _maxManaAmount;
      CurrentManaAmount = _maxManaAmount;
   }

   private void Update() {
      _horizontalDirection = Input.GetAxis("Horizontal");
      _verticalDirection = Input.GetAxis("Vertical");
      _isJumping = Convert.ToBoolean(Input.GetAxis("Jump"));
      _isCrouching = Input.GetKey(KeyCode.C);
      if (Input.GetButtonDown("Fire1")) {
         _needToAtack = true;
      }

      if (Input.GetButtonDown("Fire2")) {
         _needToCast = true;
      }
   }

   private void FixedUpdate() {
      _isGrounded = Physics2D.OverlapCircle(_groundedTrigger.position, _groundedTriggerRadius, _groundLayer);
      
      if (_animator.GetBool(_hurtAnimationKey)) {
         if (_isGrounded && Time.time - _lastPushTime > 0.02f) {
            _animator.SetBool(_hurtAnimationKey, false);
         }
         _needToAtack = false;
         _needToCast = false;
         return;
      }
      
      climbLogic();
      jumpLogic();
      crouchLogic();
     
      
      if (_needToAtack ) {
         startAtack();
         if (_isGrounded) {
            _horizontalDirection = 0;
         }
      }

      if (_needToCast && _isGrounded) {
         startCast();
         if (_isGrounded) {
            _horizontalDirection = 0;
         }
      }
      
      movePlayerLogic();
   }

   private void OnDrawGizmos() {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(_groundedTrigger.position, _groundedTriggerRadius);
      Gizmos.color = Color.magenta;
      Gizmos.DrawWireSphere(_topTrigger.position, _topTriggerRadius);
      Gizmos.color = Color.red;
      Gizmos.DrawWireCube(_swordAtackPoint.position, Vector2.one * _swordAtackRange);
   }
   
   private void movePlayerLogic() {
      _rigidbody.velocity = new Vector2(_horizontalDirection * _movementSpead, _rigidbody.velocity.y);
      _animator.SetBool(_runAnimationKey, _horizontalDirection != 0);
      
      if (isNotTurnedToMovementSide()) {
         flip();
      }
   }

   private bool isNotTurnedToMovementSide() {
      return (_horizontalDirection > 0 && !_isLookingRight) || (_horizontalDirection < 0 && _isLookingRight);
   }
   
   private void flip() {
      _isLookingRight = !_isLookingRight;
      transform.Rotate(0, 180, 0);
   }
   
   private void jumpLogic() {
      if (_isGrounded && _isJumping && !_jumpLock) {
         _rigidbody.AddForce(Vector2.up * _jumpForce);
         _isGrounded = false;
      }
      _animator.SetBool(_jumpAnimationKey, !_isGrounded && !canClimb);
   }
   
   private void crouchLogic() {
      bool canStand = !Physics2D.OverlapCircle(_topTrigger.position, _topTriggerRadius, _topLayer);

      if (!_headerCollider.enabled) {
         _needToAtack = false;
         _needToCast = false;
      }
      
      _headerCollider.enabled = !_isCrouching && canStand;
      _animator.SetBool(_crouchAnimationKey,  !_headerCollider.enabled);
      _jumpLock = _animator.GetBool(_crouchAnimationKey);
   }

   private void climbLogic() {
      if (canClimb) {
         _rigidbody.velocity =  new Vector2(_rigidbody.velocity.x, _verticalDirection * _movementSpead);
         _rigidbody.gravityScale = 0;
      } else {
         _rigidbody.gravityScale = 2;
      }
   }

   public void increseHealth(int healthAmount) {
      if (!isHealthFull()) { 
         CurrentHealthAmount += healthAmount;
      }
   }

   public void increseMana(int manaAmount) {
      if (!isManaFull()) {
         CurrentManaAmount += manaAmount;
      }
   }

   public void takeDamage(int damage, float pushPower = 0, float enemyPositionX = 0) {
      CurrentHealthAmount -= damage;

      if (CurrentHealthAmount <= 0) {
         gameObject.SetActive(false);
         Invoke(nameof(reloadSceneIfPlayerDeed), 2f);
      }

      if (pushPower != 0) {
         int pushDirection = transform.position.x > enemyPositionX ? 1 : -1;
         _rigidbody.AddForce(new Vector2(pushDirection * (pushPower / 2), pushPower));
         _animator.SetBool(_hurtAnimationKey, true);
         _lastPushTime = Time.time;
      }
   }

   public bool isManaFull() {
      return CurrentManaAmount >= _maxManaAmount;
   }
   
   public bool isHealthFull() {
      return CurrentHealthAmount >= _maxHealyhAmount;
   }
   
   private IEnumerator lerpIncreseBar(int amount, Slider slider) {
      while (amount != 0) {
         amount--;
         slider.value += 1;
         yield return new WaitForSeconds(0.035f);
      }
   }

   private void reloadSceneIfPlayerDeed() {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
   }

   private void startAtack() {
      if (_animator.GetBool(_atackAnimationKey)) {
         return;
      }
      _animator.SetBool(_atackAnimationKey, true);
   }

   private void startCast() {
      if (_animator.GetBool(_castAnimationKey)) {
         return;
      }

      if (CurrentManaAmount > 0) {
         _animator.SetBool(_castAnimationKey, true);
      } else {
         _needToCast = false;
      }
   }
   
   private void atack() {
      Collider2D[] targets = Physics2D.OverlapBoxAll(_swordAtackPoint.position, Vector2.one * _swordAtackRange, _enemyLayer);

      foreach (var target in targets) {
         IDamageable enemy = target.GetComponent<IDamageable>();
         enemy?.takeDamage(_swordDamage);
      }   
      
      _animator.SetBool(_atackAnimationKey, false);
      _needToAtack = false;
   }

   private void cast() {
      RaycastHit2D[] hits = Physics2D.RaycastAll(_castAtackPoint.position, transform.right, _castLength, _enemyLayer);

      foreach (var hit in hits) {
         IDamageable enemy = hit.collider.GetComponent<IDamageable>();
         enemy?.takeDamage(_castDamage);
      }

      CurrentManaAmount -= 30;
      _animator.SetBool(_castAnimationKey, false);
      _castLine.SetPosition(0, _castAtackPoint.position);
      _castLine.SetPosition(1, _castAtackPoint.position + transform.right * _castLength);
      _castLine.enabled = true;
      _needToCast = false;
      Invoke(nameof(disableLine), 0.1f);
   }

   private void disableLine() {
      _castLine.enabled = false;
   }
}