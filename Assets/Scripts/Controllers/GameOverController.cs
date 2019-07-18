using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class GameOverController : MonoBehaviour
{
    public static bool GameOver;
    public static string WinnerTeam = "";
    public static int WinnerTeamHealth;
    public static Player[] WinningPlayers;
    
    public Image FadeImage;
    
    public GameObject GameOverUI;
    public List<Sprite> GameOverImages;
    public Image GameOverImg;
    
    public TMP_Text WinnerTeamNameText;
    public TMP_Text WinnerTeamHealthText;
    public AudioSource WinSound;

    private bool _homeScene;
    private bool _firstGameOverFrame = true;
    
    void Start()
    {
		GameOverUI.SetActive(false);
        GameOver = false;
        
        StartCoroutine(FadeSceneManager.FadeInScene(FadeImage));
    }
    
    void Update()
    {
        if (GameOver)
        {
            if (_firstGameOverFrame)
            {
                //Hier wirds nur einmal aufgerufen
                GameOverUI.SetActive(true);
                GameOverUI.GetComponent<Animation>().Play("gameOverUi");
                _firstGameOverFrame = false;
                int i = (int) Random.Range(0f, GameOverImages.Count-1);
                GameOverImg.sprite = GameOverImages[i];
                WinnerTeamNameText.text = WinnerTeam;
                WinnerTeamHealthText.text = WinnerTeamHealth.ToString();
                PlayWinSound();
                StartCoroutine(StartLerping());
            }

            if (Gamepad.current?.startButton.isPressed == true || Keyboard.current?.enterKey.isPressed == true)
            {
                StartCoroutine(FadeSceneManager.FadeOutScene(FadeImage, "GameScene"));
                Debug.Log("Replay");
            }
            else if (Gamepad.current?.selectButton.isPressed == true || Keyboard.current?.backspaceKey.isPressed == true)
            {
                StartCoroutine(FadeSceneManager.FadeOutScene(FadeImage, "MainMenu"));
                Debug.Log("Home");
            }
        }
    }

    private IEnumerator StartLerping() {
        for (int i = 0; true; i++) {
            float progress = 0.0f;
            int j = i % WinningPlayers.Length; 

            Color currentColor = WinningPlayers[j].Color;
            Color newColor = WinningPlayers[(j + 1) % WinningPlayers.Length].Color;
            while (progress <= 1.0f) {
                progress += Time.deltaTime / 0.5f;
                WinnerTeamNameText.color = Color.Lerp(currentColor, newColor, progress);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void PlayWinSound() {
        WinSound.Play();
    }
}
