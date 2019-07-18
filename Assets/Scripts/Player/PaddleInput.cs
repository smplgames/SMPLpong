using UnityEngine;
using UnityEngine.InputSystem.Controls;

public abstract class PaddleInput
{
	public abstract float GetInput(float containerRotation);
}

public class StickInput : PaddleInput
{
	public StickControl Stick { get; }

	public StickInput(StickControl stick)
	{
		Stick = stick;
	}

	public override float GetInput(float containerRotation)
	{
		return Stick.ReadValue().Rotate(-containerRotation).x;
	}
}

public class KeyboardInput : PaddleInput
{
	protected readonly KeyControl Left, Right;
	protected readonly KeyControl Up, Down;

	public KeyboardInput(KeyControl left, KeyControl right, KeyControl up, KeyControl down)
	{
		Left = left;
		Right = right;
		Up = up;
		Down = down;
	}

	public override float GetInput(float containerRotation)
	{
		var x = Left.isPressed ? -1.0f : 0.0f;
		if (Right.isPressed) x = 1.0f;
		var y = Down.isPressed ? -1.0f : 0.0f;
		if (Up.isPressed) y = 1.0f;

		return new Vector2(x, y).Rotate(-containerRotation).x;
	}
}

public class AbsMaxInput : PaddleInput
{
	public static AbsMaxInput fromTwoSticks(StickControl stick1, StickControl stick2)
	{
		return new AbsMaxInput(new StickInput(stick1), new StickInput(stick2));
	}

	private PaddleInput[] Inputs { get; }

	public AbsMaxInput(params PaddleInput[] inputs)
	{
		Inputs = inputs;
	}

	public override float GetInput(float containerRotation)
	{
		var max = 0.0f;
		foreach (var input in Inputs)
		{
			var cur = input.GetInput(containerRotation);
			if (Mathf.Abs(cur) > Mathf.Abs(max)) max = cur;
		}

		return max;
	}
}