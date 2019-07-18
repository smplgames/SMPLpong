using UnityEngine;

public class ColorPallete : MonoBehaviour
{
	public static ColorPallete Instance;

	public Color[] Colors;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}
		else if (!ReferenceEquals(this, Instance))
		{
			Destroy(gameObject);
		}
	}
}