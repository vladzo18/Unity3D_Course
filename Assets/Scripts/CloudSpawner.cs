using UnityEngine;

public class CloudSpawner : MonoBehaviour {
    [SerializeField] private Cloud _cloud;
    [SerializeField] private float _spawnDelay;

    private float _lastSpawnTime;

    private void Update() {
        if (Time.time - _lastSpawnTime > _spawnDelay) {
            spawnCloud();
            _lastSpawnTime = Time.time;
        }
    }
    
    private void spawnCloud() {
        Cloud cloud = Instantiate(_cloud);
        cloud.startFly(transform.position);
    }
}
