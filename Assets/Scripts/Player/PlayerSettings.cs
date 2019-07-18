using UnityEngine.InputSystem;

public class PlayerSettings
{
	public Gamepad Gamepad { get; }
	public Keyboard Keyboard { get; }

	public PlayerSettings(Gamepad gamepad)
	{
		Gamepad = gamepad;
		Keyboard = null;
	}

	public PlayerSettings(Keyboard keyboard)
	{
		Gamepad = null;
		Keyboard = keyboard;
	}
}
