using UnityEngine;

public class BaitScript : MonoBehaviour
{
	void Start()
	{
		transform.localScale = Vector3.zero;
	}

	void Update()
	{
		if (transform.localScale.magnitude < 1.7)
		{
			transform.localScale += new Vector3(2 * Time.deltaTime, 2 * Time.deltaTime, 2 * Time.deltaTime);
		}
	}
}
