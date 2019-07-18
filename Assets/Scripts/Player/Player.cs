using UnityEngine;
using UnityEngine.InputSystem;

public class Player
{
	public PlayerContainer Container { get; }

	public Color Color { get; }
    public PaddleInput PaddleInput { get; }
    public Gamepad Gamepad { get; }

	public Player(PlayerContainer container, Color color, PaddleInput paddleInput, Gamepad gamepad)
	{
		Container = container;
		Color = color;
        PaddleInput = paddleInput;
        Gamepad = gamepad;

        container.Paddle.GetComponent<Renderer>().material.color = color;
	}
}