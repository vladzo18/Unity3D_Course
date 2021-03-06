using Enemys;
using UnityEngine;
using UnityEngine.UI;

public class AgentEnemy : MonoBehaviour , IDamageable {
    [SerializeField] private float _atackRange;
    [SerializeField] private LayerMask _whatIsPlayer;
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Bullet _bullet;
    [SerializeField] private bool _isLookingRight;
    [SerializeField] private int _maxHealthAmount;
    [SerializeField] private Slider _healthBar;
    [SerializeField] private GameObject _enemySystem;

    [Header("Animations")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _shootAnimationKey;

    private bool _canShoot = false;
    private int _curentHealthAmount;
    private Collider2D _playerCollider;

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

    private void FixedUpdate() {
        if (_canShoot) {
            return;
        }
        checkIfCanShoot();
    }

    private void Awake() {
        _healthBar.maxValue = _maxHealthAmount;
        CurentHealthAmount = _maxHealthAmount;
    }

    private void checkIfCanShoot() {
        _playerCollider = Physics2D.OverlapBox(transform.position, new Vector2(_atackRange, 2), 0, _whatIsPlayer);

        if (_playerCollider) {
            _canShoot = true;
            startShoot();
        } else {
            _canShoot = false;
        }
    }
    
    private void startShoot() {
        if (!isLookingToPlayer()) {
            flip();
        }
        _animator.SetBool(_shootAnimationKey, true);
    }

    private bool isLookingToPlayer() {
        return transform.position.x >  _playerCollider.transform.position.x && !_isLookingRight 
               || transform.position.x < _playerCollider.transform.position.x && _isLookingRight;
    }
    
    private void flip() {
        _isLookingRight = !_isLookingRight;
        transform.Rotate(0, 180, 0);
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

    public void takeDamage(int takedDamage) {
        CurentHealthAmount -= takedDamage;
    }
}
