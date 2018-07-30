using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlMode { Fly, Tap, Auto, Dead };

public class PlayerMovement : MonoBehaviour {

    enum RotationStage { startingPhase1, midPhase1, phase2, startingPhase3, midPhase3 }

#pragma warning disable 0649
    [SerializeField] float flappy_forwardSpeed = 2f;
    [SerializeField] float flappy_bounceSpeed = 5f;
    [SerializeField] float auto_cruiseSpeed = 3.5f;

    float _previousYPosition;
    
    float _yDistanceInitial;
    float _currentPower;
    Vector3 _currentVelocity;
    Vector3 _newPosition;
    Vector3 _moveDirection;
    Quaternion _targetRotation;
    Rigidbody2D myRigidbody;
    PlayerEngine myEngine;
    float _minY, _maxY, _height, _floorHeight;

    ControlMode _controlMode;
    bool _didFlap;

    //new vars
    float targetHeigth, startingPositionY;
    bool moveVertically;
    bool startCountingAngleTurn = false;
    float distanceTraveledFromAngle0;
    float positionAtAngle0;
    float distanceToTravel;
    float distanceTraveled;
    float distanceRemaining;
    float directionModifier;
    float startingRotation;
    float currentRotationAngle;

    RotationStage myRotationStage;

    [SerializeField] float flySpeed;
    [SerializeField] float maxClimbAngle = 60f;
    [SerializeField] float turnSpeed = 5f;
    [SerializeField] float tolerance = 0.0005f;
    [SerializeField] float climbingFactor = 12.5f;

    Transform _transitionStart, _transitionEnd;

#pragma warning restore

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myEngine = GetComponent<PlayerEngine>();
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
        myRigidbody.gravityScale = 0f;
        myRigidbody.drag = 0f;

        StartCoroutine(HandleFlyMovementEveryFixedUpdate());
    }

    void ChangeToDeadMode()
    {
        myRigidbody.gravityScale = 1f;
        myRigidbody.drag = 1f;
    }

    void ChangeToFlappyMode()
    {
        myRigidbody.gravityScale = 0.5f;
        //myRigidbody.drag = 1f;
        myRigidbody.velocity = Vector2.zero;
        StartCoroutine(HandleFlappyMovementEveryFixedUpdate());
    }

    public void ChangeToAutoMode(Transform wayPoint1, Transform wayPoint2)
    {
        _controlMode = ControlMode.Auto;
        myRigidbody.gravityScale = 0f;
        myRigidbody.velocity = Vector2.zero;
        _transitionStart = wayPoint1;
        _transitionEnd = wayPoint2;
        StartCoroutine(HandleAutoMovementEveryFixedUpdate());
    }

    // Update is called once per frame
    void Update () {
        _currentPower = myEngine.GetPower();
        MovementLogic();
    }

    void MovementLogic()
    {
        _previousYPosition = transform.position.y;
        if (_controlMode == ControlMode.Fly)
        {
            FlyMovement();
        }
        else if (_controlMode == ControlMode.Tap)
        {
            FlappyMovement();
        }
    }


    #region FlyMovement
    void FlyMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            moveVertically = true;
            CalculateDestination();
            UpdateRotationStageVariables(RotationStage.startingPhase1);
        }
    }

    void CalculateDestination()
    {
        startingPositionY = transform.position.y;
        startingRotation = myRigidbody.rotation;
        targetHeigth = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
        targetHeigth = Mathf.Clamp(targetHeigth, _minY + _height, _maxY - _height);
        distanceToTravel = Mathf.Abs(targetHeigth - startingPositionY);
        distanceTraveled = 0f;
        distanceRemaining = distanceToTravel - distanceTraveled;
        directionModifier = Mathf.Sign(targetHeigth - startingPositionY);
    }

    void UpdateRotationStageVariables(RotationStage newRotationStage)
    {
        myRotationStage = newRotationStage;
        switch (myRotationStage)
        {
            case RotationStage.startingPhase1:
                startCountingAngleTurn = false;
                distanceTraveledFromAngle0 = 0f;
                currentRotationAngle = myRigidbody.rotation;
                break;
            case RotationStage.midPhase1:
                break;
            case RotationStage.phase2:
                break;
            case RotationStage.startingPhase3:
                currentRotationAngle = myRigidbody.rotation;
                break;
        }
    }

    void CheckRotationPhase()
    {
        switch (myRotationStage)
        {
            case RotationStage.startingPhase1:
                if (distanceRemaining - distanceTraveledFromAngle0 < 0f)
                {
                    UpdateRotationStageVariables(RotationStage.startingPhase3);
                }
                else if (currentRotationAngle == maxClimbAngle * directionModifier)
                {
                    UpdateRotationStageVariables(RotationStage.phase2);
                }
                else
                {
                    UpdateRotationStageVariables(RotationStage.midPhase1);
                }
                break;
            case RotationStage.midPhase1:
                if (distanceRemaining - distanceTraveledFromAngle0 < 0f)
                {
                    UpdateRotationStageVariables(RotationStage.startingPhase3);
                }
                else if (currentRotationAngle == maxClimbAngle * directionModifier)
                {
                    UpdateRotationStageVariables(RotationStage.phase2);
                }
                break;
            case RotationStage.phase2:
                if (distanceRemaining - distanceTraveledFromAngle0 < 0f)
                {
                    UpdateRotationStageVariables(RotationStage.startingPhase3);
                }
                break;
            case RotationStage.startingPhase3:
                UpdateRotationStageVariables(RotationStage.midPhase3);
                break;
            case RotationStage.midPhase3:
                break;
        }
    }

    IEnumerator HandleFlyMovementEveryFixedUpdate()
    {

        while (_controlMode == ControlMode.Fly)
        {
            if (moveVertically)
            {
                distanceTraveled = Mathf.Abs(myRigidbody.position.y - startingPositionY);
                distanceRemaining = distanceToTravel - distanceTraveled;

                Rotate();
            }

            Vector2 velocity = transform.TransformDirection(transform.right) * flySpeed;
            velocity.y *= (climbingFactor  * (1 + myEngine.GetPower() ) ); //Extra climbing/diving speed
            //Check that it will not over or undershoot
            
            if (moveVertically)
            {
                if (distanceRemaining < tolerance) //Close to tolerance
                {
                    velocity.y = (distanceRemaining) * directionModifier / Time.fixedDeltaTime;
                    moveVertically = false;
                    myRigidbody.rotation = 0f;
                }

                if (myRotationStage == RotationStage.midPhase3 || myRotationStage == RotationStage.startingPhase3)
                {
                    if (distanceRemaining < Mathf.Abs(velocity.y * Time.fixedDeltaTime)) //Overshoot 
                    {
                        velocity.y = (distanceRemaining) * directionModifier / Time.fixedDeltaTime;
                        moveVertically = false;
                        myRigidbody.rotation = 0f;
                    }
                }
            }

            myRigidbody.velocity = velocity;

            yield return new WaitForFixedUpdate();
        }
    }
 
    private void Rotate()
    {
        if (!startCountingAngleTurn && Mathf.Abs(myRigidbody.rotation) < tolerance)
        {
            distanceTraveledFromAngle0 = 0f;
            positionAtAngle0 = myRigidbody.position.y;
            startCountingAngleTurn = true;
        }

        //Understand where in the climb stage is the player
        // 1 == diving/climbing
        // 2 == mid manouver <- at maxClimbingAngle
        // 3 == ending dive
        CheckRotationPhase();

        if ( myRotationStage == RotationStage.midPhase1 || myRotationStage == RotationStage.startingPhase1)
        {
            if (startCountingAngleTurn) distanceTraveledFromAngle0 = Mathf.Abs(myRigidbody.position.y - positionAtAngle0);
            currentRotationAngle += directionModifier * turnSpeed;
            if (directionModifier == 1) currentRotationAngle = Mathf.Min(currentRotationAngle, maxClimbAngle);
            if (directionModifier == -1) currentRotationAngle = Mathf.Max(currentRotationAngle, -maxClimbAngle);
        }
        else if (myRotationStage == RotationStage.phase2)
        {
            //do nothing
        }
        else if (myRotationStage == RotationStage.midPhase3 || myRotationStage == RotationStage.startingPhase3)
        {
            currentRotationAngle -=  directionModifier * turnSpeed;
            if (directionModifier == 1) currentRotationAngle = Mathf.Max(currentRotationAngle, 0f);
            if (directionModifier == -1) currentRotationAngle = Mathf.Min(currentRotationAngle, 0f);
        }
        myRigidbody.rotation = currentRotationAngle;
    }
    #endregion


    void FlappyMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _didFlap = true;
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
                myRigidbody.velocity = Vector3.up * flappy_bounceSpeed;
                angle += 120f;
                Vector3 aux = new Vector3(0, 0, transform.rotation.eulerAngles.z + angle);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(aux), Time.fixedDeltaTime * 50f);
            }
            else
            {
                if (myRigidbody.velocity.y < 0) //No upward motion
                {
                    myRigidbody.rotation = Mathf.LerpAngle(transform.rotation.eulerAngles.z, angle, Time.fixedDeltaTime);
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
        _currentVelocity = (_newPosition - transform.position) * Time.fixedDeltaTime;
        transform.position = _newPosition;
    }

    IEnumerator HandleAutoMovementEveryFixedUpdate()
    {
        int stage = 0;
        float tolerance = 0.05f;

        while (_controlMode == ControlMode.Auto)
        {
            if (stage == 0)
            {
                _moveDirection = (_transitionStart.position - transform.position);
                if (_moveDirection.sqrMagnitude < tolerance)
                {
                    stage = 1;
                }
                _moveDirection = _moveDirection.normalized;
            }
            else if (stage == 1)
            {
                _moveDirection = (_transitionEnd.position - transform.position);
                _moveDirection = _moveDirection.normalized;
            }
            _newPosition = transform.position + _moveDirection * auto_cruiseSpeed * Time.fixedDeltaTime;

            float separationAngle = Vector2.SignedAngle(transform.position, _newPosition);
            float newZAngle = (separationAngle > 0) ? 
                Mathf.Min(turnSpeed * Time.fixedDeltaTime, separationAngle) :
                Mathf.Max( - turnSpeed * Time.fixedDeltaTime, separationAngle);
            myRigidbody.MoveRotation(newZAngle);
            myRigidbody.MovePosition(_newPosition);
            yield return new WaitForFixedUpdate();
        }
    }

    void StabilizePlayer()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 0f);
        myRigidbody.angularVelocity = 0f;
    }

}
