using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentEnemy : MonoBehaviour {
    [SerializeField] private float _atackRange;
    [SerializeField] private LayerMask _whatIsPlayer;
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Bullet _bullet;
    [SerializeField] private bool _isLookingRight;
    [SerializeField] private int _maxHealthAmount;
    [SerializeField] private Slider _healthBar;

    [Header("Animations")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _shootAnimationKey;

    private bool _canShoot = false;
    private int _curentHealthAmount;
    
    private void FixedUpdate() {
        if (_canShoot) {
            return;
        }
        
        checkIfCanShoot();
    }

    private void Awake() {
        _curentHealthAmount = _maxHealthAmount;
        _healthBar.maxValue = _maxHealthAmount;
        _healthBar.value = _maxHealthAmount;
    }

    private void checkIfCanShoot() {
        Collider2D player = Physics2D.OverlapBox(transform.position, new Vector2(_atackRange, 2), 0, _whatIsPlayer);

        if (player != null) {
            _canShoot = true;
            startShoot(player.transform.position);
        } else {
            _canShoot = false;
        }
    }
    
    private void startShoot(Vector2 playerPosition) {
        if (transform.position.x > playerPosition.x && _isLookingRight 
            || transform.position.x < playerPosition.x && !_isLookingRight) {
            _isLookingRight = !_isLookingRight;
            transform.Rotate(0, 180, 0);
        }
        
        _animator.SetBool(_shootAnimationKey, true);
    }
    
    public void Shoot() {
        Bullet bullet = Instantiate(_bullet);
        bullet.startFly(transform.right, _muzzle.position);
       
        _animator.SetBool(_shootAnimationKey, false);
        
        Invoke(nameof(checkIfCanShoot), 1f);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(_atackRange, 2, 0));
    }
}
