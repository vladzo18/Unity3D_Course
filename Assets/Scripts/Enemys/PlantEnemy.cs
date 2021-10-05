using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantEnemy : MonoBehaviour {
   [SerializeField] private float _atackArea;
   [SerializeField] private LayerMask _whoIsEnemy;
   [SerializeField] private int _takedDamage;
   [SerializeField] private bool _isLookingRight;
   [SerializeField] private float _atackDelay;
   [SerializeField] private int _maxHealthAmount;
   [SerializeField] private Slider _healthBar;

   [Header("Animation")]
   [SerializeField] private string _atackAnimationKey;
   [SerializeField] private Animator _animator;
   
   private int _curentHealthAmount;
   private float _lastAtackTime;
   private PlayerController player;
   
   private Vector3 _sizeOfAtackArea {
      get {
         return new Vector3(_atackArea, 1.5f, 0);
      }
   }
   
   private void Awake() {
      _curentHealthAmount = _maxHealthAmount;
      _healthBar.maxValue = _maxHealthAmount;
      _healthBar.value = _maxHealthAmount;
   }

   private void Update() {
      Collider2D _enemyInAtackArea = Physics2D.OverlapBox(transform.position, _sizeOfAtackArea, 0, _whoIsEnemy);
      tryToFlip(_enemyInAtackArea);
   }

   private void FixedUpdate() {
      if (player != null && Time.time - _lastAtackTime > _atackDelay) {
         player.takeDamage(_takedDamage);
         _animator.SetTrigger(_atackAnimationKey);
         _lastAtackTime = Time.time;
      }
   }

   private void OnTriggerEnter2D(Collider2D other) {
      var player = other.GetComponent<PlayerController>();
      if (player != null) {
         this.player = player;
      }
   }

   private void OnTriggerExit2D(Collider2D other) {
      var player = other.GetComponent<PlayerController>();
      if (this.player == player) {
         this.player = null;
      }
   }
   
   private void tryToFlip(Collider2D enemy) {
      if (enemy != null) {
         bool isRightSide = enemy.transform.position.x > transform.position.x ? true : false;

         if ((isRightSide && !_isLookingRight) || (!isRightSide && _isLookingRight)) {
            _isLookingRight = !_isLookingRight;
            transform.Rotate(0, 180, 0);
         }
      }
   }
   
   private void OnDrawGizmos() {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireCube(transform.position, _sizeOfAtackArea);
   }
}
