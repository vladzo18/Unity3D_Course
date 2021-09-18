using System;
using System.Data;
using UnityEngine;

public class Item : MonoBehaviour {
    
    [SerializeField] private float _timeDelay = 0.2f;
    
    private DateTime _dateTime;
    
    private void OnTriggerEnter2D(Collider2D other) {
        var player = other.GetComponent<PlayerController>();

        if ((DateTime.Now - _dateTime).TotalSeconds > _timeDelay) {
            if (player != null) {
                player.increseHealth();
                Destroy(gameObject);
                
                _dateTime = DateTime.Now;
            }
        }
    }
}
