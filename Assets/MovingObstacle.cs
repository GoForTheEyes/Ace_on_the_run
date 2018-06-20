using UnityEngine;

public class MovingObstacle : MonoBehaviour {

#pragma warning disable 0649
    [SerializeField] float speed;
    [SerializeField] float switchTime;
    float _timeSinceLastSwitch;
    Vector3 _direction;
    float _minY, _maxY;
#pragma warning restore

    // Use this for initialization
    void Start () {
        _timeSinceLastSwitch = 0f;
        _minY = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0.15f)).y; //Can't go lower than 15% of screen
        _maxY = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0.85f)).y; //Can't go lower than 85% of screen
        var _coinFlip = Random.Range(0,1);
        if (_coinFlip < 0.5f)
        {
            _direction = Vector3.up;
        }
        else
        {
            _direction = - Vector3.up;
        }
	}

    private void Update()
    {
        var _newPosition = transform.position + _direction * speed * Time.deltaTime;
        _newPosition.y = Mathf.Clamp(_newPosition.y, _minY, _maxY);
        transform.position = _newPosition;

        _timeSinceLastSwitch += Time.deltaTime;
        if (_timeSinceLastSwitch >= switchTime)
        {
            _timeSinceLastSwitch = 0f;
            Switch();
        }
    }

    void Switch()
    {
        _direction *= -1f; 
    }



}
