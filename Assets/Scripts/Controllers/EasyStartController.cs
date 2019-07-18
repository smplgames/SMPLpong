using UnityEngine;

public class EasyStartController : MonoBehaviour
{
	private PowerUpController _powerUpController;

	public float ActionTime = 6.845f;


	private void Awake()
	{
		_powerUpController = GetComponent<PowerUpController>();

		if (enabled)
		{
			_powerUpController.enabled = false;
		}
	}

	private void Start()
	{
		Time.timeScale = 0.5f;
		Invoke(nameof(FunkyTime), ActionTime * Time.timeScale);
	}

	private void FunkyTime()
	{
		Time.timeScale = 1f;
		_powerUpController.enabled = true;
	}
}