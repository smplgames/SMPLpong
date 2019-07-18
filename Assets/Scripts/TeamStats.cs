using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamStats : MonoBehaviour
{
    public TMP_Text TeamName;
    public TMP_Text Health;
    public Image[] Colors;
    public int TeamIndex;
    private Team team;

    // Start is called before the first frame update
    void Start()
    {
        var teams = FindObjectOfType<PlayerController>().Teams;
        team = teams[TeamIndex];
        TeamName.text = team.Name;
        for (int i = 0; i < Colors.Length; i++)
        {
            var player = team.Players[i / (Colors.Length / team.Players.Length)];
            Colors[i].color = player.Color;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Health.text = team.Health.ToString();
    }
}
