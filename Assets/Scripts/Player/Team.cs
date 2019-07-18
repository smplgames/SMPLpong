using System;

public class Team
{
	public Player[] Players { get; }

	private int _health;

	public int Health
	{
		get => _health;
		set => _health = Math.Max(0, value);
	}

	public string Name { get;  }

	public Team(string name, Player[] players)
	{
		Name = name;
		Players = players;
	}
}