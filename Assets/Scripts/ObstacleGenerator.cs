using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ObstacleGenerator : MonoBehaviour
{
	public Transform ObstacleParentTransform;
	public GameObject GameField;

	public List<GameObject> ObstaclePrefabs;

	public int MaxObstacleAmount;
	public readonly Dictionary<GameObject, Vector3> Obstacles = new Dictionary<GameObject, Vector3>();

	private int _obstacleCount = 0;

	private float _timer;
	public int MinLives, MaxLives;
	public float DistanceBetweenObjects;
	private float _width, _height;

    public float RotationSpeedObstacle;

	private bool _ballWait = true;

	private readonly Random _random = new Random();

	void Start()
	{
		_timer = 5;
		_width = GameField.transform.localScale.x * 10 * 0.8f;
		_height = GameField.transform.localScale.y * 10 * 0.8f;
	}

	public void SpawnObject(GameObject g, bool isObstacle)
	{
		var position = FindFreePosition();
		g.transform.position = position;
		Obstacles.Add(g, position);

		if (isObstacle) _obstacleCount++;
	}

	public void FreeObject(GameObject obstacle, bool isObstacle)
	{
		Obstacles.Remove(obstacle);
		if (isObstacle) _obstacleCount--;
	}

	private Vector3 FindFreePosition()
	{
		var counter = 0;
		while (true)
		{
			counter++;
			var pos = new Vector3((_width * 0.2f + UnityEngine.Random.Range(0f, _width * 0.6f)) - _width / 2,
				(_height * 0.2f + UnityEngine.Random.Range(0f, _height * 0.6f)) - _height / 2, 0f);
			if (IsFree(pos))
			{
				return pos;
			}

			if (counter > 500)
			{
				if (IsFree(pos + DistanceBetweenObjects * Vector3.right))
				{
					return pos + DistanceBetweenObjects * Vector3.right;
				}
				else if (IsFree(pos + DistanceBetweenObjects * Vector3.up))
				{
					return pos + DistanceBetweenObjects * Vector3.up;
				}
				else if (IsFree(pos - DistanceBetweenObjects * Vector3.right))
				{
					return pos - DistanceBetweenObjects * Vector3.right;
				}
				else if (IsFree(pos - DistanceBetweenObjects * Vector3.up))
				{
					return pos - DistanceBetweenObjects * Vector3.up;
				}
				else if (counter == 1000)
				{
					Debug.Log("No free space left");
					return pos;
				}
			}
		}
	}

	public void SphereTest(float radius)
	{
		if (Physics.SphereCast(new Ray(new Vector3(0, 0, 10), new Vector3(0, 0, -1)), radius + 1, out var hit, 20))
		{
			if (hit.transform.parent != null)
			{
				if (Obstacles.ContainsKey(hit.transform.parent.gameObject))
				{
					FreeObject(hit.transform.parent.gameObject,
						hit.transform.parent.gameObject.GetComponentInChildren<Obstacle>() != null);
					Destroy(hit.transform.parent.gameObject);
				}
			}
		}

		_ballWait = false;
	}

	public void AllowSpawn()
	{
		_ballWait = true;
	}

	private bool IsOnGameBoard(Vector3 vec)
	{
		if (vec.x < -_width / 2 || vec.x > _width / 2 || vec.y < -_height / 2 || vec.y > _height / 2)
			return false;
		return true;
	}

	private bool IsFree(Vector3 pos)
	{
		if (!IsOnGameBoard(pos)) return false;

		foreach (var position in Obstacles.Values)
		{
			if (Vector3.Distance(pos, position) < DistanceBetweenObjects)
			{
				return false;
			}
		}

		return true;
	}

	void Update()
	{
		if (GameOverController.GameOver) return;
		_timer -= Time.deltaTime;
		if (!_ballWait) return;
		if (_timer > 0) return;

		if (_obstacleCount < MaxObstacleAmount)
		{
			var obstacle = Instantiate(ObstaclePrefabs[_random.Next(ObstaclePrefabs.Count)], ObstacleParentTransform);
			obstacle.transform.Rotate(0f, 0f, _random.Next(361));

			var obstacleScript = obstacle.GetComponentInChildren<Obstacle>();
			obstacleScript.Generator = this;
            obstacleScript.RotationSpeedObstacle = RotationSpeedObstacle;
			obstacleScript.Lives = UnityEngine.Random.Range(MinLives, MaxLives);
			obstacleScript.LiveColors = ColorPallete.Instance.Colors;
			obstacleScript.LateStart();

			SpawnObject(obstacle, true);
		}

		_timer = (float) (_random.NextDouble() * 4 + 4);
	}
}