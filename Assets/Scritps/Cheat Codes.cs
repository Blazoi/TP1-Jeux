using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheatCodes : MonoBehaviour
{
	public event Action<int> addPoints;
	public event Action playShakeComboAnimation;
	public event Action playBlinkComboAnimation;
	public event Action deleteAllPairs;

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
			playShakeComboAnimation();
		}

		// Animations Combo
		if (Keyboard.current.oKey.wasPressedThisFrame)
		{
			playBlinkComboAnimation();
		}
		if (Keyboard.current.lKey.wasPressedThisFrame)
		{
			playShakeComboAnimation();
		}

		// Suppressions touts appâts et poissons
		if(Keyboard.current.numpad0Key.wasPressedThisFrame)
		{
			deleteAllPairs();
		}
	}
}
