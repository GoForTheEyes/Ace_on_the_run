using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] float arcade_playerSpeed = 1.5f;
    [SerializeField] float flappy_forwardSpeed = 2f;
    [SerializeField] float flappy_bounceSpeed = 5f;

    public enum VerticalMovement { MovingToPoint, CompletingMovement , NotMoving };

    public enum ControlMode { Arcade, Flappy, Static };

    Rigidbody2D _myBody;
    BoxCollider2D _myCollider;

    float _targetHeight;
    float _currentHeight;
    float _yDistanceInitial;
    float _turningAngle = 5f;
    float _maxTurnAngle = 60f;
    float _distanceToStopVerticalMovement = 0.25f;
    float _verticalBoost;
    Quaternion _targetRotation;
    float _minY, _maxY, _height;
    VerticalMovement _verticalMovement;
    ControlMode _controlMode;
    bool _didFlap;

    private void Start()
    {
        _myBody = GetComponent<Rigidbody2D>();
        _myCollider = GetComponent<BoxCollider2D>();
        _minY = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0.025f)).y; //Can't go lower than 2.75% of screen
        _maxY = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0.975f)).y; //Can't go lower than 97.5% of screen
        _height = _myCollider.size.y / 2 * Mathf.Abs(transform.lossyScale.x);
        ChangeToArcadeMode();
        ChangeToFlappyMode();
        
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
        _controlMode = ControlMode.Flappy;
        _verticalMovement = VerticalMovement.NotMoving;
        _myBody.gravityScale = 0.25f;
        _myBody.velocity = Vector2.zero;
        StartCoroutine(HandleFlappyMovementEveryFixedUpdate());

    }

    private void Update()
    {
        if (_controlMode == ControlMode.Arcade)
        {
            ArcadeMovement();
        }
        else if (_controlMode == ControlMode.Flappy)
        {
            FlappyMovement();
        }
    }



    void ArcadeMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _currentHeight = transform.position.y;
            _targetHeight = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
            _targetHeight = Mathf.Clamp(_targetHeight, _minY + _height, _maxY - _height);
            _yDistanceInitial = _targetHeight - _currentHeight;
            _targetRotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Sign(_yDistanceInitial) * _maxTurnAngle));
            _verticalMovement = VerticalMovement.MovingToPoint;
        }
        
    }

    void FlappyMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _didFlap = true;
        }
    }

    IEnumerator HandleFlappyMovementEveryFixedUpdate()
    {
        while (_controlMode == ControlMode.Flappy)
        {
            HandleFlappyHorizontalMovement(flappy_forwardSpeed);
            float angle = -90f;

            if (_didFlap)
            {
                _didFlap = false;
                _myBody.velocity = Vector3.up * flappy_bounceSpeed;
                angle += 120f;
                Vector3 aux = new Vector3(0, 0, transform.rotation.eulerAngles.z + angle);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(aux), Time.fixedDeltaTime * 50f);
            }
            else
            {
                if (_myBody.velocity.y <0) //No upward motion
                {
                    _myBody.rotation = Mathf.LerpAngle(transform.rotation.eulerAngles.z, angle, Time.fixedDeltaTime);
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    void HandleFlappyHorizontalMovement(float speed)
    {
        transform.position += transform.right * Time.fixedDeltaTime * speed;
    }

    IEnumerator HandleArcadeMovementEveryFixedUpdate()
    {
        _verticalBoost = 0f;
        while (_controlMode == ControlMode.Arcade)
        {
            if (_verticalMovement != VerticalMovement.NotMoving)
            {
                HandleRotation();
                if (_verticalMovement == VerticalMovement.MovingToPoint)
                {
                    _verticalBoost = 1f;
                }
            }
            HandleArcadeMovement(arcade_playerSpeed, _verticalBoost);
            yield return new WaitForFixedUpdate();
        }
    }

    void HandleArcadeMovement(float speed, float verticalBoost)
    {

        Vector3 _direction = transform.right;
        var newPosition = transform.position + _direction * speed * Time.fixedDeltaTime;
        newPosition.y += - (transform.position.y - newPosition.y) * (1f + verticalBoost);
        newPosition.y = Mathf.Clamp(newPosition.y, _minY + _height, _maxY - _height);
        _myBody.MovePosition(newPosition);

    }

    void HandleRotation()
    {
        _currentHeight = transform.position.y;
        if (_verticalMovement == VerticalMovement.MovingToPoint)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.fixedDeltaTime * _turningAngle);

            if (Mathf.Abs(_currentHeight - _targetHeight) <= _distanceToStopVerticalMovement)
            {
                _verticalMovement = VerticalMovement.CompletingMovement;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.fixedDeltaTime * _turningAngle);
            }
        }
        else if (_verticalMovement == VerticalMovement.CompletingMovement)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.fixedDeltaTime * _turningAngle * 2f);

            if (_currentHeight == _targetHeight)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                _verticalMovement = VerticalMovement.NotMoving;
            }

            if (transform.rotation.z == 0)
            {
                _verticalMovement = VerticalMovement.NotMoving;
            }
        }
    }




}
