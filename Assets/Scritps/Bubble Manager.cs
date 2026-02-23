using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
	[SerializeField] GameObject bubblePrefab;
	bool canSpawnNextGroup = true;

	void Update()
	{
		if (canSpawnNextGroup)
		{
			canSpawnNextGroup = false;
			StartCoroutine(SpawnGroup());
		}
	}

	IEnumerator SpawnGroup()
	{
		// Position
		float xPosition = Random.Range(-20f, 20f);
		float zPosition = Random.Range(-20f, 20f);

		GameObject emptyParent = new GameObject("Bubbles");
		emptyParent.AddComponent<Rigidbody>();
		emptyParent.GetComponent<Rigidbody>().useGravity = false;
		emptyParent.transform.position = new Vector3(xPosition, -2, zPosition);

		for (int i = 0; i < 40; i++)
		{
			// Position
			float xBubblePosition = Random.Range(-7.5f, 7.5f);
			float yBubblePosition = Random.Range(-1f, 1f);
			float zBubblePosition = Random.Range(-7.5f, 7.5f);

			GameObject newBubble = Instantiate(bubblePrefab);
			newBubble.transform.parent = emptyParent.transform;
			newBubble.transform.localPosition = new Vector3(xBubblePosition, yBubblePosition, zBubblePosition);
			newBubble.AddComponent<BubbleScript>();
		}
		yield return new WaitForSeconds(.5f);
		canSpawnNextGroup = true;
	}
}
