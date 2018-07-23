using UnityEngine;

public class TrackCamera : MonoBehaviour {

    Transform mainCamera;

	// Use this for initialization
	void OnEnable()
    {
        mainCamera = Camera.main.transform;
    }

	// Update is called once per frame
	void Update () {
        transform.position = mainCamera.position;
	}
}
