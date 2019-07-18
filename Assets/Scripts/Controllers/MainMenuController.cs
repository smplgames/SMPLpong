using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Image = UnityEngine.UI.Image;

public class MainMenuController : MonoBehaviour
{
	[Serializable]
	public class PlayerHolder
	{
		public Image Image;
		public TextMeshProUGUI Text;

		public Gamepad Gamepad { get; set; }
		public bool IsKeyboard => Gamepad == null;

		public bool Ready { get; private set; }

		public bool Joined { get; private set; }

		public void Join(Gamepad gamepad)
		{
			Text.text = "Press A to ready up";
			Image.gameObject.GetComponent<Animation>().Play();

			Gamepad = gamepad;
			Ready = false;
			Joined = true;
		}

		public void Join(Sprite sprite)
		{
			Text.text = "Press A to ready up";
			Image.sprite = sprite;
			Image.gameObject.GetComponent<Animation>().Play();


			Ready = false;
			Joined = true;
		}

		public void Exit()
		{
			Text.text = "Press A to join";
			Ready = false;
			Joined = false;
			
			Gamepad = null;
		}

		public void NotReady()
		{
			Text.text = "Press A to ready up";
			Ready = false;
			Joined = true;
		}

		public void SetReady()
		{
			Text.text = "Ready";
			Ready = true;
			Joined = true;
		}
	}

	public Sprite KeyBoardSprite;
	public Image fade;
	
	public TextMeshProUGUI GameModeText;
	public int currentGameMode;
	public GameObject MainPanel, CreditsPanel;
	
	public PlayerHolder[] Players;
	public int MaxTeams = 4;

	private int playerMode = 4;
	private int playersReady = 0;

	void Start()
	{
		InputSystem.onEvent += InputSystem_onEvent;
		
		StartCoroutine(FadeSceneManager.FadeInScene(fade));
	}

	private void OnDestroy()
	{
		InputSystem.onEvent -= InputSystem_onEvent;
	}

	private void Update()
	{
        var ph0 = Players[0].Text.gameObject.transform.parent.gameObject;
        var ph1 = Players[1].Text.gameObject.transform.parent.gameObject;
        var ph2 = Players[2].Text.gameObject.transform.parent.gameObject;
        var ph3 = Players[3].Text.gameObject.transform.parent.gameObject;

        if (Gamepad.all.Count < 4 && playerMode == 4)
		{
			ph3.SetActive(false);

			ph0.GetComponent<RectTransform>().anchoredPosition += new Vector2(160, 0);
			ph1.GetComponent<RectTransform>().anchoredPosition += new Vector2(160, 0);
            ph2.GetComponent<RectTransform>().anchoredPosition += new Vector2(160, 0);

            playerMode = 3;
        }
        if (Gamepad.all.Count < 3 && playerMode == 3)
        {
            ph2.SetActive(false);

            ph0.GetComponent<RectTransform>().anchoredPosition += new Vector2(160, 0);
            ph1.GetComponent<RectTransform>().anchoredPosition += new Vector2(160, 0);
            ph2.GetComponent<RectTransform>().anchoredPosition += new Vector2(160, 0);

            playerMode = 2;
        }
        if (Gamepad.all.Count >= 3 && playerMode == 2)
        {
            ph2.SetActive(true);

            ph0.GetComponent<RectTransform>().anchoredPosition -= new Vector2(160, 0);
            ph1.GetComponent<RectTransform>().anchoredPosition -= new Vector2(160, 0);
            ph2.GetComponent<RectTransform>().anchoredPosition -= new Vector2(160, 0);

            playerMode = 3;
        }
		if (Gamepad.all.Count >= 4 && playerMode == 3)
		{
			ph3.SetActive(true);

            ph0.GetComponent<RectTransform>().anchoredPosition -= new Vector2(160, 0);
            ph1.GetComponent<RectTransform>().anchoredPosition -= new Vector2(160, 0);
            ph2.GetComponent<RectTransform>().anchoredPosition -= new Vector2(160, 0);

            playerMode = 4;
		}
		
		ChangeGameMode(0);
	}

	private void InputSystem_onEvent(InputEventPtr eventPtr)
	{
		if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
			return;

		var gamepad = InputSystem.GetDeviceById(eventPtr.deviceId) as Gamepad;
		var keyboard = InputSystem.GetDeviceById(eventPtr.deviceId) as Keyboard;
		if (gamepad == null && keyboard == null) return;

		if (keyboard != null)
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				var index = FindFreeSlot();
				if (index == -1) return;

				Players[index].Join(KeyBoardSprite);
				Players[index].SetReady();
				ChangeReadyPlayers(1);
			}

			return;
		}

		var ph = GetPlayerHolder(gamepad);
		if (ph == null && gamepad.aButton.ReadValue() == 1)
		{
			var index = FindFreeSlot();

			Players[index].Join(gamepad);
			return;
		}
		
		if (ph == null) return;
		if (!ph.Ready)
		{
			if (gamepad.rightShoulder.ReadValue() == 1)
			{
				ChangeGameMode(1);
			}
			else if (gamepad.leftShoulder.ReadValue() == 1)
			{
				ChangeGameMode(-1);
			}
		}


		if (gamepad.aButton.ReadValue() == 1)
		{
			if (!ph.Ready)
			{
				ph.SetReady();
				ChangeReadyPlayers(1);
			}
		}

		if (gamepad.bButton.ReadValue() == 1)
		{
			if (ph.Ready)
			{
				ph.NotReady();
				ChangeReadyPlayers(-1);
			}
			else if (ph.Joined)
			{
				ph.Exit();
			}
		}
	}

	public void ChangeGameMode(int nr)
	{
		if (playerMode != 4)
		{
			GameModeText.text = "";
			ChangeControllerColorsTeam((TeamSetting) currentGameMode);
			
			return;
		}
		
		currentGameMode += nr;

		if (currentGameMode < 0) currentGameMode = 3;
		if (currentGameMode >= 4) currentGameMode = 0;

		var gameMode = (TeamSetting) currentGameMode;
		switch (gameMode)
		{
			case TeamSetting.TEAMS_2v2:
				GameModeText.text = "2 vs 2";
				break;
			case TeamSetting.TEAMS_3v1:
				GameModeText.text = "3 vs 1";
				break;
			case TeamSetting.FREE_FOR_ALL:
				GameModeText.text = "Free for all";
				break;
		}

		ChangeControllerColorsTeam(gameMode);
	}

	public void ChangeControllerColorsTeam(TeamSetting mode)
	{
		for (int i = 0; i < Players.Length; i++)
		{
			Players[i].Image.color = Players[i].Joined ? ColorPallete.Instance.Colors[i] : Color.white;
		}
	}
	
	private void ChangeReadyPlayers(int nr)
	{
		playersReady += nr;

		if (playersReady == playerMode)
		{
			MatchSettings ms;
			if (playerMode == 2)
			{
				ms = new TwoPlayerMatchSettings(
					new PlayerSettings(Players[0].Gamepad),
					new PlayerSettings(Players[1].Gamepad)
				);
			}
            else if (playerMode == 3)
            {
                ms = new ThreePlayerMatchSettings(
                     new PlayerSettings(Players[0].Gamepad),
                     new PlayerSettings(Players[1].Gamepad),
                     new PlayerSettings(Players[2].Gamepad)
                );
            }
			else
			{
				ms = new FourPlayerMatchSettings(
					(TeamSetting) currentGameMode,
					new PlayerSettings(Players[0].Gamepad), 
					new PlayerSettings(Players[1].Gamepad), 
					new PlayerSettings(Players[2].Gamepad), 
					new PlayerSettings(Players[3].Gamepad)
				);
			}
			
			MatchSettings.NextMatch = ms;

			StartCoroutine(FadeSceneManager.FadeOutScene(fade, "GameScene"));
		}
	}

	private PlayerHolder GetPlayerHolder(Gamepad gamepad)
	{
		for (int i = 0; i < Players.Length; i++)
		{
			if (Players[i].Gamepad?.id == gamepad.id) return Players[i];
		}

		return null;
	}

	private int FindFreeSlot()
	{
		for (int i = 0; i < Players.Length; i++)
		{
			if (!Players[i].Joined) return i;
		}

		return -1;
	}

	public void TogglePanels()
	{
		MainPanel.SetActive(!MainPanel.activeSelf);
		CreditsPanel.SetActive(!CreditsPanel.activeSelf);
	}
}
