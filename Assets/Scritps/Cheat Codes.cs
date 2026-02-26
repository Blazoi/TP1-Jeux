using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheatCodes : MonoBehaviour
{
	public event Action<float> addPoints;
	public event Action<int> playComboAnimation;
	public event Action deleteAllPairs;
	public event Action<int> testSetup;

	void Update()
	{
		// +1 ou +4 points avec animation
		if (Keyboard.current.uKey.wasPressedThisFrame)
		{
			addPoints(1);
		}
		if (Keyboard.current.iKey.wasPressedThisFrame)
		{
			addPoints(2);
			playComboAnimation(0);
		}

		// Animations Combo
		if (Keyboard.current.oKey.wasPressedThisFrame)
		{
			playComboAnimation(1);
		}
		if (Keyboard.current.lKey.wasPressedThisFrame)
		{
			playComboAnimation(0);
		}

		// Suppressions touts appâts et poissons
		if(Keyboard.current.digit0Key.wasPressedThisFrame)
		{
			deleteAllPairs();
		}
		// Setup Tests
		if(Keyboard.current.digit1Key.wasPressedThisFrame)
		{
			testSetup(1);
		}
		if(Keyboard.current.digit2Key.wasPressedThisFrame)
		{
			testSetup(2);
		}
	}
}
