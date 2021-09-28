using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
   
   [Header("Movement")]
   [SerializeField] private Transform _transform;
   [SerializeField] private Rigidbody2D _rigidbody;
   [SerializeField] private SpriteRenderer _spriteRenderer;
   
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
   [SerializeField] private MoveVariant _variantOfMovement;
   [SerializeField] private int _maxHealyhAmount;
   [SerializeField] private int _currentHealthAmount;
   [SerializeField] private int _maxManaAmount;
   [SerializeField] private int _currentManaAmount;

   [Header("Animation")]
   [SerializeField] private Animator _animator;
   [SerializeField] private string _jumpAnimationKey;
   [SerializeField] private string _runAnimationKey;
   [SerializeField] private string _crouchAnimationKey;

   [Header("UI")] 
   [SerializeField] private TMP_Text _coinsAmountText;
   [SerializeField] private Slider _healthAmountSlider;
   [SerializeField] private Slider _manaAmountSlider;

   private float _horizontalDirection;
   private float _verticalDirection;
   private bool _isJumping;
   private bool _isCrouching;
   private bool _jumpLock = false;
   private int _coins;

   public bool canClimb { get; set; }

   public int Coins {
      get => _coins;
      set {
         _coins = value;
         _coinsAmountText.text = value.ToString();
      }
   }
   
   private void Awake() {
      _healthAmountSlider.maxValue = _maxHealyhAmount;
      _healthAmountSlider.value = _currentHealthAmount;
      _manaAmountSlider.maxValue = _maxManaAmount;
      _manaAmountSlider.value = _currentManaAmount;
   }

   private void Update() {
      _horizontalDirection = Input.GetAxis("Horizontal");
      _verticalDirection = Input.GetAxis("Vertical");
      _isJumping = Convert.ToBoolean(Input.GetAxis("Jump"));
      _isCrouching = Input.GetKey(KeyCode.C);
   }

   private void FixedUpdate() {
      movePlayerLogic(_variantOfMovement);
      
      jumpLogic();
      crouchLogic();
      climbLogic();
   }

   private void OnDrawGizmos() {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(_groundedTrigger.position, _groundedTriggerRadius);
      Gizmos.color = Color.magenta;
      Gizmos.DrawWireSphere(_topTrigger.position, _topTriggerRadius);
   }
   
   private void movePlayerLogic(MoveVariant mv) {
      switch (mv) {
         case MoveVariant.RigitbodyVelocity: 
            _rigidbody.velocity = new Vector2(_horizontalDirection * _movementSpead, _rigidbody.velocity.y);
            break;
         case MoveVariant.RigitbodyAddForce:
            _rigidbody.AddForce(new Vector2(_horizontalDirection * _movementSpead, 0));
            break;
         case MoveVariant.TransformTranslete:
            _transform.Translate(_horizontalDirection * _movementSpead, 0, 0);
            break;
         case MoveVariant.TransformPosition:
            if (_horizontalDirection != 0 ) 
               _transform.position = _transform.position + (Vector3.right * (_horizontalDirection * _movementSpead));
            break;
         case MoveVariant.RigitbodyMovePosition:
            if (_horizontalDirection != 0 ) 
               _rigidbody.MovePosition(_rigidbody.position + (Vector2.right * (_horizontalDirection * _movementSpead)));
            break;
      }
      
      flipLogic();
      
      _animator.SetBool(_runAnimationKey, _horizontalDirection != 0);
   }

   private void jumpLogic() {
      bool _isGrounded = Physics2D.OverlapCircle(_groundedTrigger.position, _groundedTriggerRadius, _groundLayer);
      
      if (_isGrounded && _isJumping && !_jumpLock) {
         _rigidbody.AddForce(Vector2.up * _jumpForce);
         _isGrounded = false;
      }
      _animator.SetBool(_jumpAnimationKey, !_isGrounded && !canClimb);
   }

   private void flipLogic() {
      if (_horizontalDirection > 0 && _spriteRenderer.flipX) {
         _spriteRenderer.flipX = false;
      } else if (_horizontalDirection < 0 && !_spriteRenderer.flipX) {
         _spriteRenderer.flipX = true;
      }
   }
   
   private void crouchLogic() {
      bool canStand = !Physics2D.OverlapCircle(_topTrigger.position, _topTriggerRadius, _topLayer);
      
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
         _currentHealthAmount += healthAmount;
         StartCoroutine(someCorutine(healthAmount, _healthAmountSlider));
      }
   }

   public void increseMana(int manaAmount) {
      if (!isManaFull()) {
         _currentManaAmount += manaAmount;
         StartCoroutine(someCorutine(manaAmount, _manaAmountSlider));
      }
   }

   public void takeDamage(int damage) {
      _currentHealthAmount -= damage;
      _healthAmountSlider.value = _currentHealthAmount;

      if (_currentHealthAmount <= 0) {
         gameObject.SetActive(false);
         Invoke(nameof(reloadSceneIfPlayerDeed), 2f);
      }
   }

   public bool isManaFull() {
      return _currentManaAmount >= _maxManaAmount;
   }
   
   public bool isHealthFull() {
      return _currentHealthAmount >= _maxHealyhAmount;
   }
   
   private IEnumerator someCorutine(int amount, Slider slider) {
      int goalAmount = (int)(slider.value + amount);
      
      while (slider.value < goalAmount) {
         slider.value += 1;
         yield return new WaitForSeconds(0.035f);
      }
   }

   private void reloadSceneIfPlayerDeed() {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
   }

   private enum MoveVariant {
      TransformPosition,
      TransformTranslete,
      RigitbodyVelocity,
      RigitbodyAddForce,
      RigitbodyMovePosition
   }
}