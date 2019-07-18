public abstract class MatchSettings
{
	public static MatchSettings NextMatch;
}

public class TwoPlayerMatchSettings : MatchSettings
{
	public PlayerSettings Player1 { get; }
	public PlayerSettings Player2 { get; }

	public TwoPlayerMatchSettings(PlayerSettings player1, PlayerSettings player2)
	{
		Player1 = player1;
		Player2 = player2;
	}
}

public class ThreePlayerMatchSettings : MatchSettings
{
    public PlayerSettings Player1 { get; }
    public PlayerSettings Player2 { get; }
    public PlayerSettings Player3 { get; }

    public ThreePlayerMatchSettings(PlayerSettings player1, PlayerSettings player2, PlayerSettings player3)
    {
        Player1 = player1;
        Player2 = player2;
        Player3 = player3;
    }
}

public enum TeamSetting
{
	TEAMS_2v2,
	TEAMS_3v1,
	FREE_FOR_ALL,
}

public class FourPlayerMatchSettings : MatchSettings
{
	public TeamSetting type { get; }

	public PlayerSettings player1 { get; }
	public PlayerSettings player2 { get; }
	public PlayerSettings player3 { get; }
	public PlayerSettings player4 { get; }

	public FourPlayerMatchSettings(TeamSetting type, PlayerSettings player1, PlayerSettings player2,
		PlayerSettings player3, PlayerSettings player4)
	{
		this.type = type;
		this.player1 = player1;
		this.player2 = player2;
		this.player3 = player3;
		this.player4 = player4;
	}
}