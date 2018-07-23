using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace game
{
    public enum GameState { Playing, Transition}

    public class GameManager : MonoBehaviour
    {

        public static GameManager instance; 
#pragma warning disable 0649
        [SerializeField] Text power;
        [SerializeField] Text health;
        [SerializeField] Text shield;
        [SerializeField] Text control;

        [SerializeField] GameObject obstacleSpawnerManager;
        [SerializeField] GameObject enemySpawnerManager;
        [SerializeField] StartGamePanel startGamePanel;

        [SerializeField] BackgroundManager bgManager;

        [SerializeField] ChangeBackgroundObject changeBGObject;

        Player player;
        Shield playerShield;
        public bool newAreaIsOpenAir;
        GameState currentGameState;
#pragma warning restore
        

        private void Awake()
        {
            player = FindObjectOfType<Player>();
            playerShield = player.GetComponentInChildren<Shield>();
            newAreaIsOpenAir = true;
            if (instance == null)
            {
                instance = this;
            }
        }

        private void OnEnable()
        {
            InitializeStartGamePanel();
            EnableListeners();
        }

        void Start()
        {
            currentGameState = GameState.Playing;
        }


        void EnableListeners()
        {
            player.PlayerHealthChanged += OnPlayerHealthChanged;
            player.PlayerEnginePowerChanged += OnPlayerPowerChanged;
            player.ControlModeChanged += OnControlModeChanged;
            bgManager.StartTransition += OnStartBackgroundTransition;
            changeBGObject.EnterTransitionArea += OnEnterTransitionArea;
            changeBGObject.ExitTransitionArea += OnExitTransitionArea;
        }

        void DisableListeners()
        {
            player.PlayerHealthChanged -= OnPlayerHealthChanged;
            player.PlayerEnginePowerChanged -= OnPlayerPowerChanged;
            player.ControlModeChanged -= OnControlModeChanged;
            bgManager.StartTransition -= OnStartBackgroundTransition;
            changeBGObject.EnterTransitionArea -= OnEnterTransitionArea;
            changeBGObject.ExitTransitionArea -= OnExitTransitionArea;
        }

        private void OnDisable()
        {
            DisableListeners();
        }

        void InitializeStartGamePanel()
        {
            startGamePanel.gameObject.SetActive(true);
            startGamePanel.StartCountdownComplete += OnStartCountdownComplete;
            PauseGame();
        }

        void OnStartCountdownComplete()
        {
            StartGame();
        }

        void StartGame()
        {
            ResumeGame();
            ActivateOpposition();
        }
        
        void ActivateOpposition()
        {
            if (newAreaIsOpenAir)
            {
                enemySpawnerManager.SetActive(true);
            }
            else
            {
                obstacleSpawnerManager.SetActive(true);
            }
        }

        void DeactivateOpposition()
        {
            enemySpawnerManager.SetActive(false);
            obstacleSpawnerManager.SetActive(false);
        }

        void PauseGame()
        {
            GlobalManager.Instance.UpdateState(GlobalState.Paused);
        }

        void ResumeGame()
        {
            GlobalManager.Instance.UpdateState(GlobalState.Playing);
        }




        // Update is called once per frame
        void Update()
        {
            UpdateShield();
        }



        void OnPlayerPowerChanged(float newPower)
        {
            power.text = Mathf.Floor(newPower * 100).ToString() + "%";
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

        void OnStartBackgroundTransition()
        {
            DeactivateOpposition();
        }


        void OnEnterTransitionArea()
        {
            currentGameState = GameState.Transition;
            newAreaIsOpenAir = !newAreaIsOpenAir;
        }

        void OnExitTransitionArea()
        {
            currentGameState = GameState.Playing;

            ActivateOpposition();
        }



    }
}
