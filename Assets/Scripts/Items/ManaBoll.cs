using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBoll : Item {
    [SerializeField] private int _takedMana;
    
    protected override void doItemAction(PlayerController player) {
        player.increseMana(_takedMana);
    }

    protected override bool canTakeItem(PlayerController player) {
        return !player.isManaFull();
    }
}
