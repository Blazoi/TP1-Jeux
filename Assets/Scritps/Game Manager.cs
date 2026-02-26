using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  [SerializeField] GameObject player;
  [SerializeField] GameObject ennemyPrefab;
  [SerializeField] GameObject baitPrefab;

  public static event Action<float, float> UpdatePoints;
  public static event Action<int> ComboUI;
  public static event Action<float> GameOverUI;

  Coroutine gameTimeCoroutine;
  bool canSpawnNextPair = true;
  bool isGameOver = false;

  int baitNumber = 0;
  float waitTime = 5;
  float points = 0;


  void Start()
  {
    CheatCodes cheats = transform.GetComponent<CheatCodes>();
    cheats.addPoints += AddPoints;
    cheats.deleteAllPairs += ResetGame;
    cheats.playComboAnimation += CallComboUI;
    cheats.testSetup += testSetupHandler;

    for (int i = 0; i < 3; i++) SpawnPair(true);
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
  IEnumerator SpawnPair(bool setUp = false)
  {
    /*
		 Déterminer position aléatoire
		 Même position pour le poisson avec un offset
		 Offset en y est 0, plus facile à jouer

		 Empêcher l'apparition d'un autre appât jusqu'après la fin du cooldown
		 */

    // Position
    Vector3 newBaitPosition = new Vector3(
      UnityEngine.Random.Range(-10, 10),
      UnityEngine.Random.Range(-5, 5),
      UnityEngine.Random.Range(-10, 10));

    float offset = UnityEngine.Random.Range(-2, 2);

    // https://gemini.google.com/share/896868075062
    Vector3 positionOffset = new Vector3(offset + 2 * Mathf.Sign(offset), 0, offset + 2 * Mathf.Sign(offset));

    // Nouvel appât
    GameObject newBait = Instantiate(baitPrefab, newBaitPosition, Quaternion.identity);

    // Nouveau Poisson
    GameObject newEnnemy = Instantiate(ennemyPrefab, newBaitPosition + positionOffset, Quaternion.identity);
    newEnnemy.GetComponent<EnnemyScript>().EnnemyDoneDrifting += EnnemyDoneDriftingHandler;

    baitNumber++;

    yield return new WaitForSeconds(setUp ? 0 : 1f);
    canSpawnNextPair = true;
  }

  void EnnemyDoneDriftingHandler(int baitCollided)
  {
    /*
		 Quand l'ennemi fini de bouger
		 Ajouter les points (baitCollided^2)
		 Afficher COMBO au hasard (0 ou 1) si baitCollided > 1
		*/

    AddPoints(baitCollided);

    if (baitCollided > 1)
    {
      int comboSelector = UnityEngine.Random.Range(0, 1);
      CallComboUI(comboSelector);
    }
  }
  void CallComboUI(int comboSelector)
  {
    ComboUI(comboSelector);
  }

  void AddPoints(float baitCollided)
  {
    float pointsToAdd = Mathf.Pow(baitCollided, 2);
    points += pointsToAdd;
    UpdatePoints(pointsToAdd, points);
  }


  // * Gameplay
  IEnumerator GameOver()
  {
    /*
		 Empêcher l'apparrition de nouvelles paires
		 Figer le joueur

		 Attendre 5 secondes
		 Détruire les paires existantes

     Faire apparaître 3 paires initiales
		 Réinitialiser et repartir le jeu normalement
		 */

    GameOverUI(points);
    canSpawnNextPair = false;
    player.GetComponent<Rigidbody>().isKinematic = true;

    yield return new WaitForSeconds(5);
    ResetGame();
    for (int i = 0; i < 3; i++) SpawnPair(true);
  }

  void ResetGame()
  {
    // Trouver tous les ennemis et appâts et les détruire
    GameObject[] ennemies = GameObject.FindGameObjectsWithTag("Ennemy");
    GameObject[] baits = GameObject.FindGameObjectsWithTag("Bait");

    foreach (GameObject ennemy in ennemies)
    {
      Destroy(ennemy);
    }
    foreach (GameObject bait in baits)
    {
      Destroy(bait);
    }

    player.transform.position = Vector3.zero;
    player.GetComponent<Rigidbody>().isKinematic = false;
    canSpawnNextPair = true;
    isGameOver = false;
    points = 0;
    baitNumber = 0;
  }

  void testSetupHandler(int fishAmount)
  {
    player.transform.position = Vector3.zero;
    player.transform.rotation = Quaternion.identity;

    GameObject[] baits = GameObject.FindGameObjectsWithTag("Bait");
    GameObject[] ennemies = GameObject.FindGameObjectsWithTag("Ennemy");

    foreach (GameObject ennemy in ennemies)
    {
      if (ennemy.name == "TestEnnemy")
      {
        Destroy(ennemy);
        break;
      }
    }

    foreach (GameObject bait in baits)
    {
      if (bait.name == "TestBait")
      {
        Destroy(bait);
      }
    }

    GameObject newFish = Instantiate(ennemyPrefab, new Vector3(0, 0, 2), Quaternion.identity);
    newFish.name = "TestEnnemy";
    for (int i = 1; i <= fishAmount; i++)
    {
      GameObject newBait = Instantiate(baitPrefab, new Vector3(0, 0, 2 + i * 2), Quaternion.identity);
      newBait.name = "TestBait";
    }
  }
}
