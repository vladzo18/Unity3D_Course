using System;
using System.Data;
using UnityEngine;

public abstract class Item : MonoBehaviour {
    [SerializeField] private float _timeDelay = 0.2f;
    
    private DateTime _dateTime;
    
    private void OnTriggerEnter2D(Collider2D other) {
        var player = other.GetComponent<PlayerController>();
        bool timeIsValid = (DateTime.Now - _dateTime).TotalSeconds > _timeDelay;
        
        if ((player != null) && timeIsValid) {
            doItemAction(player);
            Destroy(gameObject);
            
            _dateTime = DateTime.Now;
        }
    }

    protected abstract void doItemAction(PlayerController player);
}
