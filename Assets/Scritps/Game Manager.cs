using System;
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

  public static event Action<float, float> UpdatePoints;
  public static event Action<int> ComboUI;
  public static event Action<float> GameOverUI;

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
      nb.transform.position = new Vector3(0, 0, 5 + i * 2);
    }

    // todo add the comboui animations to the cheats
    CheatCodes cheats = transform.GetComponent<CheatCodes>();
    cheats.addPoints += AddPoints;
    cheats.deleteAllPairs += DestroyAll;
    cheats.playComboAnimation += CallComboUI;
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
    float xBaitPosition = UnityEngine.Random.Range(-10f, 10f);
    float yBaitPosition = UnityEngine.Random.Range(-1.5f, 8f);
    float zBaitPosition = UnityEngine.Random.Range(-10f, 10f);

    float offset = UnityEngine.Random.Range(-2, 2);
    Vector3 positionOffset = new Vector3(offset, 0, offset);

    // Nouvel appât
    GameObject newBait = Instantiate(baitPrefab);
    newBait.transform.position = new Vector3(xBaitPosition, yBaitPosition, zBaitPosition);

    // Nouveau Poisson
    GameObject newEnnemy = Instantiate(ennemyPrefab);
    newEnnemy.GetComponent<EnnemyScript>().EnnemyDoneDrifting += AfterEnnemyDoneDrifting;
    newEnnemy.transform.position = newBait.transform.position + positionOffset;

    baitNumber++;

    yield return new WaitForSeconds(5f);
    canSpawnNextPair = true;
  }

  void AfterEnnemyDoneDrifting(int baitCollided)
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
      if (comboSelector == 0) CallComboUI(comboSelector);
      else CallComboUI(comboSelector);
    }
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

		 Réinitialiser et repartir le jeu normalement
		 */

    GameOverUI(points);
    canSpawnNextPair = false;
    player.GetComponent<Rigidbody>().isKinematic = true;

    yield return new WaitForSeconds(4);
    DestroyAll();
    yield return new WaitForSeconds(1);

    player.GetComponent<Rigidbody>().isKinematic = false;
    player.transform.position = Vector3.zero;
    canSpawnNextPair = true;
    isGameOver = false;
    points = 0;
    baitNumber = 0;
  }

  void DestroyAll()
  {
    // Trouver tous les ennemis et appâts et les détruire
    GameObject[] ennemies = GameObject.FindGameObjectsWithTag("Ennemy");
    GameObject[] baits = GameObject.FindGameObjectsWithTag("Bait");

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
      Debug.Log(target.transform.localScale);
      yield return null;
    }
    Destroy(target);
  }

  void CallComboUI(int comboSelector)
  {
    ComboUI(comboSelector);
  }
}
