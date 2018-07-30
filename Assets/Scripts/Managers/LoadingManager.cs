using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : Singleton<LoadingManager>
{

    Action<String> UnloadComplete;

    string currentLevelName;
    Scene currentScene;
    string nextLevelName;
    bool loadAfterUnload;

    List<AsyncOperation> loadOperations;
    List<AsyncOperation> unloadOperations;

    // Use this for initialization
    void Start ()
    {
        currentScene = SceneManager.GetActiveScene();
        currentLevelName = currentScene.name;
        loadOperations = new List<AsyncOperation>();
        unloadOperations = new List<AsyncOperation>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (mode != LoadSceneMode.Additive)
        {
            return;
        }

        if (currentScene.name == "ManagerScene")
        {
            currentScene = scene;
            currentLevelName = scene.name;
            SceneManager.SetActiveScene(scene);
            return;

        }

        DisableOldScene();

        currentScene = scene;
        currentLevelName = scene.name;

        SceneManager.SetActiveScene(scene);
    }

    private void DisableOldScene()
    {

        if (currentScene.IsValid())
        {
            //Disable old scene
            GameObject[] oldSceneObjects = currentScene.GetRootGameObjects();
            for (int i = 0; i < oldSceneObjects.Length; i++)
            {
                oldSceneObjects[i].SetActive(false);
            }
        }
        SceneManager.UnloadSceneAsync(currentScene);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ChangeScene()
    {
        GlobalState currentState = GlobalManager.Instance.GetCurrentGlobalState();

        switch (currentState)
        {
            case GlobalState.NotStarted:
                LoadLevel("MainMenu", true);

                break;
            case GlobalState.MainMenu:
               LoadLevel("Game", true);

                break;
            default:
                LoadLevel("MainMenu", true);

                break;
        }
    }

    void LoadLevel(string levelName, bool additive)
    {
        AsyncOperation ao;
        if (additive)
        {
            ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        }
        else
        {
            ao = SceneManager.LoadSceneAsync(levelName);
        }

        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to load level " + levelName);
            return;
        }
        ao.completed += OnLoadOperationComplete;
        loadOperations.Add(ao);

        currentLevelName = levelName;
    }



    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (loadOperations.Contains(ao))
        {
            loadOperations.Remove(ao);

            if (loadOperations.Count == 0)
            {
                UpdateGameState();
            }
        }
    }



    public void RestartGameScene()
    {
        LoadLevel("Game", true);
    }

    private void UpdateGameState()
    {
        if (currentLevelName == "SplashScreen")
        {
            GlobalManager.Instance.UpdateState(GlobalState.NotStarted);
        }
        else if (currentLevelName == "MainMenu")
        {
            GlobalManager.Instance.UpdateState(GlobalState.MainMenu);
        }
        else
        {
            GlobalManager.Instance.UpdateState(GlobalState.Paused);
        }
    }

    public void LoadSplash()
    {
        LoadLevel("SplashScreen", true);
    }

}
