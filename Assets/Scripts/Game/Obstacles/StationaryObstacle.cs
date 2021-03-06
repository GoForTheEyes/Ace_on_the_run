﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryObstacle : MonoBehaviour {

#pragma warning disable 0649
    [SerializeField] float scoreValue;
    [SerializeField][Tooltip("Value between (0,1)")] float minY, maxY;
#pragma warning restore

    float _minY, _maxY;
    bool _playerWentThrough;

    // Use this for initialization
    void Start () {
        _minY = Camera.main.ViewportToWorldPoint(new Vector2(0f, minY)).y; //Can't go lower than 15% of screen
        _maxY = Camera.main.ViewportToWorldPoint(new Vector2(0f, maxY)).y; //Can't go lower than 85% of screen

        transform.position = new Vector2(transform.position.x, Random.Range(_minY, _maxY));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_playerWentThrough)
        {
            _playerWentThrough = true;
            ScoreManager.instance.UpdateScoreExtra(scoreValue);
        }
    }

}
