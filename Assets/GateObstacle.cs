using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateObstacle : MonoBehaviour {

#pragma warning disable 0649
    [SerializeField] float scoreValue;
    [SerializeField] float speed , minScreenAperture, maxScreenAperture;
    [SerializeField] float timeInBetween;
    [SerializeField] GameObject lowerObstacle, upperObstacle;
    [SerializeField] BoxCollider2D centerAperture;
#pragma warning restore

    bool _closing;
    bool _playerWentThrough;
    float _timeElapsedSinceChange = 0f;
    float _screenHeightInWorldCoordinates;

    // Use this for initialization
    void Start () {
        _screenHeightInWorldCoordinates = Camera.main.ViewportToWorldPoint(Vector2.up).y
            - Camera.main.ViewportToWorldPoint(Vector2.zero).y;

        var _coinFlip = Random.Range(0, 1);
        if (_coinFlip < 0.5f)
        {
            _closing = true;
        }
        else
        {
            _closing = false; ;
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (_timeElapsedSinceChange > timeInBetween)
        {
            _closing = !_closing;
            _timeElapsedSinceChange = 0f;
        }
        _timeElapsedSinceChange += Time.deltaTime;

        if (_closing)
        {
            Close();
        }
        else
        {
            Open();
        }
	}


    void Open()
    {
        if ( (centerAperture.size.y / _screenHeightInWorldCoordinates) < maxScreenAperture)
        {
            lowerObstacle.transform.position += Vector3.down * speed * Time.deltaTime;
            upperObstacle.transform.position += Vector3.up * speed * Time.deltaTime;
            var _gapIncrease = speed * Time.deltaTime * 2f;
            centerAperture.size = new Vector2(centerAperture.size.x, centerAperture.size.y + _gapIncrease);
        }
    }

    void Close()
    {
        if ( (centerAperture.size.y / _screenHeightInWorldCoordinates) > minScreenAperture)
        {
            lowerObstacle.transform.position += Vector3.up * speed * Time.deltaTime;
            upperObstacle.transform.position += Vector3.down * speed * Time.deltaTime;
            var _gapDecrease = speed * Time.deltaTime * 2f;
            centerAperture.size = new Vector2(centerAperture.size.x, centerAperture.size.y - _gapDecrease);
        }
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
