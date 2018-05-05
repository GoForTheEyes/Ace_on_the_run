using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public float cameraOffsetRight, cameraOffsetLeft;
    float _rightWorldOffsetX, _leftWorldOffsetX;
    GameObject _playerObject;

    void Start () {
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _rightWorldOffsetX = - Camera.main.ViewportToWorldPoint(new Vector2(cameraOffsetRight, 0f)).x;
        _leftWorldOffsetX = -Camera.main.ViewportToWorldPoint(new Vector2(cameraOffsetLeft, 0f)).x;
        transform.position = new Vector3 (_playerObject.transform.position.x + _rightWorldOffsetX, transform.position.y, transform.position.z);
    }
	
	void FixedUpdate () {
        MoveTheCamera();
    }

    void MoveTheCamera()
    {
        float _playerViewportXPosition = Camera.main.WorldToViewportPoint(_playerObject.transform.position).x;
        if ( _playerViewportXPosition > cameraOffsetRight)
        {
            float _cameraPositionX = Mathf.Lerp(transform.position.x, _playerObject.transform.position.x + _rightWorldOffsetX, 0.9f * Time.fixedDeltaTime);
            transform.position = new Vector3(_cameraPositionX, transform.position.y, transform.position.z);
        }        
        else if (_playerViewportXPosition < cameraOffsetLeft)
        {
            float _cameraPositionX = Mathf.Lerp(transform.position.x, _playerObject.transform.position.x + _leftWorldOffsetX, 0.9f * Time.fixedDeltaTime);
            transform.position = new Vector3(_cameraPositionX, transform.position.y, transform.position.z);
        }
    }

}
