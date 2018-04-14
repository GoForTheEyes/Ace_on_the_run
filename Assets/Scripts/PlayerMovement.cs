using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] float playerSpeed;

    public enum VerticalMovement { MovingToPoint, CompletingMovement , NotMoving };




    Rigidbody2D _myBody;
    BoxCollider2D _myCollider;

    float _targetHeight;
    float _currentHeight;
    float _yDistanceInitial;
    float _turningAngle = 5f;
    float _maxTurnAngle = 60f;
    Quaternion _targetRotation;
    float _minY, _maxY, _height;
    VerticalMovement verticalMovement;
    

    private void Start()
    {
        _myBody = GetComponent<Rigidbody2D>();
        _myBody.gravityScale = 0;
        _minY = Camera.main.ScreenToWorldPoint(Vector2.zero).y;
        _maxY = Camera.main.ScreenToWorldPoint(new Vector2(0f, Screen.height)).y;
        _myCollider = GetComponent<BoxCollider2D>();
        _height = _myCollider.size.y;

        verticalMovement = VerticalMovement.NotMoving;
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _currentHeight = transform.position.y;
            _targetHeight = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
            _yDistanceInitial = _targetHeight - _currentHeight;
            _targetRotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Sign(_yDistanceInitial) * _maxTurnAngle));
            verticalMovement = VerticalMovement.MovingToPoint;
        }
    }


    void FixedUpdate () {
        if (verticalMovement == VerticalMovement.MovingToPoint)
        {
            _currentHeight = transform.position.y;
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.fixedDeltaTime * _turningAngle);

            if (Mathf.Abs(_currentHeight - _targetHeight)<= 0.25f)
            {
                verticalMovement = VerticalMovement.CompletingMovement;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.fixedDeltaTime * _turningAngle);
            }
        }
        else if (verticalMovement == VerticalMovement.CompletingMovement)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.fixedDeltaTime * _turningAngle);

            if (transform.rotation.z == 0)
            {
                verticalMovement = VerticalMovement.NotMoving;
            }
        }


        Vector3 _direction = transform.right;
        var newPosition = transform.position + _direction * playerSpeed * Time.fixedDeltaTime;
        newPosition.y = Mathf.Clamp(newPosition.y, _minY + _height * 4f, _maxY - _height *4f);
        _myBody.MovePosition(newPosition);

	}


}
