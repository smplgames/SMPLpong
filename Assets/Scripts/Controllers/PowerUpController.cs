using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(ObstacleGenerator))]
[RequireComponent(typeof(PowerUpStore))]
public class PowerUpController : MonoBehaviour
{
	public GameObject BallPrefab;
	public Transform PowerUpTransformParent;
	public float SpawnProbabilityPerSecond = 0.075f;
	public int MaxPowerUps = 2;
	public int MaxCollectionTime = 15;
	public AudioSource PowerUpAudio;

	private PowerUpStore powerUpStore;
	private ObstacleGenerator _obstacleGenerator;
	private PlayerController _playerController;

	private readonly List<PowerUp> _powerUpQueue = new List<PowerUp>();

	private readonly Random _random = new Random();

	private readonly List<ActivatedPowerUp> _activePowerUps = new List<ActivatedPowerUp>();

	private void Start()
	{
		powerUpStore = GetComponent<PowerUpStore>();
		_obstacleGenerator = GetComponent<ObstacleGenerator>();
		_playerController = GetComponent<PlayerController>();

		InvokeRepeating(nameof(SpawnPowerUp), 1.0f, 1.0f);
	}

	private void SpawnPowerUp()
	{
		CheckExpiredPowerUps();

		if (_activePowerUps.Count >= MaxPowerUps) return;
		if (_random.NextDouble() > SpawnProbabilityPerSecond) return;

		if (_powerUpQueue.Count == 0) GeneratePowerUpQueue();

		if (!GameOverController.GameOver)
		{
			var index = _random.Next(_powerUpQueue.Count);
			var powerUp = _powerUpQueue[index];
			_powerUpQueue.RemoveAt(index);

			GenerateAndAddPowerUp(powerUp);
		}
	}

	private void GenerateAndAddPowerUp(PowerUp powerUp)
	{
		var powerUpGameObject = new GameObject(powerUp.Name, typeof(SpriteRenderer), typeof(BoxCollider),
				typeof(SpriteColorCycle))
			{tag = "PowerUp"};
		powerUpGameObject.transform.parent = PowerUpTransformParent;
		powerUpGameObject.SetActive(false);

		powerUpGameObject.GetComponent<SpriteRenderer>().sprite = powerUp.Sprite;

		var boxCollider = powerUpGameObject.GetComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		boxCollider.size = new Vector3(5, 5, 1); // TODO: dynamically set this

		var activatedPowerUp = new ActivatedPowerUp(powerUpGameObject, Time.time, powerUp);
		_activePowerUps.Add(activatedPowerUp);

		_obstacleGenerator.SpawnObject(powerUpGameObject, false);

		powerUpGameObject.SetActive(true);
	}

	private void CheckExpiredPowerUps()
	{
		for (var i = 0;
			i < _activePowerUps.Count;
			i++)
			if (Time.time - _activePowerUps[i].CreationTime >= MaxCollectionTime)
				DestroyPowerUp(i--);
	}

	private void DestroyPowerUp(int i)
	{
		DestroyPowerUp(_activePowerUps[i]);
	}

	private void DestroyPowerUp(ActivatedPowerUp powerUp)
	{
		_obstacleGenerator.FreeObject(powerUp.GameObject, false);
		Destroy(powerUp.GameObject);
		_activePowerUps.Remove(powerUp);
	}

	private void GeneratePowerUpQueue()
	{
		foreach (var powerUp in powerUpStore.PowerUps)
			for (var i = 0;
				i < powerUp.ProbabilityCount;
				i++)
				_powerUpQueue.Add(powerUp);
	}

	public void CollectedPowerUp(GameObject lastPaddle, GameObject collectedGameObject)
	{
		PowerUpAudio.Play();
		var powerUp = _activePowerUps.Find(a => ReferenceEquals(a.GameObject, collectedGameObject));

		var dynMethod = GetType().GetMethod("Handle_" + powerUp.Type.Name,
			BindingFlags.NonPublic | BindingFlags.Instance);

		DestroyPowerUp(powerUp);
		if (dynMethod == null)
			Debug.LogError(
				"Method could not be found. Please create a method for: " + "Handle_" + powerUp.Type.Name);
		else
			StartCoroutine((IEnumerator) dynMethod.Invoke(this, new object[]
			{
				lastPaddle, powerUp
			}));
	}

	#region HandlePowerUps

	private IEnumerator Handle_Size_Modification(GameObject lastPaddle, ActivatedPowerUp activatedPower, float factor)
	{
		if (lastPaddle != null)
		{
			var currentLocalScale = lastPaddle.transform.localScale;
			currentLocalScale.x *= factor;
			lastPaddle.transform.localScale = currentLocalScale;

			yield return new WaitForSeconds(10);

			currentLocalScale = lastPaddle.transform.localScale;
			currentLocalScale.x /= factor;
			lastPaddle.transform.localScale = currentLocalScale;
		}

		yield return null;
	}

	private IEnumerator Handle_Enlarge(GameObject lastPaddle, ActivatedPowerUp activatedPower)
	{
		return Handle_Size_Modification(lastPaddle, activatedPower, 1.5f);
	}

	private IEnumerator Handle_Minimize(GameObject lastPaddle, ActivatedPowerUp activatedPower)
	{
		return Handle_Size_Modification(lastPaddle, activatedPower, 0.75f);
	}

	private bool _isRotating;

	private IEnumerator Handle_Rotate(GameObject lastPaddle, ActivatedPowerUp activatedPower)
	{
		if (!_isRotating)
		{
			_isRotating = true;
			var fromRotation = _playerController.Parent.transform.localRotation;
			var toRotation = fromRotation * Quaternion.Euler(0, 0, 90);

			const float maxTime = 1.5f;
			float currentTime = 0;
			while (currentTime < maxTime)
			{
				yield return new WaitForEndOfFrame();

				currentTime += Time.fixedDeltaTime;
				_playerController.Parent.transform.localRotation =
					Quaternion.Lerp(fromRotation, toRotation, currentTime / maxTime);
			}

			_isRotating = false;
		}
	}

	private IEnumerator Handle_Speed(GameObject lastPaddle, ActivatedPowerUp activatedPower)
	{
		Time.timeScale *= 1.5f;
		yield return new WaitForSeconds(10);
		Time.timeScale /= 1.5f;
	}

	private IEnumerator Handle_Ball(GameObject lastPaddle, ActivatedPowerUp activatedPower)
	{
		var newBall = Instantiate(BallPrefab);
		newBall.GetComponent<BallController>().OnlyLiveOnce = true;
		yield return null;
	}

	private IEnumerator Handle_Life(GameObject lastPaddle, ActivatedPowerUp activatedPower)
	{
		if (lastPaddle != null)
		{
			_playerController.GetPlayerAndTeam(lastPaddle.transform.parent.gameObject, out var team, out var player);
			team.Health += 1;
		}

		yield return null;
	}

	#endregion

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.CompareTag("Ball")) GetComponent<AudioSource>().Play();
	}
}

public struct ActivatedPowerUp
{
	public GameObject GameObject { get; }
	public float CreationTime { get; }
	public PowerUp Type { get; }

	public ActivatedPowerUp(GameObject gameObject, float creationTime, PowerUp type)
	{
		GameObject = gameObject;
		CreationTime = creationTime;
		Type = type;
	}
}