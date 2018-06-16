using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    [SerializeField] float _speed;

    Engine _myEngine;
    Rigidbody2D _myBody;
    float _minY, _maxY, _height;

    // Use this for initialization
    protected virtual void Start () {
        _minY = Camera.main.ViewportToWorldPoint(Vector2.zero).y;
        _maxY = Camera.main.ViewportToWorldPoint(new Vector2(0f, 1f)).y;
        _myEngine = GetComponent<Engine>();
        _myBody = GetComponent<Rigidbody2D>();
        _height = GetHeight();
    }

    protected virtual float GetHeight()
    {
        return GetComponent<BoxCollider2D>().size.y / 2 * Mathf.Abs(transform.lossyScale.x);
    }

	// Update is called once per frame
	protected virtual void FixedUpdate () {

        MovementLogic();
	}

    protected virtual void MovementLogic()
    {
        Vector3 _direction = -transform.right * Mathf.Sign(transform.localScale.x);
        var newPosition = transform.position + _direction * _speed * Time.fixedDeltaTime;
        newPosition.y = Mathf.Clamp(newPosition.y, _minY + _height, _maxY - _height);
        _myBody.MovePosition(newPosition);
    }

}
