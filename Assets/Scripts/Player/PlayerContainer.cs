using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerContainer : MonoBehaviour
{
	public GameObject Paddle;
	//public Rigidbody PaddleRigidbody;
	public GameObject Wall;
    public Gamepad Gamepad { get; set; }

	public void Awake()
	{
		//PaddleRigidbody = Paddle.GetComponent<Rigidbody>();
	}
}
