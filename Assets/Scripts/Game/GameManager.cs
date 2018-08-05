using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace Game
{
    public enum GameMode {Fly, Tap, Transition}

    public class GameManager : MonoBehaviour
    {

        public Action StartBackgroundTransition;


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
        [SerializeField] UIManager uIManager;
        [SerializeField] ChangeBackgroundObject changeBGObject;
        [SerializeField] EarlyTransitionTrigger earlyTransitionTrigger;

        Player player;
        Shield playerShield;


        bool callingTransition;
        float minTimeBeforeTransition = 60f; //60 seconds
        float probablityToTransition = 0.75f; //75%
        float currentProbabilityToTransition;
        float checkTransitionFrequency = 20f; //check for transition again every 20 seconds
        float timeSinceModeChange;
        GameMode currentGameMode;
        GameMode nextGameMode;
#pragma warning restore


        private void Awake()
        {
            player = FindObjectOfType<Player>();
            playerShield = player.GetComponentInChildren<Shield>();
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
            currentGameMode = GameMode.Transition;
            nextGameMode = GameMode.Fly;
            UpdateGameMode();
        }

        void UpdateGameMode()
        {
            player.ChangeControlMode(nextGameMode);
            var aux = currentGameMode;
            currentGameMode = nextGameMode;
            if (currentGameMode==GameMode.Transition & aux==GameMode.Fly)
            {
                nextGameMode = GameMode.Tap;
            }
            else if (currentGameMode == GameMode.Transition & aux == GameMode.Tap)
            {
                nextGameMode = GameMode.Fly;
            }
            else
            {
                nextGameMode = GameMode.Transition;
            }
            timeSinceModeChange = 0f;
            currentProbabilityToTransition = probablityToTransition;
            callingTransition = false;
        }

        void Update()
        {
            UpdateShield();
            GameModeTransitionLogic();
        }


        /// <summary>
        /// If game is not in transition mode track time until its ready to transition
        /// </summary>
        private void GameModeTransitionLogic()
        {
            if (callingTransition)
            {
                return;
            }

            if (currentGameMode != GameMode.Transition)
            {
                timeSinceModeChange += Time.deltaTime;
                if (timeSinceModeChange >= minTimeBeforeTransition)
                {
                    callingTransition = true;
                    CheckForTransition();
                }
            }
        }

        /// <summary>
        /// Roll random number to check if game mode transition happens
        /// Repeat 20 seconds until true
        /// </summary>
        void CheckForTransition()
        {
            if (currentProbabilityToTransition >= UnityEngine.Random.Range(0f,1f) )
            {
                InitiateTransition();
            }
            else
            {
                Invoke("CheckForTransition", checkTransitionFrequency);
                currentProbabilityToTransition = Mathf.Min(currentProbabilityToTransition + 0.1f, 1f);
            }
        }

        void InitiateTransition()
        {
            StartBackgroundTransition();
        }



        void EnableListeners()
        {
            player.PlayerHealthChanged += OnPlayerHealthChanged;
            player.PlayerEnginePowerChanged += OnPlayerPowerChanged;
            player.ControlModeChanged += OnControlModeChanged;
            player.PlayerDied += uIManager.OnPlayerDied;
            player.PlayerDied += OnPlayerDied;
            changeBGObject.EnterTransitionArea += OnEnterTransitionArea;
            changeBGObject.ExitTransitionArea += OnExitTransitionArea;
            earlyTransitionTrigger.NearTransitionArea += OnEnterOuterTransitionArea;
            StartBackgroundTransition += bgManager.OnStartTransition;
        }

        void DisableListeners()
        {
            player.PlayerHealthChanged -= OnPlayerHealthChanged;
            player.PlayerEnginePowerChanged -= OnPlayerPowerChanged;
            player.ControlModeChanged -= OnControlModeChanged;
            player.PlayerDied -= uIManager.OnPlayerDied;
            player.PlayerDied -= OnPlayerDied;
            changeBGObject.EnterTransitionArea -= OnEnterTransitionArea;
            changeBGObject.ExitTransitionArea -= OnExitTransitionArea;
            earlyTransitionTrigger.NearTransitionArea -= OnEnterOuterTransitionArea;
            StartBackgroundTransition -= bgManager.OnStartTransition;
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
            if (currentGameMode == GameMode.Fly)
            {
                enemySpawnerManager.SetActive(true);
            }
            else if (currentGameMode == GameMode.Tap)
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
            if (GlobalManager.Instance != null)
            GlobalManager.Instance.UpdateState(GlobalState.Paused);
        }

        void ResumeGame()
        {
            GlobalManager.Instance.UpdateState(GlobalState.Playing);
        }

        private void OnPlayerDied()
        {
            player.PlayerDied -= uIManager.OnPlayerDied;
            player.PlayerDied -= OnPlayerDied;
            Invoke("PauseGame", 1f);
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

        void OnEnterOuterTransitionArea()
        {
            DeactivateOpposition();
        }

        void OnEnterTransitionArea()
        {
            UpdateGameMode();
        }

        void OnExitTransitionArea()
        {
            UpdateGameMode();
            ActivateOpposition();
        }



    }
}
