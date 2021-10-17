using UnityEngine;

public class Spikes : MonoBehaviour {
    [SerializeField] private float _damageDelay;
    [SerializeField] private int _takedDamage;
    
    private PlayerController _player;
    private float _lastDamageTime;
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (_player == null) {
            _player = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        var player = other.GetComponent<PlayerController>();
        if (_player == player) {
            _player = null;
        }
    }

    private void FixedUpdate() {
        if (_player != null && Time.time - _lastDamageTime > _damageDelay) {
            _player.takeDamage(_takedDamage);
            _lastDamageTime = Time.time;
        }
    }
}
