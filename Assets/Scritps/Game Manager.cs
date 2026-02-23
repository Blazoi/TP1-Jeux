using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
	[SerializeField] GameObject player;
	[SerializeField] GameObject ennemyPrefab;
	[SerializeField] GameObject baitPrefab;

	[SerializeField] Canvas canvas;
	[SerializeField] TMP_Text gameOverText;
	[SerializeField] TMP_Text timeText;
	[SerializeField] TMP_Text pointsText;
	[SerializeField] TMP_Text plusPointsAnimatedText;
	[SerializeField] TMP_Text comboAnimated;

	float gameTime = 0;
	int baitNumber = 0;
	Coroutine gameTimeCoroutine;
	bool canSpawnNextPair = true;
	bool isGameOver = false;

	float waitTime = 5;
	float points = 0;


	void Start()
	{
		// todo delete those when done testing
		// todo dont forget to start the game with 3 pairs
		GameObject ne = Instantiate(ennemyPrefab);
		ne.transform.position = new Vector3(0, 0, 3);
		ne.GetComponent<EnnemyScript>().EnnemyDoneDrifting += AfterEnnemyDoneDrifting;

		for (int i = 0; i < 2; i++)
		{
			GameObject nb = Instantiate(baitPrefab);
			nb.transform.position = new Vector3(0, 0, 5 + i*2);
		}

		// todo add the comboui animations to the cheats
		CheatCodes cheats = transform.AddComponent<CheatCodes>();
		cheats.addPoints += AfterEnnemyDoneDrifting;
		cheats.deleteAllPairs += BaitEnnemyDestruction;

		gameTimeCoroutine = StartCoroutine(GameTimeCounter());

		timeText.rectTransform.localPosition = new Vector3(475, -200, 0);
		pointsText.rectTransform.localPosition = new Vector3(475, -250, 0);
	}

	void Update()
	{
		/*
		 Faire apparaître une paire après cooldown et si nombre < 20
		 Sinon commencer la séquence de fin de jeu
		*/
		if (canSpawnNextPair && baitNumber < 20)
		{
			waitTime = Mathf.Max(2, 5 - Mathf.Pow(points, .25f));

			canSpawnNextPair = false;
			StartCoroutine(SpawnPair());
		}
		else if (baitNumber >= 20 && !isGameOver)
		{
			isGameOver = true;
			StartCoroutine(GameOver());
		}
	}

	// * Ennemis et appâts
	IEnumerator SpawnPair()
	{
		/*
		 Déterminer position aléatoire
		 Même position pour le poisson avec un offset
		 Offset en y est 0, plus facile à jouer

		 Empêcher l'apparition d'un autre appât jusqu'après la fin du cooldown
		 */


		// Position
		float xBaitPosition = Random.Range(-10f, 10f);
		float yBaitPosition = Random.Range(-1.5f, 8f);
		float zBaitPosition = Random.Range(-10f, 10f);

		float offset = Random.Range(-2, 2);
		Vector3 positionOffset = new Vector3(offset, 0, offset);

		// Nouvel appât
		GameObject newBait = Instantiate(baitPrefab);
		newBait.transform.position = new Vector3(xBaitPosition, yBaitPosition, zBaitPosition);

		// Nouveau Poisson
		GameObject newEnnemy = Instantiate(ennemyPrefab);
		newEnnemy.GetComponent<EnnemyScript>().EnnemyDoneDrifting += AfterEnnemyDoneDrifting;
		newEnnemy.transform.position = newBait.transform.position + positionOffset;

		baitNumber++;

		yield return new WaitForSeconds(waitTime);
		canSpawnNextPair = true;
	}

	void AfterEnnemyDoneDrifting(int baitCollided)
	{
		/*
		 Quand l'ennemi fini de bouger
		 Ajouter les points (baitCollided^2)
		 Afficher COMBO si baitCollided > 1

		 Mettre à jour l'UI
		*/
		float addedPoints = Mathf.Pow(baitCollided, 2);
		points += addedPoints;

		if (baitCollided > 1) StartCoroutine(ShakeComboUI());
		pointsText.text = "Points: " + points;
		StartCoroutine(PlusPointsAnimation(addedPoints));

	}

	// * Gameplay
	IEnumerator GameOver()
	{
		/*
		 Arrêter le temps
		 Empêcher l'apparrition de nouvelles paires
		 Figer le joueur
		 Afficher l'UI de fin de jeu

		 Attendre 5 secondes
		 Détruire les paires existantes

		 Afficher l'UI régulier
		 Réinitialiser et repartir le jeu normalement
		 */
		StopCoroutine(gameTimeCoroutine);
		canSpawnNextPair = false;
		player.GetComponent<Rigidbody>().isKinematic = true;

		gameOverText.rectTransform.localScale = new Vector3(1, 1, 1);
		timeText.rectTransform.localPosition = Vector3.zero;
		pointsText.rectTransform.localPosition = new Vector3(0, -50, 0);

		yield return new WaitForSeconds(4);
		BaitEnnemyDestruction();
		yield return new WaitForSeconds(1);
		// yield return new WaitForSeconds(1);

		player.transform.position = Vector3.zero;
		gameOverText.rectTransform.localScale = Vector3.zero;
		timeText.rectTransform.localPosition = new Vector3(475, -200, 0);
		pointsText.rectTransform.localPosition = new Vector3(475, -250, 0);

		gameTimeCoroutine = StartCoroutine(GameTimeCounter());
		player.GetComponent<Rigidbody>().isKinematic = false;
		canSpawnNextPair = true;
		isGameOver = false;
		gameTime = 0;
		baitNumber = 0;
	}

	void BaitEnnemyDestruction()
	{
		// Trouver tous les ennemis et appâts et les détruire
		GameObject[] ennemies = GameObject.FindGameObjectsWithTag("Ennemy");
		GameObject[] baits = GameObject.FindGameObjectsWithTag("Bait");

		// TODO Make a for loop to start a coroutine for every object to get them to Vector3.zero
		// TODO Keep this for loop to destroy them afterwards

		foreach (GameObject ennemy in ennemies)
		{
			StartCoroutine(ScaleDown(ennemy));
		}
		foreach (GameObject bait in baits)
		{
			StartCoroutine(ScaleDown(bait));
		}
	}

	IEnumerator ScaleDown(GameObject target)
	{
		while (target.transform.localScale.magnitude > 0)
		{
			target.transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime;
			yield return null;
		}
		Destroy(target);
	}

	// * User Interface
	IEnumerator GameTimeCounter()
	{
		// Ajouter .01s au temps, l'arrondir et update l'UI
		yield return new WaitForSeconds(.1f);
		gameTime += .1f;

		gameTime = Mathf.Round(gameTime * 100) / 100;

		timeText.text = "Temps: " + gameTime;
		gameTimeCoroutine = StartCoroutine(GameTimeCounter());
	}

	IEnumerator ShakeComboUI()
	{
		// Faire défiler de gauche à droite COMBO
		while (comboAnimated.rectTransform.localPosition.x < 750)
		{
			comboAnimated.rectTransform.localPosition += new Vector3(2f, 0, 0);
			yield return null;
		}
		comboAnimated.rectTransform.localPosition = new Vector3(-750, 0, 0);
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
}
