using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DysonHarvest
{
    // Attach to the GameEndPanel GameObject in the Canvas.
    // Connect OnVictory and OnGameOver from GameManager in the Inspector,
    // or call ShowVictory() / ShowGameOver() directly.
    public class GameEndPanel : MonoBehaviour
    {
        public TMP_Text titleText;
        public TMP_Text subtitleText;

        private static readonly Color VictoryColor  = new Color(1f, 0.85f, 0.1f);
        private static readonly Color GameOverColor = new Color(1f, 0.2f, 0.2f);

        private void Start()
        {
            var gm = GameManager.Instance;
            if (gm != null)
            {
                gm.OnVictory  += ShowVictory;
                gm.OnGameOver += ShowGameOver;
            }
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            var gm = GameManager.Instance;
            if (gm != null)
            {
                gm.OnVictory  -= ShowVictory;
                gm.OnGameOver -= ShowGameOver;
            }
        }

        public void ShowVictory()
        {
            gameObject.SetActive(true);
            if (titleText != null)
            {
                titleText.text = "VICTORIA";
                titleText.color = VictoryColor;
            }
            if (subtitleText != null)
                subtitleText.text = "La Esfera de Dyson está completa.\n¡El Sol está bajo tu control!";
        }

        public void ShowGameOver()
        {
            gameObject.SetActive(true);
            if (titleText != null)
            {
                titleText.text = "DERROTA";
                titleText.color = GameOverColor;
            }
            if (subtitleText != null)
                subtitleText.text = "La energía se agotó.\nEl portal se ha cerrado.";
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
