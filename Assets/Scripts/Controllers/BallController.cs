using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class BallController : MonoBehaviour
{
	private PowerUpController _powerUpController;
	private PlayerController _playerController;
	private ObstacleGenerator _obstacleGenerator;
	private AudioSource _audioSource;
	public AudioClip WallSound;

	public float WaitTime = 0.75f;

	public bool OnlyLiveOnce { get; set; }

	public static float Speed;
	public float MaxBounceDegree;
	private new Rigidbody rigidbody;
	public Mesh CubeM, BallM;
	private State ballstate;
	public float initialDirectionMaxRandomDegrees;
	public float bounceDirectionMaxRandomDegrees;
	private GameObject _lastPaddle;
	public GameObject effect;
	private RumbleController rumbleController;
	public GameObject sparklingEffect;
    private ShakeableTransform cameraShake;

	public enum State
	{
		Ball,
		Cube
	};

	// Start is called before the first frame update
	private void Start()
	{
		var gameController = GameObject.FindGameObjectWithTag("GameController");
		_playerController = gameController.GetComponent<PlayerController>();
		_powerUpController = gameController.GetComponent<PowerUpController>();
		_obstacleGenerator = gameController.GetComponent<ObstacleGenerator>();
		_audioSource = GetComponent<AudioSource>();
		rigidbody = gameObject.GetComponent<Rigidbody>();
		rumbleController = GameObject.FindObjectOfType<RumbleController>();
        cameraShake = Camera.main.GetComponent<ShakeableTransform>();

		Reset();
		Invoke(nameof(StartBall), WaitTime);
	}

	private void Reset()
	{
		_obstacleGenerator.SphereTest(gameObject.transform.localScale.x);
		_lastPaddle = null;
		transform.position = Vector3.zero;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
	}

	private void StartBall()
	{
		_obstacleGenerator.AllowSpawn();
		var players = GameObject.FindGameObjectsWithTag("Player");
		var targetPlayer = players[Random.Range(0, players.Length)];
		var direction = (targetPlayer.transform.position - transform.position).normalized;
		direction = ((Vector2) direction).Rotate(Random.Range(-initialDirectionMaxRandomDegrees,
			initialDirectionMaxRandomDegrees));
		rigidbody.velocity = direction * Speed;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.CompareTag("Wall"))
		{
			_audioSource.PlayOneShot(WallSound, 0.7F);
			_playerController.GetPlayerAndTeam(collider.transform.parent.gameObject, out var team, out var player);
            cameraShake.InduceStress(0.2f);

			if (player == null || team == null)
			{
				Debug.LogError("Player or team not found for: " + collider.transform.parent.gameObject);
				return;
			}

			team.Health -= 1;
			if (team.Health <= 0)
			{
				player.Container.Wall.GetComponent<BoxCollider>().isTrigger = false;
				
				Debug.Log("Team " + team.Name + " has lost");

				if (_playerController.TeamsAliveCount() == 1)
				{
					var winner = _playerController.GetFirstTeamAlive();
					
					GameOverController.GameOver = true;
					GameOverController.WinnerTeam = winner.Name;
                    GameOverController.WinnerTeamHealth = winner.Health;
					GameOverController.WinningPlayers = winner.Players;
					
					gameObject.SetActive(false);
				}
				
				
				foreach (var loser in team.Players)
				{
					loser.Container.Paddle.SetActive(false);
				}
			}

			GameObject g = Instantiate(effect);
			g.transform.position = transform.position;
			
			Debug.Log(_playerController.TeamsAliveCount());
			if (OnlyLiveOnce)
			{
				Destroy(gameObject);
			} else if (_playerController.TeamsAliveCount() > 1)
			{
				Reset();
				Invoke(nameof(StartBall), WaitTime);
			}
		}
		else if (collider.CompareTag("PowerUp"))
		{
			_powerUpController.CollectedPowerUp(_lastPaddle, collider.gameObject);
		}
	}

	private void Update()
	{
		rigidbody.velocity = rigidbody.velocity.normalized * Speed;
	}

	private void OnCollisionEnter(Collision collision)
	{
		_audioSource.Play();

		if (collision.collider.CompareTag("Player"))
		{
			var contact = collision.GetContact(0);
			var distanceFromCenter =
				Vector3.Cross(contact.normal, contact.point - contact.otherCollider.transform.position).z;
			var bounceAngle =
				MaxBounceDegree * (2 * distanceFromCenter / contact.otherCollider.transform.localScale.x) +
				Random.Range(-bounceDirectionMaxRandomDegrees, bounceDirectionMaxRandomDegrees);
			rigidbody.velocity = ((Vector2) contact.normal).Rotate(bounceAngle);
			_lastPaddle = collision.gameObject;
			GameObject g = Instantiate(sparklingEffect);
			g.transform.position = collision.GetContact(0).point;
			g.transform.Rotate(0f, bounceAngle, 0f);

			_playerController.GetPlayerAndTeam(collision.collider.transform.parent.gameObject, out var team,
				out var player);
			rumbleController.AddRumble(player.Gamepad, 1f, 0.05f);
		}

		rumbleController.AddRumbleForAll(0.5f, 0.05f);
        cameraShake.InduceStress(0.08f);

		rigidbody.velocity = rigidbody.velocity.normalized * Speed;

		GetComponent<AudioSource>().Play();

		// timeScaleController.Freeze(Time.fixedDeltaTime * 2);
	}

	

	public void SetState(State state)
	{
		ballstate = state;

		if (state == State.Ball)
		{
			GetComponent<MeshFilter>().mesh = BallM;
			GetComponent<BoxCollider>().enabled = false;
			GetComponent<SphereCollider>().enabled = true;
		}
		else if (state == State.Cube)
		{
			GetComponent<MeshFilter>().mesh = CubeM;
			GetComponent<SphereCollider>().enabled = false;
			GetComponent<BoxCollider>().enabled = true;
		}
		else
		{
			Debug.LogError("Player or team not found for: " + GetComponent<Collider>().transform.parent.gameObject);
		}
	}
}