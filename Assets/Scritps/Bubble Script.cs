using UnityEngine;

public class BubbleScript : MonoBehaviour
{
  void Start()
  {
    transform.parent.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 5, 0);
		transform.localScale = new Vector3(.1f, .1f, .1f);
  }
  void FixedUpdate()
  {
		if (transform.localScale.magnitude < .4f) transform.localScale += new Vector3(.25f * Time.fixedDeltaTime, .25f * Time.fixedDeltaTime, .25f * Time.fixedDeltaTime);	
		if (transform.parent.transform.localPosition.y >= 15) Destroy(transform.parent.gameObject);
  }

  void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Wall"))
		{
			Destroy(transform.parent.gameObject);
		}
	}
}
