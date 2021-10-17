using UnityEngine;

public class Coin : Item {
    [SerializeField] private int _takedValue;
    
    protected override void doItemAction(PlayerController player) {
        player.Coins += _takedValue;
    }

    protected override bool canTakeItem(PlayerController player) => true;
}
