using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GlobalState { NotStarted, MainMenu, Playing, Paused }

[System.Serializable] public class EventGameState : UnityEvent<GlobalState, GlobalState> { }

public class GlobalManager : Singleton<GlobalManager>
{
    public EventGameState OnGlobalStateChanged;
#pragma warning disable 0649
    public bool Debug;
    public bool DebugMenu;

    GlobalState currentGlobalState = GlobalState.NotStarted;
#pragma warning restore


    void Start ()
    {
        if (Debug)
        {
            OnGlobalStateChanged.Invoke(GlobalState.Paused, GlobalState.MainMenu);
        }
        else if (DebugMenu)
        {
            OnGlobalStateChanged.Invoke(GlobalState.MainMenu, GlobalState.NotStarted);
        }
        else
        {
            if (currentGlobalState == GlobalState.NotStarted)
            {
                LoadingManager.Instance.LoadSplash();
            }
        }
    }

    public void UpdateState(GlobalState state)
    {
        GlobalState previousState = currentGlobalState;
        currentGlobalState = state;

        switch (currentGlobalState)
        {
            case GlobalState.NotStarted:
                break;

            case GlobalState.MainMenu:
                break;

            case GlobalState.Playing:
                Time.timeScale = 1.0f;
                break;

            case GlobalState.Paused:
                Time.timeScale = 0.0f;
                break;

            default:
                break;
        }
        OnGlobalStateChanged.Invoke(currentGlobalState, previousState);
    }

    public GlobalState GetCurrentGlobalState()
    {
        return currentGlobalState;
    }

}





