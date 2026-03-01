using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
// --- Références & Prefabs ---
  [SerializeField] GameObject player;
  [SerializeField] GameObject ennemyPrefab;
  [SerializeField] GameObject baitPrefab;

  // --- Événements ---
  public event Action<float, float> UpdatePoints;
  public event Action<int> ComboUI;
  public event Action<float> GameOverUI;

  // --- États du Jeu ---
  bool canSpawnNextPair = true;
  bool isGameOver = false;

  // --- Données & Statistiques ---
  int baitNumber = 0;
  float waitTime = 5;
  float points = 0;


  void Start()
  {
    GetComponent<CheatCodes>().addPoints += AddPoints;
    GetComponent<CheatCodes>().deleteAllPairs += ResetGame;
    GetComponent<CheatCodes>().playComboAnimation += CallComboUI;
    GetComponent<CheatCodes>().testSetup += testSetupHandler;

    for (int i = 0; i < 2; i++) StartCoroutine(SpawnPair(true));
  }

  void Update()
  {
    /*
		 Faire apparaître une paire après cooldown et si nombre < 20
		 Sinon commencer la séquence de fin de jeu
		*/
    if (canSpawnNextPair && baitNumber < 21)
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
		 Déterminer une position aléatoire
		 Même position pour le poisson avec un offset

		 Empêcher l'apparition d'un autre appât jusqu'après la fin du cooldown
		 */

    // Position
    Vector3 newBaitPosition = new Vector3(
      UnityEngine.Random.Range(-10, 10),
      UnityEngine.Random.Range(-5, 5),
      UnityEngine.Random.Range(-10, 10));

    float offset = UnityEngine.Random.Range(-2, 2);

    // https://gemini.google.com/share/896868075062
    // Ajouter 2 dans la même direction pour empêcher qu'ils spawn l'un sur l'autre
    Vector3 positionOffset = new Vector3(offset + 2 * Mathf.Sign(offset), 0, offset + 2 * Mathf.Sign(offset));

    // Nouveaux GameObjects
    GameObject newBait = Instantiate(baitPrefab, newBaitPosition, Quaternion.identity);
    GameObject newEnnemy = Instantiate(ennemyPrefab, newBaitPosition + positionOffset, Quaternion.identity);
    newEnnemy.GetComponent<EnnemyScript>().EnnemyDoneDrifting += EnnemyDoneDriftingHandler;

    baitNumber++;

    if (!setUp)
    {
      yield return new WaitForSeconds(waitTime);
      canSpawnNextPair = true;
    }
  }

  void EnnemyDoneDriftingHandler(int baitCollided, GameObject ennemyFish)
  {
    /*
		 Ajouter les points
     Détruire l'ennemi
		*/

    AddPoints(baitCollided);
    Destroy(ennemyFish);
  }
  void CallComboUI(int comboSelector)
  {
    ComboUI(comboSelector);
  }

  void AddPoints(float baitCollided)
  {
    /*
     Ajouter points au total (baitCollided ^ 2)
     Au besoin, afficher Combo
     0 => Shake
     1 => Blink

     Update l'UI
    */

    float pointsToAdd = Mathf.Pow(baitCollided, 2);
    points += pointsToAdd;

    if (baitCollided > 1)
    {
      int comboSelector = UnityEngine.Random.Range(0, 2);
      CallComboUI(comboSelector);
    }
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
    /*
     Mettre le joueur au milieu
     Supprimer les objets de tests s'ils existent déjà

     Créer nouveau poisson devant le joueur
     Créer x nouveau appâts à 2 unités de chacun et le premier 2 unités du poisson
    */
    player.transform.position = Vector3.zero;
    player.transform.rotation = Quaternion.identity;

    // https://docs.unity3d.com/6000.3/Documentation/ScriptReference/GameObject.Find.html
    Destroy(GameObject.Find("TestEnnemy"));

    GameObject[] baits = GameObject.FindGameObjectsWithTag("Bait");
    foreach (GameObject bait in baits)
    {
      if (bait.name == "TestBait") Destroy(bait);
    }

    GameObject newFish = Instantiate(ennemyPrefab, new Vector3(0, 0, 2), Quaternion.identity);
    newFish.name = "TestEnnemy";
    newFish.GetComponent<EnnemyScript>().EnnemyDoneDrifting += EnnemyDoneDriftingHandler;

    for (int i = 1; i <= fishAmount; i++)
    {
      GameObject newBait = Instantiate(baitPrefab, new Vector3(0, 0, 2 + i * 2), Quaternion.identity);
      newBait.name = "TestBait";
    }
  }
}
