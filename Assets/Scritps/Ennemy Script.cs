using System;
using System.Collections;
using UnityEngine;

public class EnnemyScript : MonoBehaviour
{
	public event Action<int> EnnemyDoneDrifting;

	private Vector3 originalPosition;
	private int baitCollided = 0;
	private bool canCollide = true;

	void Start()
	{
		originalPosition = transform.position;
		transform.localScale = Vector3.zero;
	}

	void Update()
	{
		if (transform.localScale.magnitude < 1)
		{
			transform.localScale += new Vector3(2 * Time.deltaTime, 2 * Time.deltaTime, 2 * Time.deltaTime);
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player") && canCollide)
		{
			canCollide = false;
			Coroutine countdownCoroutine = StartCoroutine(Countdown());
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Bait"))
		{
			baitCollided++;
			Destroy(other.gameObject);
		}
	}

	IEnumerator Countdown()
	{
		yield return new WaitForSeconds(1.5f);

		if (baitCollided > 0)
		{
			EnnemyDoneDrifting(baitCollided);
			Destroy(gameObject);
		}
		else
		{
			transform.position = originalPosition;
			transform.eulerAngles = Vector3.zero;
			GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
			GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}
		canCollide = true;
	}
}
