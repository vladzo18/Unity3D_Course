using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleHp : Item {
    [SerializeField] private int _takedHealth;
    
    protected override void doItemAction(PlayerController player) {
       player.increseHealth(_takedHealth);
    }

    protected override bool canTakeItem(PlayerController player) {
        return !player.isHealthFull();
    }
}
