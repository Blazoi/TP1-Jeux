using System;
using System.Collections;
using UnityEngine;

public class EnnemyScript : MonoBehaviour
{
  // --- Configuration Visuelle ---
  [SerializeField] GameObject tailRotationPivot;
  [SerializeField] GameObject lFin;
  [SerializeField] GameObject rFin;
  [SerializeField] float regularTailSpeed = 45f;

  // --- Événements ---
  public event Action<int, GameObject> EnnemyDoneDrifting;

  // --- Composants ---
  Rigidbody selfRigidbody;
  Vector3 originalPosition;

  // --- État de l'Animation  ---
  float tailSpeed;
  float fastTailSpeed;
  bool tailDirection = true;

  // --- État du Gameplay ---
  int baitCollided = 0;
  bool isMoving = false;

  void Start()
  {
    originalPosition = transform.position;
    transform.localScale = Vector3.zero;
    selfRigidbody = GetComponent<Rigidbody>();
    StartCoroutine(ScaleUp());
    StartCoroutine(YRotation());

    fastTailSpeed = regularTailSpeed * 2.5f;
  }

  void Update()
  {


    // Animations
    // Queue & Nageoires
    tailSpeed = selfRigidbody.linearVelocity.magnitude > 0 ? fastTailSpeed : regularTailSpeed;
    tailSpeed *= Time.deltaTime;
    float currentYRotation = tailRotationPivot.transform.localEulerAngles.y;

    if (tailDirection)
    {
      tailRotationPivot.transform.localEulerAngles += new Vector3(0, tailSpeed, 0);
      tailDirection = currentYRotation >= 40 ? false : true;

      lFin.transform.localEulerAngles += new Vector3(0, tailSpeed / 4, 0);
      rFin.transform.localEulerAngles -= new Vector3(0, tailSpeed / 4, 0);
    }
    else
    {
      tailRotationPivot.transform.localEulerAngles -= new Vector3(0, tailSpeed, 0);
      tailDirection = currentYRotation > 50 ? true : false;

      lFin.transform.localEulerAngles -= new Vector3(0, tailSpeed / 4, 0);
      rFin.transform.localEulerAngles += new Vector3(0, tailSpeed / 4, 0);
    }
  }

  void OnCollisionEnter(Collision collision)
  {
    if (collision.transform.CompareTag("Wall")) return;

    if (!isMoving)
    {
      isMoving = true;
      selfRigidbody.isKinematic = false;
      StartCoroutine(Countdown());
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
      EnnemyDoneDrifting(baitCollided, gameObject);
    }
    else
    {
      transform.position = originalPosition;
      transform.eulerAngles = Vector3.zero;
      selfRigidbody.linearVelocity = Vector3.zero;
      selfRigidbody.angularVelocity = Vector3.zero;
    }

    isMoving = false;
  }

  IEnumerator ScaleUp()
  {
    while (transform.localScale.x < 1.25f)
    {
      transform.localScale += new Vector3(2 * Time.deltaTime, 2 * Time.deltaTime, 2 * Time.deltaTime);
      yield return null;
    }
  }

  IEnumerator YRotation()
  {
    float rotationIncrement = UnityEngine.Random.Range(-90, 90);
    float waitTime = UnityEngine.Random.Range(.5f, 2f);

    float rotated = 0;

    yield return new WaitForSeconds(waitTime);

    while (Mathf.Sign(rotationIncrement) > 0 ? rotationIncrement - rotated >= 0 : rotationIncrement - rotated <= 0)
    {
      float step = rotationIncrement * Time.deltaTime;
      rotated += step;

      transform.Rotate(new Vector3(0, step, 0));

      yield return null;
    }
    StartCoroutine(YRotation());
  }
}