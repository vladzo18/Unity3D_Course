using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour {
    private delegate void someAction(PlayerController player);
    
    private void OnTriggerEnter2D(Collider2D other) {
        doActionIfObjectNotNull(other, (obj => obj.canClimb = true ));
    }

    private void OnTriggerExit2D(Collider2D other) {
        doActionIfObjectNotNull(other, (obj => obj.canClimb = false ));
    }

    private void doActionIfObjectNotNull(Collider2D other, someAction sm) {
        var player = other.GetComponent<PlayerController>();
        if (player != null) {
            sm?.Invoke(player);
        }
    }
}

