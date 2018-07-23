using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlMode { Fly, Tap, Auto, Dead };

public class PlayerMovement : MonoBehaviour {

    enum VerticalMovement { MovingToPoint, CompletingMovement, NotMoving };
    

    [SerializeField] float arcade_playerSpeed = 1.5f;
    [SerializeField] float flappy_forwardSpeed = 2f;
    [SerializeField] float flappy_bounceSpeed = 5f;
    [SerializeField] float auto_cruiseSpeed = 3.5f;

    float _previousYPosition;
    float _targetHeight, _currentHeight;
    float _yDistanceInitial;
    float _turningAngle = 5f;
    float _maxTurnAngle = 60f;
    float _distanceToStopVerticalMovement = 0.25f;
    float _verticalBoost;
    float _currentPower;
    Vector3 _newPosition;
    Vector3 _moveDirection;
    Quaternion _targetRotation;
    Rigidbody2D _myBody;
    PlayerEngine _myEngine;
    float _minY, _maxY, _height, _floorHeight;
    VerticalMovement _verticalMovement;
    ControlMode _controlMode;
    bool _didFlap;

    Transform _transitionStart, _transitionEnd;

    private void Awake()
    {
        _myBody = GetComponent<Rigidbody2D>();
        _myEngine = GetComponent<PlayerEngine>();
    }

    // Use this for initialization
    void Start () {
        _height = GetHeight();
        _minY = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0.0275f)).y; //Can't go lower than 2.75% of screen
        _maxY = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0.975f)).y; //Can't go lower than 97.5% of screen
        _floorHeight = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0f)).y;
    }

    float GetHeight()
    {
        var _myCollider = GetComponent<PolygonCollider2D>();
        var _myPoints = _myCollider.points;
        var _min = _myPoints[0].y;
        var _max = _myPoints[0].y;
        foreach (var point in _myPoints)
        {
            if (point.y < _min) _min = point.y;
            if (point.y > _max) _max = point.y;
        }
        return (_max - _min) * Mathf.Abs(transform.lossyScale.y) / 2f;
    }

    public void ChangeControlMode(ControlMode _newControlMode)
    {
        _controlMode = _newControlMode;
        switch (_controlMode)
        {
            case ControlMode.Fly:
                ChangeToArcadeMode();
                break;
            case ControlMode.Tap:
                ChangeToFlappyMode();
                break;
            case ControlMode.Dead:
                ChangeToDeadMode();
                break;
        }
    }

    void ChangeToArcadeMode()
    {
        _verticalMovement = VerticalMovement.NotMoving;
        _myBody.gravityScale = 0f;
        StartCoroutine(HandleArcadeMovementEveryFixedUpdate());
    }

    void ChangeToDeadMode()
    {
        _myBody.gravityScale = 1f;
    }

    void ChangeToFlappyMode()
    {
        _verticalMovement = VerticalMovement.NotMoving;
        _myBody.gravityScale = 0.25f;
        _myBody.velocity = Vector2.zero;
        StartCoroutine(HandleFlappyMovementEveryFixedUpdate());
    }

    public void ChangeToAutoMode(Transform wayPoint1, Transform wayPoint2)
    {
        _controlMode = ControlMode.Auto;
        _verticalMovement = VerticalMovement.NotMoving;
        _myBody.gravityScale = 0f;
        _myBody.velocity = Vector2.zero;
        _transitionStart = wayPoint1;
        _transitionEnd = wayPoint2;
        StartCoroutine(HandleAutoMovementEveryFixedUpdate());
    }

    // Update is called once per frame
    void Update () {
        _currentPower = _myEngine.GetPower();
        MovementLogic();
    }

    void MovementLogic()
    {
        _previousYPosition = transform.position.y;
        if (_controlMode == ControlMode.Fly)
        {
            ArcadeMovement();
        }
        else if (_controlMode == ControlMode.Tap)
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

    IEnumerator HandleAutoMovementEveryFixedUpdate()
    {
        int stage = 0;
        float tolerance = 0.05f;
        
        while (_controlMode == ControlMode.Auto)
        {
            if (stage==0)
            {
                _moveDirection = (_transitionStart.position - transform.position);
                if (_moveDirection.sqrMagnitude < tolerance)
                {
                    stage = 1;
                }
                _moveDirection = _moveDirection.normalized;
            }
            else if (stage==1)
            {
                _moveDirection = (_transitionEnd.position - transform.position);
                _moveDirection = _moveDirection.normalized;
            }
            _newPosition = transform.position + _moveDirection * auto_cruiseSpeed * Time.fixedDeltaTime;
            _myBody.MovePosition(_newPosition);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator HandleFlappyMovementEveryFixedUpdate()
    {
        while (_controlMode == ControlMode.Tap)
        {
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
                if (_myBody.velocity.y < 0) //No upward motion
                {
                    _myBody.rotation = Mathf.LerpAngle(transform.rotation.eulerAngles.z, angle, Time.fixedDeltaTime);
                }
            }
            HandleFlappyMovement(flappy_forwardSpeed);
            yield return new WaitForFixedUpdate();
        }
    }

    void HandleFlappyMovement(float speed)
    {
        _newPosition = transform.position + transform.right * speed * _currentPower * Time.fixedDeltaTime;
        _newPosition.y = Mathf.Clamp(_newPosition.y, _minY + _height, _maxY - _height);
        transform.position = _newPosition;
    }

    IEnumerator HandleArcadeMovementEveryFixedUpdate()
    {

        while (_controlMode == ControlMode.Fly)
        {
            if (_verticalMovement == VerticalMovement.MovingToPoint) //Move faster vertically
            {
                _verticalBoost = 1f;
            }
            else
            {
                _verticalBoost = 0f;
            }

            if (_verticalMovement != VerticalMovement.NotMoving)
            {
                HandleRotation();
            }
            HandleArcadeMovement(arcade_playerSpeed, _verticalBoost);
            yield return new WaitForFixedUpdate();
        }
    }

    void HandleArcadeMovement(float speed, float verticalBoost)
    {
        _newPosition = transform.position + transform.right * speed * _currentPower * Time.fixedDeltaTime;
        _newPosition.y += -(transform.position.y - _newPosition.y) * (1f + verticalBoost);
        _newPosition.y = Mathf.Clamp(_newPosition.y, _minY + _height, _maxY - _height);
        _myBody.MovePosition(_newPosition);
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
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.fixedDeltaTime * _turningAngle);

            if (Mathf.Abs(_currentHeight - _targetHeight) < 0.05f)
            {
                _verticalMovement = VerticalMovement.NotMoving;
                StabilizePlayer();
            }

            if (transform.rotation.z == 0)
            {
                _verticalMovement = VerticalMovement.NotMoving;
                StabilizePlayer();
            }
        }
    }

    void StabilizePlayer()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        _verticalBoost = 0f;
        _myBody.velocity = new Vector2(_myBody.velocity.x, 0f);
        _myBody.angularVelocity = 0f;
    }

}
