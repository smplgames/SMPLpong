using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerController : MonoBehaviour
{
	public static float Speed;
	public int BaseHealth = 10;

	[Header("Player Generation")] public int PlayerCount = 4;

	public GameObject Parent;
	public GameObject PlayerPrefab;

	public Player[] Players { get; set; }
	public Team[] Teams { get; set; }

	private void Awake()
	{
		var playerColors = ColorPallete.Instance.Colors;
		PlayerContainer[] playerObjects = SpawnPlayer(PlayerPrefab, Parent, PlayerCount);
		if (MatchSettings.NextMatch is TwoPlayerMatchSettings match2p)
		{
			//TODO: individual control
			Players = new Player[4];
			if (match2p.Player1.Gamepad == null)
			{
				Players[0] = new Player(playerObjects[0], playerColors[0],
					new KeyboardInput(Keyboard.current.aKey, Keyboard.current.dKey,
						Keyboard.current.wKey, Keyboard.current.sKey), null);
				Players[1] = new Player(playerObjects[2], playerColors[0],
					new KeyboardInput(Keyboard.current.aKey, Keyboard.current.dKey,
						Keyboard.current.wKey, Keyboard.current.sKey), null);
			}
			else
			{
				Players[0] = new Player(playerObjects[0], playerColors[0],
					new StickInput(match2p.Player1.Gamepad.rightStick),
					match2p.Player1.Gamepad);
				Players[1] = new Player(playerObjects[2], playerColors[0],
					new StickInput(match2p.Player1.Gamepad.leftStick),
					match2p.Player1.Gamepad);
			}

			if (match2p.Player2.Gamepad == null)
			{
				Players[2] = new Player(playerObjects[1], playerColors[1],
					new KeyboardInput(Keyboard.current.leftArrowKey, Keyboard.current.rightArrowKey,
						Keyboard.current.upArrowKey, Keyboard.current.downArrowKey), null);
				Players[3] = new Player(playerObjects[3], playerColors[1],
					new KeyboardInput(Keyboard.current.leftArrowKey, Keyboard.current.rightArrowKey,
						Keyboard.current.upArrowKey, Keyboard.current.downArrowKey), null);
			}
			else
			{
				Players[2] = new Player(playerObjects[1], playerColors[1],
					new StickInput(match2p.Player2.Gamepad.rightStick),
					match2p.Player2.Gamepad);
				Players[3] = new Player(playerObjects[3], playerColors[1],
					new StickInput(match2p.Player2.Gamepad.leftStick),
					match2p.Player2.Gamepad);
			}

			Teams = new[]
			{
				new Team("Player 1", new[] {Players[0], Players[1]}),
				new Team("Player 2", new[] {Players[2], Players[3]}),
			};
		}
        else if (MatchSettings.NextMatch is ThreePlayerMatchSettings match3p)
        {
            Players = new[]
            {
                new Player(playerObjects[0], playerColors[0],
                    AbsMaxInput.fromTwoSticks(match3p.Player1.Gamepad.leftStick, match3p.Player1.Gamepad.rightStick),
                    match3p.Player1.Gamepad),
                new Player(playerObjects[2], playerColors[1],
                    AbsMaxInput.fromTwoSticks(match3p.Player2.Gamepad.leftStick, match3p.Player2.Gamepad.rightStick),
                    match3p.Player2.Gamepad),
                new Player(playerObjects[1], playerColors[2],
                    AbsMaxInput.fromTwoSticks(match3p.Player3.Gamepad.leftStick, match3p.Player3.Gamepad.rightStick),
                    match3p.Player3.Gamepad),
            };
            Teams = new[]
            {
                new Team("Player 1", new[] {Players[0]}),
                new Team("Player 2", new[] {Players[1]}),
                new Team("Player 3", new[] {Players[2]}),
            };
            playerObjects[3].Paddle.SetActive(false);
            playerObjects[3].Wall.GetComponent<BoxCollider>().isTrigger = false;
        }
        else if (MatchSettings.NextMatch is FourPlayerMatchSettings match4p)
		{
            Players = new[]
            {
                new Player(playerObjects[0], playerColors[0],
                    AbsMaxInput.fromTwoSticks(match4p.player1.Gamepad.leftStick, match4p.player1.Gamepad.rightStick),
                    match4p.player1.Gamepad),
                new Player(playerObjects[2], playerColors[1],
                    AbsMaxInput.fromTwoSticks(match4p.player2.Gamepad.leftStick, match4p.player2.Gamepad.rightStick),
                    match4p.player2.Gamepad),
                new Player(playerObjects[1], playerColors[2],
                    AbsMaxInput.fromTwoSticks(match4p.player3.Gamepad.leftStick, match4p.player3.Gamepad.rightStick),
                    match4p.player3.Gamepad),
                new Player(playerObjects[3], playerColors[3],
                    AbsMaxInput.fromTwoSticks(match4p.player4.Gamepad.leftStick, match4p.player4.Gamepad.rightStick),
                    match4p.player4.Gamepad),
            };
			switch (match4p.type)
			{
				case TeamSetting.TEAMS_2v2:
					Teams = new[]
					{
						new Team("Team 1", new[] {Players[0], Players[1]}),
						new Team("Team 2", new[] {Players[2], Players[3]}),
					};
					break;
				case TeamSetting.TEAMS_3v1:
					Teams = new[]
					{
						new Team("Majority", new[] {Players[0], Players[1], Players[2]}),
						new Team("Lone Wolf", new[] {Players[2]}),
					};
					break;
				case TeamSetting.FREE_FOR_ALL:
					Teams = new[]
					{
						new Team("Player 1", new[] {Players[0]}),
						new Team("Player 2", new[] {Players[1]}),
						new Team("Player 3", new[] {Players[2]}),
						new Team("Player 4", new[] {Players[3]}),
					};
					break;
			}
		}
		else
		{
			Players = new[]
			{
				new Player(playerObjects[0], Color.red,
					new KeyboardInput(Keyboard.current.leftArrowKey, Keyboard.current.rightArrowKey,
						Keyboard.current.upArrowKey, Keyboard.current.downArrowKey), null),
				new Player(playerObjects[2], Color.red, new KeyboardInput(Keyboard.current.leftArrowKey,
					Keyboard.current.rightArrowKey, Keyboard.current.upArrowKey, Keyboard.current.downArrowKey), null),
				new Player(playerObjects[1], Color.green, new KeyboardInput(Keyboard.current.aKey,
					Keyboard.current.dKey,
					Keyboard.current.wKey, Keyboard.current.sKey), null),
				new Player(playerObjects[3], Color.green, new KeyboardInput(Keyboard.current.aKey,
					Keyboard.current.dKey,
					Keyboard.current.wKey, Keyboard.current.sKey), null),
			};
			Teams = new[]
			{
				new Team("Player 1", new[] {Players[0], Players[1]}),
				new Team("Player 2", new[] {Players[2], Players[3]}),
			};
		}

		foreach (var team in Teams)
		{
			team.Health = BaseHealth;
		}
	}

	private void FixedUpdate()
	{
		if (Players == null) return;
		foreach (var player in Players)
		{
			var playerContainer = player.Container;
			var inputValue = player.PaddleInput.GetInput(playerContainer.transform.rotation.eulerAngles.z);

			var newPosition = playerContainer.Paddle.transform.position +
			                  playerContainer.Paddle.transform.right * Speed * Time.fixedDeltaTime *
			                  inputValue;

			var maxX = playerContainer.Wall.transform.localScale.x / 2.0f -
			           playerContainer.Paddle.transform.localScale.x / 2.0f -
			           playerContainer.Paddle.transform.localScale.y / 2.0f -
			           (Mathf.Abs(playerContainer.Wall.transform.localScale.x / 2.0f) -
			            Mathf.Abs(playerContainer.Paddle.transform.localPosition.y));

			playerContainer.Paddle.transform.position = newPosition;

			playerContainer.Paddle.transform.localPosition = new Vector3(
				Mathf.Clamp(playerContainer.Paddle.transform.localPosition.x, -maxX, maxX),
				playerContainer.Paddle.transform.localPosition.y, playerContainer.Paddle.transform.localPosition.z);
		}
	}

	private static PlayerContainer[] SpawnPlayer(GameObject playerPrefab, GameObject parent, int playerCount)
	{
		var playerObjects = new PlayerContainer[playerCount];
		for (var i = 0; i < playerCount; i++)
		{
			var currentPlayerObject = Instantiate(playerPrefab, parent.transform);
			currentPlayerObject.name = "Player " + (i + 1);
			var currentPlayerTransform = currentPlayerObject.transform;
			playerObjects[i] = currentPlayerObject.GetComponent<PlayerContainer>();
			currentPlayerTransform.eulerAngles =
				new Vector3(currentPlayerTransform.eulerAngles.x, currentPlayerTransform.eulerAngles.y, 90 * i);
		}

		return playerObjects;
	}

	public int TeamsAliveCount()
	{
		return Teams.Count(team => team.Health > 0);
	}

	public Team GetFirstTeamAlive()
	{
		return Teams.FirstOrDefault(team => team.Health > 0);
	}

	public void GetPlayerAndTeam(GameObject playerObject, out Team team, out Player player)
	{
		foreach (var currentTeam in Teams)
		{
			foreach (var currentPlayer in currentTeam.Players)
			{
				if (ReferenceEquals(playerObject, currentPlayer.Container.gameObject))
				{
					team = currentTeam;
					player = currentPlayer;
					return;
				}
			}
		}

		team = null;
		player = null;
	}
}