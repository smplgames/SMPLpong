using UnityEngine;

public class TeamStatSelector : MonoBehaviour
{
    public GameObject TwoTeamHud;
    public GameObject FourTeamHud;
    void Start()
    {
        var playerController = FindObjectOfType<PlayerController>();
        if (playerController.Teams.Length >= 4)
        {
            FourTeamHud.SetActive(true);
        } else
        {
            TwoTeamHud.SetActive(true);
        }
    }
}
