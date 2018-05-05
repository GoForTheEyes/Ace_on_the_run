using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    //public static GameManager instance;

    public Text power;
    public Text health;

    PlayerHealth _player;

	// Use this for initialization
	void Start () {
        //if (instance == null)
        //      {
        //          GameManager.instance = this;
        //      }
        _player = GameObject.FindObjectOfType<PlayerHealth>();
        EnableListeners();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void EnableListeners()
    {
        _player.PlayerHealthChanged += OnPlayerHealthChanged;
        _player.PlayerEnginePowerChanged += OnPlayerPowerChanged;
    }

    void DisableListeners()
    {
        _player.PlayerHealthChanged -= OnPlayerHealthChanged;
        _player.PlayerEnginePowerChanged -= OnPlayerPowerChanged;
    }

    void OnPlayerPowerChanged(float newPower)
    {
        power.text = Mathf.Floor(newPower*100).ToString() + "%";
    }

    void OnPlayerHealthChanged(float newHealth)
    {
        health.text = Mathf.Floor(newHealth).ToString();
    }


}
