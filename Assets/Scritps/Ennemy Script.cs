using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class EnnemyScript : MonoBehaviour
{
  public event Action<int> EnnemyDoneDrifting;
  Vector3 originalPosition;
  int baitCollided = 0;

  [SerializeField] GameObject tailRotationPivot;
  [SerializeField] float regularTailSpeed = 45;
  [SerializeField] GameObject lFin;
  [SerializeField] GameObject rFin;

  float tailSpeed;
  float fastTailSpeed;
  bool tailDirection = true;

  void Start()
  {
    originalPosition = transform.position;
    transform.localScale = Vector3.zero;
    // StartCoroutine(YRotation());

    fastTailSpeed = regularTailSpeed * 2.5f;
  }

  void Update()
  {
    if (transform.localScale.magnitude < 2f)
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
    if (collision.transform.CompareTag("Wall")) return;

    GetComponent<Rigidbody>().isKinematic = false;
    Coroutine countdownCoroutine = StartCoroutine(Countdown());

    if (collision.gameObject.CompareTag("Player"))
    {
      collision.transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
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
    yield return new WaitForSeconds(2.5f);

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
  }

  // IEnumerator YRotation()
  // {
  //   float waitTime = UnityEngine.Random.Range(.5f, 2f);
  //   float rotationToAdd = UnityEngine.Random.Range(-100, 180);
  //   yield return new WaitForSeconds(waitTime);

  //   float initialYRotation = transform.eulerAngles.y;
  //   float goalRotation = (initialYRotation + rotationToAdd);
  //   float rotationTracker = 0;

  //   while (goalRotation - (initialYRotation + rotationTracker) > 1)
  //   {
  //     float rotationIncrement = rotationToAdd * Time.deltaTime;
  //     transform.Rotate(0, rotationIncrement, 0);
  //     rotationTracker += rotationIncrement;
  //     yield return null;
  //   }

  //   StartCoroutine(YRotation());
  // }
  // IEnumerator YRotation()
  // {
  //   float newAngle = UnityEngine.Random.Range(-90, 90);



  //   StartCoroutine(YRotation());
  // }

  // todo FINISH THE ROTATION
  IEnumerator YRotation()
  {
    float rotationIncrement = UnityEngine.Random.Range(-90, 90);
    float waitTime = UnityEngine.Random.Range(.5f, 2f);
    
    float initialY = transform.eulerAngles.y;
    float goal = initialY + rotationIncrement;
    float rotated =  0;

    yield return new WaitForSeconds(waitTime);

    while (goal - (transform.eulerAngles.y + rotated) >= 1 )
    {
      float step =  rotationIncrement * Time.deltaTime;
      rotated += step;

      transform.Rotate(new Vector3(0, step, 0));
      
      yield return null;
    }
    StartCoroutine(YRotation());
  }
}