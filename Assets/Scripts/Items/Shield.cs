using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Item {
    [SerializeField] private int _takedShieldForce;
    
    protected override void doItemAction(PlayerController player) {
        player.increseShieldForce(_takedShieldForce);
    }
}
