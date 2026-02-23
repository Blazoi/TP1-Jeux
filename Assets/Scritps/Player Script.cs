using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
	[SerializeField] GameObject tailRotationPivot;
	[SerializeField] float regularTailSpeed = 1;

	
	float swimSpeed = 30;
	float rotationSpeed = 45;
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
		tailSpeed = _space.ReadValue<float>() == 1f ? fastTailSpeed : regularTailSpeed;

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

  void OnCollisionEnter(Collision collision)
  {
    // if (collision.gameObject.CompareTag("Ennemy"))
		// {
		// 	playerTouchedFish(collision.gameObject, GetComponent<Rigidbody>().linearVelocity);
		// }
  }
}
