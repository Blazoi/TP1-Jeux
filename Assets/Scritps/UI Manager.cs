using System.Collections;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
  [SerializeField] Canvas canvas;
  [SerializeField] TMP_Text gameOverText;
  [SerializeField] TMP_Text timeText;
  [SerializeField] TMP_Text pointsText;
  [SerializeField] TMP_Text plusPointsAnimatedText;
  [SerializeField] TMP_Text comboText;

  float gameTime = 0;
  bool isTimeFlowing = true;

  void Start()
  {
    StartCoroutine(GameTimeCounter());
    GameManager.UpdatePoints += UpdatePointsHandler;
    GameManager.ComboUI += ComboUIHandler;
		GameManager.GameOverUI += GameOverHandler;

    timeText.rectTransform.localPosition = new Vector3(-475, -200, 0);
    pointsText.rectTransform.localPosition = new Vector3(-475, -250, 0);
  }

  IEnumerator GameTimeCounter()
  {
    // Ajouter .01s au temps, l'arrondir et update l'UI
    while (isTimeFlowing)
    {
      yield return new WaitForSeconds(.1f);
      gameTime += .1f;
      gameTime = Mathf.Round(gameTime * 100) / 100;

      timeText.text = "Temps: " + gameTime;
    }
  }

  void ComboUIHandler(int comboSelector)
  {
    StartCoroutine(comboSelector == 0 ? ShakeComboUI() : BlinkComboUI());
  }

  IEnumerator ShakeComboUI()
  {
    float timeEllapsed = 0;
    float shakeDelay = .05f;

    TMP_Text newCombo = Instantiate(comboText, canvas.transform);
    newCombo.rectTransform.anchoredPosition = new Vector2(0, 3);
    newCombo.text = "COMBO";

    while (timeEllapsed < 1)
    {
      float xPosition = Random.Range(-15, 15);
      float yPosition = Random.Range(-15, 15);

      newCombo.rectTransform.anchoredPosition = new Vector2(xPosition, yPosition);
      yield return new WaitForSeconds(shakeDelay);
      timeEllapsed += shakeDelay;
    }

    Destroy(newCombo.gameObject);
  }
  IEnumerator BlinkComboUI()
  {
    TMP_Text newCombo = Instantiate(comboText, canvas.transform);
    newCombo.rectTransform.anchoredPosition = new Vector2(10, 0);

    float timeEllapsed = 0;
    float blinkDelay = .05f;
    bool isVisible = true;

    while (timeEllapsed < 1)
    {
      if (!isVisible)
      {
        newCombo.text = "COMBO";
        isVisible = true;
      }
      else
      {
        newCombo.text = "";
        isVisible = false;
      }
      yield return new WaitForSeconds(blinkDelay);
      timeEllapsed += blinkDelay;
    }
    Destroy(newCombo.gameObject);
  }

  void UpdatePointsHandler(float addedPoints, float points)
  {
    pointsText.text = "Points: " + points;
    StartCoroutine(PlusPointsAnimation(addedPoints));
  }
  IEnumerator PlusPointsAnimation(float points)
  {
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

    gameOverText.rectTransform.localScale = new Vector3(1, 1, 1);
    timeText.rectTransform.localPosition = Vector3.zero;
    pointsText.rectTransform.localPosition = new Vector3(0, -50, 0);
		pointsText.text = "Score final: " + scoreFinal;

    yield return new WaitForSeconds(5);

    gameOverText.rectTransform.localScale = Vector3.zero;
    timeText.rectTransform.localPosition = new Vector3(-475, -200, 0);
    pointsText.rectTransform.localPosition = new Vector3(-475, -250, 0);
		pointsText.text = "Points: 0";
    timeText.text = "Temps: 0";

    gameTime = 0;
		isTimeFlowing = true;
  }
}
