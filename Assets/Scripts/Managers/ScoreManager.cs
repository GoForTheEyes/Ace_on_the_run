using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public Text score;

    public static ScoreManager instance;

    Player _player;

    float _playerInitialPosition, _playerTotalDistance, _extraScore, _currentScore;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
    }

    // Use this for initialization
    void Start () {
        if (instance == null)
        {
            instance = this;
        }
        _playerInitialPosition = _player.transform.position.x;
        _playerTotalDistance = 0f;
        _extraScore = 0f;
        _currentScore = 0f;
    }
	
	// Update is called once per frame
	void Update () {
        if (_player)
        {
            UpdateScore();
        }
    }

    void UpdateScore()
    {
        CalculateTotalDistance();
        _currentScore = Mathf.Floor(_playerTotalDistance / 5f) * 5f + _extraScore;
        score.text = _currentScore.ToString();
    }

    void CalculateTotalDistance()
    {
        var _currentPositionX = _player.transform.position.x;
        if (_currentPositionX - _playerInitialPosition > _playerTotalDistance)
        {
            _playerTotalDistance = _currentPositionX - _playerInitialPosition;
        }
    }

    public void UpdateScoreExtra(float extra)
    {
        _extraScore += extra;
    }

}
