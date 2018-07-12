using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum ControlMode { Arcade, Flappy, Auto, Dead };

public class GameManager : MonoBehaviour {

    public Text power;
    public Text health;
    public Text shield;
    public Text control;
    

    public GameObject obstacleSpawnerManager;
    public GameObject enemySpawnerManager;
    public BackgroundManager bgManager;
    public ChangeBackgroundObject changeBGObject;

    Player _player;
    Shield playerShield;
    bool _newAreaIsOpenAir;
    

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        playerShield = _player.GetComponentInChildren<Shield>();
        EnableListeners();
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        UpdateShield();
        
    }

    void EnableListeners()
    {
        _player.PlayerHealthChanged += OnPlayerHealthChanged;
        _player.PlayerEnginePowerChanged += OnPlayerPowerChanged;
        _player.ControlModeChanged += OnControlModeChanged;
        bgManager.StartTransition += OnStartTransition;
        changeBGObject.ExitTransitionArea += OnExitTransition;
    }

    void DisableListeners()
    {
        _player.PlayerHealthChanged -= OnPlayerHealthChanged;
        _player.PlayerEnginePowerChanged -= OnPlayerPowerChanged;
        _player.ControlModeChanged -= OnControlModeChanged;
        bgManager.StartTransition -= OnStartTransition;
        changeBGObject.ExitTransitionArea -= OnExitTransition;
    }

    void OnPlayerPowerChanged(float newPower)
    {
        power.text = Mathf.Floor(newPower*100).ToString() + "%";
    }

    void OnPlayerHealthChanged(float newHealth)
    {
        health.text = Mathf.Floor(newHealth).ToString();
    }

    void OnControlModeChanged(string newMode)
    {
        control.text = newMode;
    }

    void UpdateShield()
    {
        shield.text = Mathf.Floor(playerShield.ShieldStrength).ToString() + "%";
    }

    
    void OnStartTransition(bool newAreaIsOpenAir)
    {
        obstacleSpawnerManager.SetActive(false);
        enemySpawnerManager.SetActive(false);
        _newAreaIsOpenAir = newAreaIsOpenAir;
    }

    void OnExitTransition()
    {
        if (_newAreaIsOpenAir)
        {
            enemySpawnerManager.SetActive(true);
        }
        else
        {
            obstacleSpawnerManager.SetActive(true);
        }
        _player.ExitTransitionObject(_newAreaIsOpenAir);
    }




}
