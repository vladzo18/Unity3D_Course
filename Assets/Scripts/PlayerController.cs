using System;
using UnityEngine;

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

   [Header("Animation")]
   [SerializeField] private Animator _animator;
   [SerializeField] private string _jumpAnimationKey;
   [SerializeField] private string _runAnimationKey;
   [SerializeField] private string _crouchAnimationKey;
   
   private float _direction;
   private bool _isJumping;
   private bool _isCrouching;
   private bool _isGrounded;
   private bool _jumpLock = false;
   
   private void Update() {
      _direction = Input.GetAxis("Horizontal");
      _isJumping = Convert.ToBoolean(Input.GetAxis("Jump"));
      _isCrouching = Input.GetKey(KeyCode.C);
   }

   private void FixedUpdate() {
      _isGrounded = Physics2D.OverlapCircle(_groundedTrigger.position, _groundedTriggerRadius, _groundLayer);

      movePlayerLogic(_variantOfMovement);
      
      jumpLogic();
      
      crouchLogic();
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
            _rigidbody.velocity = new Vector2(_direction * _movementSpead, _rigidbody.velocity.y);
            break;
         case MoveVariant.RigitbodyAddForce:
            _rigidbody.AddForce(new Vector2(_direction * _movementSpead, 0));
            break;
         case MoveVariant.TransformTranslete:
            _transform.Translate(_direction * _movementSpead, 0, 0);
            break;
         case MoveVariant.TransformPosition:
            if (_direction != 0 ) 
               _transform.position = _transform.position + (Vector3.right * (_direction * _movementSpead));
            break;
         case MoveVariant.RigitbodyMovePosition:
            if (_direction != 0 ) 
               _rigidbody.MovePosition(_rigidbody.position + (Vector2.right * (_direction * _movementSpead)));
            break;
      }
      
      flipLogic();
      
      _animator.SetBool(_runAnimationKey, _direction != 0);
   }

   private void jumpLogic() {
      if (_isGrounded && _isJumping && !_jumpLock) {
         _rigidbody.AddForce(Vector2.up * _jumpForce);
         _isGrounded = false;
         _animator.SetBool(_jumpAnimationKey, true);
      }
      _animator.SetBool(_jumpAnimationKey, !_isGrounded);
   }

   private void flipLogic() {
      if (_direction > 0 && _spriteRenderer.flipX) {
         _spriteRenderer.flipX = false;
      } else if (_direction < 0 && !_spriteRenderer.flipX) {
         _spriteRenderer.flipX = true;
      }
   }
   
   private void crouchLogic() {
      bool canStand = !Physics2D.OverlapCircle(_topTrigger.position, _topTriggerRadius, _topLayer);
      
      _headerCollider.enabled = !_isCrouching && canStand;
      _animator.SetBool(_crouchAnimationKey,  !_headerCollider.enabled);
      _jumpLock = _animator.GetBool(_crouchAnimationKey);
   }

   public void increseHealth(int healthAmount) {
      Debug.Log("Ухх здоровье!) + " + healthAmount);
   }
   
   public void increseShieldForce(int shieldForceAmount) {
      Debug.Log("Вахх защита!) + " + shieldForceAmount);
   }
   
   private enum MoveVariant {
      TransformPosition,
      TransformTranslete,
      RigitbodyVelocity,
      RigitbodyAddForce,
      RigitbodyMovePosition
   }
}