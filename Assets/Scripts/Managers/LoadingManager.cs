using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : Singleton<LoadingManager>
{

    string currentLevelName;
    List<AsyncOperation> loadOperations;
    List<AsyncOperation> unloadOperations;

    // Use this for initialization
    void Start ()
    {
        loadOperations = new List<AsyncOperation>();
        unloadOperations = new List<AsyncOperation>();
        GlobalManager.Instance.AddPersistentObject(gameObject);
    }
	
    public void ChangeScene()
    {
        GlobalState currentState = GlobalManager.Instance.GetCurrentGlobalState();
        switch (currentState)
        {
            case GlobalState.NotStarted:
                LoadLevel("MainMenu", false);
                break;
            case GlobalState.MainMenu:
                LoadLevel("Game", false);
                break;
            default:
                LoadLevel("MainMenu", false);
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

    void UnloadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
        ao.completed += OnUnloadOperationComplete;
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

    private void UpdateGameState()
    {
        if (currentLevelName == "MainMenu")
        {
            GlobalManager.Instance.UpdateState(GlobalState.MainMenu);
        }
        else
        {
            GlobalManager.Instance.UpdateState(GlobalState.Playing);
        }
    }

    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        if (unloadOperations.Contains(ao))
        {
            unloadOperations.Remove(ao);

            if (unloadOperations.Count == 0)
            {
                Debug.Log("Unloaded");
            }
        }
    }



    

}
