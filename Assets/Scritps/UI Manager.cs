using System.Collections;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
  // --- Éléments d'Interface ---
  [SerializeField] Canvas canvas;
  [SerializeField] TMP_Text gameOverText;
  [SerializeField] TMP_Text timeText;
  [SerializeField] TMP_Text pointsText;
  [SerializeField] TMP_Text plusPointsAnimatedText;
  [SerializeField] TMP_Text comboText;

  // --- Gestion des Coroutines ---
  Coroutine gameTimeCoroutine;

  // --- État du Jeu ---
  float gameTime = 0;
  bool isTimeFlowing = true;
  bool isComboPlaying = false;

  void Start()
  {
    // Lancer le compteur et abonnement aux events du GameManager
    gameTimeCoroutine = StartCoroutine(GameTimeCounter());
    GetComponent<GameManager>().UpdatePoints += UpdatePointsHandler;
    GetComponent<GameManager>().ComboUI += ComboUIHandler;
    GetComponent<GameManager>().GameOverUI += GameOverHandler;

    // Initialisation de l'UI
    timeText.rectTransform.localPosition = new Vector3(-475, -200, 0);
    timeText.text = "Temps: 0s";
    pointsText.rectTransform.localPosition = new Vector3(-475, -250, 0);
  }

  IEnumerator GameTimeCounter()
  {
    // Update l'UI chaque .1s
    // On arrondit sinon erreurs comme: 0.4000000000001
    while (isTimeFlowing)
    {
      yield return new WaitForSeconds(.1f);
      gameTime += .1f;
      gameTime = Mathf.Round(gameTime * 100) / 100;

      timeText.text = "Temps: " + gameTime + "s";
    }
  }

  void ComboUIHandler(int comboSelector)
  {
    /*
     0 => Shake COMBO
     1 => Blink COMBO
    */
    if (!isComboPlaying)
    {
      isComboPlaying = true;
      StartCoroutine(comboSelector == 0 ? ShakeComboUI() : BlinkComboUI());
    }
  }

  IEnumerator ShakeComboUI()
  {
    /*
     Tracker le temps écoulé et temps entre chaque secouement
     Changer la position après chauqe délai
    */
    float timeEllapsed = 0;
    float shakeDelay = .05f;

    comboText.text = "COMBO";

    while (timeEllapsed < 1)
    {
      float xPosition = Random.Range(-15, 15);
      float yPosition = Random.Range(-15, 15);

      comboText.rectTransform.anchoredPosition = new Vector2(xPosition, yPosition);
      yield return new WaitForSeconds(shakeDelay);
      timeEllapsed += shakeDelay;
    }

    comboText.text = "";
    isComboPlaying = false;
  }
  IEnumerator BlinkComboUI()
  {
    /*
     Tracker le temps écoulé
     Attendre le délai
     Afficher "COMBO" chaque fois que isVisible = true
    */
    float timeEllapsed = 0;
    float blinkDelay = .05f;
    bool isVisible = true;

    while (timeEllapsed < 1)
    {
      comboText.text = isVisible ? "" : "COMBO";
      isVisible = comboText.text == "" ? false : true;
      yield return new WaitForSeconds(blinkDelay);
      timeEllapsed += blinkDelay;
    }

    comboText.text = "";
    isComboPlaying = false;
  }

  void UpdatePointsHandler(float addedPoints, float totalPoints)
  {
    pointsText.text = "Points: " + totalPoints;
    StartCoroutine(PlusPointsAnimation(addedPoints));
  }
  IEnumerator PlusPointsAnimation(float points)
  {
    /*
     Créer un nouveau texte
      le positionner
      le parenter
      définir le texte " + x points"
     
     L'animer (vers le haut) puis détruire
    */
    TMP_Text newText = Instantiate(plusPointsAnimatedText);
    newText.rectTransform.SetParent(canvas.transform);
    newText.rectTransform.anchoredPosition = new Vector3(250, -250, 0);
    newText.text = "+ " + points + " points";

    while (newText.rectTransform.anchoredPosition.y < -90)
    {
      newText.rectTransform.anchoredPosition += new Vector2(0, 1);
      yield return null;
    }

    Destroy(newText.gameObject);
  }

  void GameOverHandler(float scoreFinal)
  {
    // Arrêter le temps et commencer séquence côté UI
    isTimeFlowing = false;
    StartCoroutine(GameOver(scoreFinal));
  }
  IEnumerator GameOver(float scoreFinal)
  {
    /*
		 Arrêter le temps
		 Afficher l'UI de fin de jeu

		 Attendre 5 secondes

		 Afficher l'UI régulier
		 */

    StopCoroutine(gameTimeCoroutine);
    StoppedGameUI(scoreFinal);

    yield return new WaitForSeconds(5);

    ResetUI();
    isTimeFlowing = true;
    gameTime = 0;
    gameTimeCoroutine = StartCoroutine(GameTimeCounter());
  }

  void StoppedGameUI(float scoreFinal)
  {
    gameOverText.text = "Partie Finie";
    timeText.rectTransform.localPosition = Vector3.zero;
    pointsText.rectTransform.localPosition = new Vector3(0, -50, 0);
    pointsText.text = "Score final: " + scoreFinal;
  }
  void ResetUI()
  {
    gameOverText.text = "";
    timeText.rectTransform.localPosition = new Vector3(-475, -200, 0);
    pointsText.rectTransform.localPosition = new Vector3(-475, -250, 0);
    pointsText.text = "Points: 0";
    timeText.text = "Temps: 0s";
  }
}
