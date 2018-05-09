using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryObstacle : MonoBehaviour {

    [SerializeField][Tooltip("Value between (0,1)")] float minY, maxY;

    float _minY, _maxY;

    // Use this for initialization
    void Start () {
        _minY = Camera.main.ViewportToWorldPoint(new Vector2(0f, minY)).y; //Can't go lower than 15% of screen
        _maxY = Camera.main.ViewportToWorldPoint(new Vector2(0f, maxY)).y; //Can't go lower than 85% of screen

        transform.position = new Vector2(transform.position.x, Random.Range(_minY, _maxY));
    }
	

}
