using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {


#pragma warning disable 0649
    [SerializeField] float MaxRightOffset;
    [SerializeField] float MaxLeftOffset;
    [SerializeField] float CameraSpeed;
    [SerializeField] GameObject ScreenDependentObjects;

    float worldWidth, rightBoundX, leftBoundX;
    GameObject focusCameraTarget;
#pragma warning restore

    private void Start()
    {
        worldWidth = Camera.main.ViewportToWorldPoint(Vector2.right).x - Camera.main.ViewportToWorldPoint(Vector2.zero).x;
        rightBoundX = worldWidth * MaxRightOffset;
        leftBoundX = worldWidth * MaxLeftOffset;
        ResetPosition();
        FocusOnPlayer();
    }

    void ResetPosition()
    {
        transform.position = Vector3.zero;
    }


    void FocusOnPlayer()
    {
        focusCameraTarget = GameObject.FindGameObjectWithTag("Player");
        transform.position = new Vector3(focusCameraTarget.transform.position.x + (worldWidth / 2f) - leftBoundX,
            focusCameraTarget.transform.position.y, focusCameraTarget.transform.position.z);
        ScreenDependentObjects.SetActive(true);
    }

    void LateUpdate()
    {
        if (focusCameraTarget != null)
        {
            MoveTheCamera();
        }
    }

    void MoveTheCamera()
    {
        float freeCameraXPositionMove = 
            Camera.main.WorldToViewportPoint(transform.position + Vector3.right * CameraSpeed * Time.fixedDeltaTime).x 
            - Camera.main.WorldToViewportPoint(transform.position).x;
        float playerViewportXPosition = Camera.main.WorldToViewportPoint(focusCameraTarget.transform.position).x;

        if ( playerViewportXPosition - freeCameraXPositionMove > MaxRightOffset)
        {
            float cameraPositionX = focusCameraTarget.transform.position.x + (worldWidth / 2f) - rightBoundX;
            transform.position = new Vector3(cameraPositionX, transform.position.y, transform.position.z);
        }
        else if (playerViewportXPosition - freeCameraXPositionMove < MaxLeftOffset)
        {
            float cameraPositionX = Mathf.Lerp(transform.position.x, 
                focusCameraTarget.transform.position.x + (worldWidth / 2f) - leftBoundX, 
                Time.fixedDeltaTime);
            transform.position =  new Vector3(cameraPositionX, transform.position.y, transform.position.z);
        }

    }

}
