using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelDoor : MonoBehaviour {
   [SerializeField] private int _coinsToNextLevel;
   [SerializeField] private SpriteRenderer _renderer;
   [SerializeField] private Sprite _openedDoorSprite;

   private void OnTriggerEnter2D(Collider2D other) {
      var player = other.GetComponent<PlayerController>();

      if (player != null && (_coinsToNextLevel == player.Coins)) {
         _renderer.sprite = _openedDoorSprite;
         Invoke(nameof(nextLevel), 2f);
      }
   }

   private void nextLevel() {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
   }
}
