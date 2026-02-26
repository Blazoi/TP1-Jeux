using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
  [SerializeField] GameObject tailPivot;
  [SerializeField] float regularTailSpeed = 90;

  [SerializeField] GameObject lFin;
  [SerializeField] GameObject rFin;

  [SerializeField] float swimSpeed = 30;
  [SerializeField] float rotationSpeed = 45;
  float tailSpeed;
  float fastTailSpeed;

  bool tailDirection = true;

  InputAction _move;
  InputAction _space;
  void Start()
  {
    _move = InputSystem.actions.FindAction("Move");
    _space = InputSystem.actions.FindAction("Jump");

    fastTailSpeed = regularTailSpeed * 2.5f;
  }

  void Update()
  {
		// Animations
    // Queue
    tailSpeed = _space.ReadValue<float>() == 1f ? fastTailSpeed : regularTailSpeed;
    tailSpeed *= Time.deltaTime;

    float tailYRotation = tailPivot.transform.localEulerAngles.y;

    if (tailDirection)
    {
      tailPivot.transform.localEulerAngles += new Vector3(0, tailSpeed, 0);
      if (tailYRotation >= 20 && tailYRotation < 30)
      {
        tailDirection = false;
      }
    }
    else
    {
      tailPivot.transform.localEulerAngles -= new Vector3(0, tailSpeed, 0);
      if (tailYRotation <= 340 && tailYRotation > 330)
      {
        tailDirection = true;
      }
    }

    // Nageoires
    if (tailDirection)
    {
	    lFin.transform.localEulerAngles += new Vector3(0, tailSpeed/4, 0);
	    rFin.transform.localEulerAngles -= new Vector3(0, tailSpeed/4, 0);
    }
    else
    {
	    lFin.transform.localEulerAngles -= new Vector3(0, tailSpeed/4, 0);
	    rFin.transform.localEulerAngles += new Vector3(0, tailSpeed/4, 0);
    }
  }

  void FixedUpdate()
  {
    Vector2 moveInput = _move.ReadValue<Vector2>();
    float spaceInput = _space.ReadValue<float>();

    Rigidbody selfRigidbody = GetComponent<Rigidbody>();

    // Mouvement
    selfRigidbody.AddRelativeForce(new Vector3(0, 0, spaceInput * swimSpeed));
    selfRigidbody.AddRelativeForce(new Vector3(0, moveInput.y * swimSpeed, 0));

    // Rotation
    selfRigidbody.AddRelativeTorque(new Vector3(0, moveInput.x * rotationSpeed * Time.fixedDeltaTime, 0));
  }
}
