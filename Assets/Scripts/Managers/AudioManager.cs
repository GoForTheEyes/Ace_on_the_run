using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{

#pragma warning disable 0649
    //[Header("OtherGlobalManagers")]
    //[SerializeField] GlobalManager myGlobalManager;

    [Header("Players")]
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource ambientPlayer;
    [SerializeField] AudioSource sfxPlayer;

    [Header("Clips")]
    [SerializeField] AudioClip mainMenuClip;
    [SerializeField] List<AudioClip> gameMusicClips;

    int musicClipIndex = 0;
    bool playing;
    bool mainMenu;
    float currentSongLength, timePlaying;

#pragma warning restore

    private void Update()
    {
        if (!playing)
        {
            return;
        }

        if (!mainMenu)
        {
            timePlaying += Time.deltaTime;
            if (currentSongLength <= timePlaying)
            {
                ChangeMusicTrack();
            }
        }
    }


    void StartMainMenuMusic()
    {
        StopAllMusicPlaying();
        musicPlayer.clip = mainMenuClip;
        musicPlayer.Play();
        musicPlayer.loop = true;
        mainMenu = true;
        playing = true;
    }

    void StartGameMusic()
    {
        StopAllMusicPlaying();
        musicPlayer.clip = gameMusicClips[musicClipIndex];
        currentSongLength = musicPlayer.clip.length;
        timePlaying = 0f;
        musicPlayer.Play();
        musicPlayer.loop = false;
        mainMenu = false;
        playing = true;
    }

    void ChangeMusicTrack()
    {
        StopMusicPlaying();
        musicClipIndex = (musicClipIndex + 1) % gameMusicClips.Count;
        musicPlayer.clip = gameMusicClips[musicClipIndex];
        currentSongLength = musicPlayer.clip.length;
        timePlaying = 0f;
        musicPlayer.Play();
    }

    void StopAllMusicPlaying()
    {
        musicPlayer.Stop();
        ambientPlayer.Stop();
    }

    void StopMusicPlaying()
    {
        musicPlayer.Stop();
        ambientPlayer.Stop();
    }

    public void HandleGlobalStateChanged(GlobalState currentState, GlobalState oldState)
    {
        if (currentState == GlobalState.MainMenu)
        {
            StartMainMenuMusic();
        }
        else if (oldState == GlobalState.MainMenu &&
            (currentState==GlobalState.Paused || currentState==GlobalState.Playing))
        {
            StartGameMusic();
        }
        else
        {
            playing = false;
        }
    }

    




}
