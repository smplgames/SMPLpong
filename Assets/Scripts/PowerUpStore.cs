using System;
using System.Collections.Generic;
using UnityEngine;


public class PowerUpStore : MonoBehaviour
{
	public List<PowerUp> PowerUps;
}


[Serializable]
public class PowerUp
{
	public string Name;
	public Sprite Sprite;

	public int ProbabilityCount = 5;
}