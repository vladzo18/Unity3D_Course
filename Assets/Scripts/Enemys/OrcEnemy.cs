using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OrcEnemy : MonoBehaviour {
    private enum OrcStates {
        Patrol,
        Attack
    };
    private OrcStates _curentState;

    [SerializeField] private float _patrolArea;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed;
    [SerializeField] private float _atackDelay;
    [SerializeField] private bool _isWalkingRight;
    [SerializeField] private LayerMask _whoIsEnemy;
    [SerializeField] private Arrow _arrow;
    [SerializeField] private Transform _muzzle;
    [SerializeField] private int _maxHealthAmount;
    [SerializeField] private Slider _healthBar;

    [Header("Animations")] 
    [SerializeField] private Animator _animator;
    [SerializeField] private string _walkAnimatinKey;
    [SerializeField] private string _atackAnimatinKey;

    private Vector2 _startPosition;
    private Collider2D _enemyInArea;
    private float _lastAtackTime;
    private int _curentHealthAmount;

    private Vector2 DrawingGizosCenter {
        get {
            if (_startPosition == Vector2.zero)
                return transform.position;
            else
                return _startPosition;
        }
    }
    
    private void Awake() {
        _curentState = OrcStates.Patrol;
        _startPosition = transform.position;
        _curentHealthAmount = _maxHealthAmount;
        _healthBar.maxValue = _maxHealthAmount;
        _healthBar.value = _curentHealthAmount;
    }

    private void Update() {
       _enemyInArea = Physics2D.OverlapBox(_startPosition, new Vector2(_patrolArea * 2, 2),0,  _whoIsEnemy);
    }

    private void FixedUpdate() {
        switch (_curentState) {
            case OrcStates.Patrol:
                patrol();
                break;
            case OrcStates.Attack:
                atack();
                break;
        }
    }

    private void atack() {
        if (!_enemyInArea) {
            setState(OrcStates.Patrol);
            return;
        }

        _rigidbody.velocity = Vector2.zero;
        
        
        if (Time.time - _lastAtackTime > _atackDelay) {
            _animator.SetBool(_atackAnimatinKey, true);
            _lastAtackTime = Time.time;
        } else {
            _animator.SetBool(_atackAnimatinKey, false);
        }
        
        _animator.SetBool(_walkAnimatinKey, false);

        tryFlipToEnemy();
    }
    
    private void Shoot() {
        Arrow arrow = Instantiate(_arrow);
        arrow.startFly(_muzzle.position, _isWalkingRight);
    }
    
    private void patrol() {
        if (_enemyInArea) {
            setState(OrcStates.Attack);
            return;
        }

        _animator.SetBool(_walkAnimatinKey, true);
        _animator.SetBool(_atackAnimatinKey, false);
        
        _rigidbody.velocity = transform.right * _speed;
        tryFlipFromWalkingBorder();
    }
    
    private void tryFlipFromWalkingBorder() {
        bool needTurnLeft = transform.position.x >= _startPosition.x + _patrolArea && _isWalkingRight;
        bool needTurnRight = transform.position.x <= _startPosition.x - _patrolArea && !_isWalkingRight;
        
        if (needTurnLeft || needTurnRight) flip();
    }

    private void tryFlipToEnemy() {
        if (_enemyInArea != null) {
            bool needTurnRight = _enemyInArea.gameObject.transform.position.x < transform.position.x && _isWalkingRight;
            bool needTurnLeft = _enemyInArea.gameObject.transform.position.x > transform.position.x && !_isWalkingRight;

            if (needTurnRight || needTurnLeft) flip();
        }
    }

    private void flip() {
        _isWalkingRight = !_isWalkingRight;
        transform.Rotate(0, 180, 0);
    }

    private void setState(OrcStates state) => _curentState = state;

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(DrawingGizosCenter, new Vector2(_patrolArea * 2, 2));
    }
}
