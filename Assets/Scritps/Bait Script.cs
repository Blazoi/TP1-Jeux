using System.Collections;
using UnityEngine;

public class BaitScript : MonoBehaviour
{
	void Start()
	{
		transform.localScale = Vector3.zero;
		StartCoroutine(ScaleUp());
	}
	IEnumerator ScaleUp()
  {
    while (transform.localScale.x < 1.25f)
    {
      transform.localScale += new Vector3(2 * Time.deltaTime, 2 * Time.deltaTime, 2 * Time.deltaTime);
      yield return null;
    }
  }

}
