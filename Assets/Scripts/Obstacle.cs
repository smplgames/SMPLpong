using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int Lives;
    public Vector3 Pos;

    private bool _arrived, _dead;
    private Transform _trans;
    private Renderer _rend;
    private Color _color;
    public Color[] LiveColors;
    private bool _rotate;
    private float _rotationSpeed;
    public float RotationSpeedObstacle;
    public ObstacleGenerator Generator { get; set; }

    // Start is called before the first frame update
    public void LateStart()
    {
        _trans = gameObject.transform.parent;
        _trans.position = new Vector3(Pos.x, Pos.y, 100);
        _rend = GetComponentInChildren<Renderer>();
        _color = new Color(0, 0, 0, 0);
        _rend.material.color = new Color(0f, 0f, 0f, 0f);
        float ran = Random.Range(0, 1f);

        if (ran > 0.77f)
        {
            _rotate = true;
            _rotationSpeed = ran + Random.Range(0.2f, 0.8f) + RotationSpeedObstacle;
        }
        else if (ran > 0.37f)
        {
            _rotate = true;
            _rotationSpeed = -(ran + 0.33f + Random.Range(0.2f, 0.8f) + RotationSpeedObstacle);
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ball"))
        {
            Lives--;
            if (Lives == 0)
            {
                Pos = new Vector3(Pos.x, Pos.y, 100);
                _arrived = false;
                _dead = true;
                Generator.FreeObject(gameObject, true);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Mathf.Abs(_trans.position.z - Pos.z) < 0.1)
        {
            if (_dead)
            {
                Destroy(gameObject);
            }

            _arrived = true;
        }

        if (!_arrived)
        {
            if (_dead)
            {
                _rend.material.color = Color.Lerp(_rend.material.color, _color, 0.1f);
                _trans.localScale = Vector3.Lerp(_trans.localScale, new Vector3(0f, 0f, 0f), 0.1f);
            }


            _trans.position = Vector3.Lerp(_trans.position, Pos, 0.08f);
        }

        _rend.material.color = Color.Lerp(_rend.material.color, LiveColors[Mathf.Max(0, Lives) % LiveColors.Length], 0.1f);
        if (_rotate)
            _trans.Rotate(0f, 0f, _trans.rotation.z + _rotationSpeed);
    }
}