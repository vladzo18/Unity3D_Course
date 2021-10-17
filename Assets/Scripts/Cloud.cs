using UnityEngine;

public class Cloud : MonoBehaviour {
    [SerializeField] private float _cloudSpeed;
    [SerializeField] private float _timeOfLife;

    public void startFly(Vector2 startPosition) {
        transform.position = startPosition;
    }
    
    private void Start() {
        Invoke(nameof(destroyCloud), _timeOfLife);
    }

    private void FixedUpdate() {
        transform.Translate(_cloudSpeed, 0, 0);
    }

    private void destroyCloud() {
        Destroy(gameObject);
    }
}
