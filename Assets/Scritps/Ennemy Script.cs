using System;
using System.Collections;
using UnityEngine;

public class EnnemyScript : MonoBehaviour
{
  public event Action<int> EnnemyDoneDrifting;
  Vector3 originalPosition;
  int baitCollided = 0;
  bool canCollide = true;

  [SerializeField] GameObject tailRotationPivot;
  [SerializeField] float regularTailSpeed = 45;
  [SerializeField] GameObject lFin;
  [SerializeField] GameObject rFin;

  float tailSpeed;
  float fastTailSpeed;
  bool tailDirection = true;

  void Start()
  {
		Debug.Log(transform.localScale.magnitude);
    originalPosition = transform.position;
    transform.localScale = Vector3.zero;

		fastTailSpeed = regularTailSpeed * 2.5f;
  }

  void Update()
  {
    if (transform.localScale.magnitude < 1)
    {
      transform.localScale += new Vector3(2 * Time.deltaTime, 2 * Time.deltaTime, 2 * Time.deltaTime);
    }

		// Animations
    // Queue
    tailSpeed = transform.GetComponent<Rigidbody>().linearVelocity.magnitude > 0 ? fastTailSpeed : regularTailSpeed;
    tailSpeed *= Time.deltaTime;

    if (tailDirection)
    {
      tailRotationPivot.transform.localEulerAngles += new Vector3(0, tailSpeed, 0);
      if (tailRotationPivot.transform.localEulerAngles.y >= 40)
      {
        tailDirection = false;
      }
    }
    else
    {
      tailRotationPivot.transform.localEulerAngles -= new Vector3(0, tailSpeed, 0);
      if (tailRotationPivot.transform.localEulerAngles.y > 50)
      {
        tailDirection = true;
      }
    }

    // Nageoires
    if (tailDirection)
    {
      lFin.transform.localEulerAngles += new Vector3(0, tailSpeed / 4, 0);
      rFin.transform.localEulerAngles -= new Vector3(0, tailSpeed / 4, 0);
    }
    else
    {
      lFin.transform.localEulerAngles -= new Vector3(0, tailSpeed / 4, 0);
      rFin.transform.localEulerAngles += new Vector3(0, tailSpeed / 4, 0);
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
			Debug.Log("Collided with a bait");
      baitCollided++;
      Destroy(other.gameObject);

			if (canCollide) EnnemyDoneDrifting(1);
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
