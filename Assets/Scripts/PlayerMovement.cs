using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] float playerSpeed = 1.5f;

    public enum VerticalMovement { MovingToPoint, CompletingMovement , NotMoving };

    public enum ControlMode { Arcade, Flappy, Static };


    Rigidbody2D _myBody;
    BoxCollider2D _myCollider;

    float _targetHeight;
    float _currentHeight;
    float _yDistanceInitial;
    float _turningAngle = 5f;
    float _maxTurnAngle = 60f;
    float _distanceToStopVerticalMovement = 0.1f;
    Quaternion _targetRotation;
    float _minY, _maxY, _height;
    VerticalMovement _verticalMovement;
    ControlMode _controlMode;
    

    private void Start()
    {
        _myBody = GetComponent<Rigidbody2D>();
        _myCollider = GetComponent<BoxCollider2D>();
        _minY = Camera.main.ScreenToWorldPoint(Vector2.zero).y;
        _maxY = Camera.main.ScreenToWorldPoint(new Vector2(0f, Screen.height)).y;
        _height = _myCollider.size.y;

        ChangeToArcadeMode();
        
    }


    void ChangeToArcadeMode()
    {
        _controlMode = ControlMode.Arcade;
        _verticalMovement = VerticalMovement.NotMoving;
        _myBody.gravityScale = 0;
        StartCoroutine(HandleArcadeMovementEveryFixedUpdate());
    }

    void ChangeToFlappyMode()
    {
        _myBody.gravityScale = 1;
    }

    private void Update()
    {
        if (_controlMode == ControlMode.Arcade)
        {
            ArcadeMovement();
        }
        else if (_controlMode == ControlMode.Flappy)
        {
            //FlappyMovement
        }
    }


    void ArcadeMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _currentHeight = transform.position.y;
            _targetHeight = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
            _yDistanceInitial = _targetHeight - _currentHeight;
            _targetRotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Sign(_yDistanceInitial) * _maxTurnAngle));
            _verticalMovement = VerticalMovement.MovingToPoint;
        }
        
    }

    IEnumerator HandleArcadeMovementEveryFixedUpdate()
    {
        while (_controlMode == ControlMode.Arcade)
        {
            if (_verticalMovement != VerticalMovement.NotMoving) HandleRotation();
            HandleHorizontalMovement();
            yield return new WaitForFixedUpdate();
        }
    }

    void HandleHorizontalMovement()
    {
        Vector3 _direction = transform.right;
        var newPosition = transform.position + _direction * playerSpeed * Time.fixedDeltaTime;
        Debug.Log(newPosition);
        newPosition.y = Mathf.Clamp(newPosition.y, _minY + _height * 4f, _maxY - _height * 4f);
        _myBody.MovePosition(newPosition);

    }

    void HandleRotation()
    {
        if (_verticalMovement == VerticalMovement.MovingToPoint)
        {
            _currentHeight = transform.position.y;
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.fixedDeltaTime * _turningAngle);

            if (Mathf.Abs(_currentHeight - _targetHeight) <= _distanceToStopVerticalMovement)
            {
                _verticalMovement = VerticalMovement.CompletingMovement;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.fixedDeltaTime * _turningAngle);
            }
        }
        else if (_verticalMovement == VerticalMovement.CompletingMovement)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.fixedDeltaTime * _turningAngle);

            if (transform.rotation.z == 0)
            {
                _verticalMovement = VerticalMovement.NotMoving;
            }
        }
    }




}
