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
   private PlayerController _enemy;
   private Collider2D _enemyInAtackArea;

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
         this._enemy = player;
      }
   }

   private void OnTriggerExit2D(Collider2D other) {
      var player = other.GetComponent<PlayerController>();
      if (this._enemy == player) {
         this._enemy = null;
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
}
