using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class BackgroundManager : MonoBehaviour {

    /// <summary>
    /// Types of Backgrounds
    /// </summary>
    [Header("Background Styles")]
    [SerializeField] List<BackgroundTheme> backgroundStyles;
    Dictionary<string, BackgroundTheme> _styleDictionary = new Dictionary<string, BackgroundTheme>();

    /// <summary>
    /// Current background style
    /// </summary>
    string _currentStyleName;
    BackgroundTheme _currentTheme;

    /// <summary>
    /// Current background & foreground elements
    /// </summary>
    List<List<GameObject>> _backgroundGroup = new List<List<GameObject>>();
    List<List<GameObject>> _foregroundGroup = new List<List<GameObject>>();

    /// <summary>
    /// Camera Tracking
    /// </summary>
    Transform _cameraTransform;
    float _currentCameraX, _lastCameraX;

    /// <summary>
    /// Parallax Effect
    /// </summary>
    [Header("Parallax")]
    [SerializeField] bool backgroundParallax = true;
    [SerializeField] float backgroundParallaxSpeed;
    [SerializeField] bool foregroundParallax = true;
    [SerializeField] float foregroundParallaxSpeed;


    // Use this for initialization
    void Start() {
        _currentStyleName = "MountainSky";
        _cameraTransform = Camera.main.transform;
        _currentCameraX = _cameraTransform.position.x;
        _lastCameraX = _cameraTransform.position.x;
        InitializeDictionary();
        InitializeBackgrounds();
    }

    //Camera moves with LateUpdate, thus manager uses it instead of Update
    private void LateUpdate()
    {
        _currentCameraX = _cameraTransform.position.x;
        ParallaxEffect();
        CheckBackground();
        CheckForeground();
        _lastCameraX = _cameraTransform.position.x;
    }

    void InitializeDictionary()
    {
        foreach (var style in backgroundStyles)
        {
            _styleDictionary.Add(style.name, style);
        }
    }

    void InitializeBackgrounds()
    {
        _currentTheme = _styleDictionary[_currentStyleName];
        _backgroundGroup = _currentTheme.BackgroundGroup;
        _foregroundGroup = _currentTheme.ForegroundGroup;
        //First background panel is to the left of the player;
        Vector3 firstBackgroundPosition = new Vector3(-_currentTheme.Width, 0f, 0f);
        //group of elements that make one complete background image
        for (int i = 0; i < _backgroundGroup.Count; i++) 
        {
            //layers in one background image
            foreach (var group in _backgroundGroup[i])
            {
                group.SetActive(true);
                group.transform.position = firstBackgroundPosition + Vector3.right * _currentTheme.Width * i;
            }
            _currentTheme.IncreaseBackgroundIndex();
        }

        for (int i = 0; i < _foregroundGroup.Count; i++)
        {
            //layers in one foreground image
            foreach (var group in _foregroundGroup[i])
            {
                group.SetActive(true);
                group.transform.position = firstBackgroundPosition + Vector3.right * _currentTheme.Width * i;
            }
            _currentTheme.IncreaseForegroundIndex();
        }
    }

    void ParallaxEffect()
    {
        if (backgroundParallax)
        {
            foreach (var group in _backgroundGroup)
            {
                foreach (var element in group)
                {
                    float deltaX = _currentCameraX - _lastCameraX;
                    if (deltaX != 0)
                    {
                        element.transform.position += Vector3.left * deltaX * backgroundParallaxSpeed;
                    }
                }
            }
        }
        if (foregroundParallax)
        {
            foreach (var group in _foregroundGroup)
            {
                foreach (var element in group)
                {
                    float deltaX = _currentCameraX - _lastCameraX;
                    if (deltaX != 0)
                    {
                        element.transform.position += Vector3.left * deltaX * foregroundParallaxSpeed;
                    }
                }
            }
        }
    }

    void CheckBackground()
    {
        var _maxDistanceFromCameraBeforeCollection = _currentTheme.Width *2f; //This might not work in all cases
        //IDEA make a list of background width and move that
        _backgroundGroup = _backgroundGroup.OrderBy(t => t.First().transform.position.x).ToList(); //Order by X position

        var _firstBackgroundGroup = _backgroundGroup.First();
        var _firstBackgroundGroupPostionX = _firstBackgroundGroup.First().transform.position.x;

        if (_maxDistanceFromCameraBeforeCollection < 
            _currentCameraX - _firstBackgroundGroupPostionX) //Check if object is beyond allowed distance from camera
        {
            CollectFirstBackground();
            SetNewBackground();
        }
    }

    void CollectFirstBackground()
    {
        var _firstBackgroundGroup = _backgroundGroup.First();
        foreach (var group in _firstBackgroundGroup)
        {
            group.SetActive(false);
        }
        _backgroundGroup.RemoveAt(0);
    }

    void SetNewBackground()
    {
        var _lastBackgroundGroup = _backgroundGroup.Last();
        var _lastBackgroundGroupPostionX = _lastBackgroundGroup.First().transform.position.x;
        var _nextBackground = _currentTheme.BackgroundGroup[_currentTheme.NextAvailableBackgroundIndex];
        _backgroundGroup.Add(_nextBackground);
        _currentTheme.IncreaseBackgroundIndex();

        foreach (var element in _nextBackground) 
        {
            element.transform.position = 
                new Vector3 (_lastBackgroundGroupPostionX + _currentTheme.Width,
                            element.transform.position.y,
                            element.transform.position.z);
            element.SetActive(true);
        }
    }

    void CheckForeground()
    {
        var _maxDistanceFromCameraBeforeCollection = _currentTheme.Width *2f; //This might not work in all cases
        //IDEA make a list of foreground width and move that
        _foregroundGroup = _foregroundGroup.OrderBy(t => t.First().transform.position.x).ToList(); //Order by X position

        var _firstForegroundGroup = _foregroundGroup.First();
        var _firstForegroundGroupPostionX = _firstForegroundGroup.First().transform.position.x;

        if (_maxDistanceFromCameraBeforeCollection <
            _currentCameraX - _firstForegroundGroupPostionX) //Check if object is beyond allowed distance from camera
        {
            CollectFirstForeground();
            SetNewForeground();
        }
    }

    void CollectFirstForeground()
    {
        var _firstForegroundGroup = _foregroundGroup.First();
        foreach (var group in _firstForegroundGroup)
        {
            group.SetActive(false);
        }
        _foregroundGroup.RemoveAt(0);
    }

    void SetNewForeground()
    {
        var _lastForegroundGroup = _foregroundGroup.Last();
        var _lastForegroundGroupPostionX = _lastForegroundGroup.First().transform.position.x;
        var _nextForeground = _currentTheme.ForegroundGroup[_currentTheme.NextAvailableForegroundIndex];
        _foregroundGroup.Add(_nextForeground);
        _currentTheme.IncreaseForegroundIndex();

        foreach (var element in _nextForeground)
        {
            element.transform.position =
                new Vector3(_lastForegroundGroupPostionX + _currentTheme.Width,
                            element.transform.position.y,
                            element.transform.position.z);
            element.SetActive(true);
        }
    }



}
