using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.Linq;


public class BackgroundManager : MonoBehaviour {

#pragma warning disable 0649

    [Header("Background Switching")]
    [SerializeField] GameObject transitionObject;
    bool openAirBackground;
    bool setTransitionObject;

    [Header("Background Styles")]
    [SerializeField] List<BackgroundTheme> backgroundStyles;
    string _currentStyleName;
    BackgroundTheme _currentTheme;
    List<string> OpenAirStyle = new List<string>();
    List<string> CavernStyle = new List<string>();
    Dictionary<string, BackgroundTheme> _styleDictionary = new Dictionary<string, BackgroundTheme>();
    List<List<GameObject>> _backgroundGroup = new List<List<GameObject>>();
    List<List<GameObject>> _foregroundGroup = new List<List<GameObject>>();

    [Header("Parallax")]
    [SerializeField] bool backgroundParallax = true;
    [SerializeField] float backgroundParallaxSpeed;
    [SerializeField] bool foregroundParallax = true;
    [SerializeField] float foregroundParallaxSpeed;

    float _widthLastBackground, _widthLastForeground;
    Transform _cameraTransform;
    float _currentCameraX, _lastCameraX;

#pragma warning restore

    private void Awake()
    {
        InitializeDictionary();
        InitializeStyles();
    }

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _currentCameraX = _cameraTransform.position.x;
        _lastCameraX = _cameraTransform.position.x;

        ChangeStyleType(); //By default selects open air
        InitializeBackgrounds();
    }

    //Camera moves with LateUpdate, thus manager uses it instead of Update
    private void LateUpdate()
    {
        _lastCameraX = _currentCameraX;
        _currentCameraX = _cameraTransform.position.x;

        if (setTransitionObject)
        {
            SetTransitionObject();
            ChangeStyleType();
            setTransitionObject = false;
            CheckForElementsAfterTransitionObject();
            return;
        }

        CheckBackground();
        CheckForeground();
        ParallaxEffect();
    }

    void InitializeStyles()
    {
        OpenAirStyle.Add("MountainSky");
        CavernStyle.Add("Cavern");
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
        float firstBackgroundPositionX = -_currentTheme.Width;
        //group of elements that make one complete background image
        for (int i = 0; i < _backgroundGroup.Count; i++) 
        {
            //layers in one background image
            foreach (var group in _backgroundGroup[i])
            {
                group.SetActive(true);
                group.transform.position = new Vector3(
                    firstBackgroundPositionX + _currentTheme.Width * i,
                    group.transform.position.y, group.transform.position.z);
            }
            _currentTheme.IncreaseBackgroundIndex();
        }

        for (int i = 0; i < _foregroundGroup.Count; i++)
        {
            //layers in one foreground image
            foreach (var group in _foregroundGroup[i])
            {
                group.SetActive(true);
                group.transform.position = new Vector3(
                    firstBackgroundPositionX + _currentTheme.Width * i,
                    group.transform.position.y, group.transform.position.z);
            }
            _currentTheme.IncreaseForegroundIndex();
        }
        _widthLastBackground = _widthLastForeground = _currentTheme.Width;
    }

    void ChangeStyleType()
    {
        if (!openAirBackground)
        {
            _currentStyleName = OpenAirStyle[Random.Range(0, OpenAirStyle.Count)];
            openAirBackground = true;
        }
        else
        {
            _currentStyleName = CavernStyle[Random.Range(0, CavernStyle.Count)];
            openAirBackground = false;
        }
        _currentTheme = _styleDictionary[_currentStyleName];
    }

    void ParallaxEffect()
    {
        var deltaTime = Time.deltaTime;
        float deltaX = _currentCameraX - _lastCameraX;
        
        if (deltaX == 0)
        {
            return;
        }

        Vector3 backgroundParallaxMovement = Vector3.left * deltaTime * backgroundParallaxSpeed * Mathf.Sign(deltaX);
        backgroundParallaxMovement.x = Mathf.Round(backgroundParallaxMovement.x * 100000f) / 100000f;
        Vector3 foregroundParallaxMovement = Vector3.left * deltaTime * foregroundParallaxSpeed * Mathf.Sign(deltaX);
        foregroundParallaxMovement.x = Mathf.Round(foregroundParallaxMovement.x * 100000f) / 100000f;

        if (backgroundParallax)
        {
            MoveParallax(_backgroundGroup, backgroundParallaxMovement);
            //For some reason parallax movement is not being uniform, therefore we need to check aligment
            AlignParallax(_backgroundGroup);
        }

        if (foregroundParallax)
        {
            MoveParallax(_foregroundGroup, foregroundParallaxMovement);
            //For some reason parallax movement is not being uniform, therefore we need to check aligment
            AlignParallax(_foregroundGroup);
        }
    }

    void MoveParallax(List<List<GameObject>> container, Vector3 parallaxMovement)
    {
        foreach (var group in container)
        {

            foreach (var element in group)
            {
                element.transform.position += parallaxMovement;
            }
        }
    }

    void AlignParallax(List<List<GameObject>> container)
    {
        //CheckOrder
        if (container[0][0].transform.position.x >= container[1][0].transform.position.x ||
            container[1][0].transform.position.x >= container[2][0].transform.position.x ||
            container[2][0].transform.position.x >= container[3][0].transform.position.x ||
            container[3][0].transform.position.x >= container[4][0].transform.position.x)
        {
            container = container.OrderBy(t => t.First().transform.position.x).ToList(); //Order by X position
        }

        var Decimals = container[0][0].transform.position.x - Mathf.Floor(container[0][0].transform.position.x);

        for (int i = 1; i < container.Count; i++)
        {
            var currentDecimal = container[i][0].transform.position.x - Mathf.Floor(container[i][0].transform.position.x);

            if (currentDecimal == Decimals) continue;

            var width = Mathf.Round(container[i][0].GetComponent<SpriteRenderer>().sprite.bounds.size.x);

            foreach (var element in container[i])
            {
                var aux = element.transform.position;
                aux.x = container[i-1][0].transform.position.x + width;
                element.transform.position = aux;
            }
        }
    }

    void CheckBackground()
    {

        _backgroundGroup = _backgroundGroup.OrderBy(t => t.First().transform.position.x).ToList(); //Order by X position

        var _firstBackgroundGroup = _backgroundGroup.First();
        var _firstBackgroundGroupPostionX = _firstBackgroundGroup.First().transform.position.x;

        var _distanceFromCameraBeforeCollection = _firstBackgroundGroup[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x + 10f;

        if (_distanceFromCameraBeforeCollection < 
            _currentCameraX - _firstBackgroundGroupPostionX) //Check if object is beyond allowed distance from camera
        {
            CollectFirstBackground();
            SetNewBackground();
        }
    }

    //Check if background/foreground elements of old theme are right of transition object center
    void CheckForElementsAfterTransitionObject()
    {
        if (_backgroundGroup.Last()[0].transform.position.x > transitionObject.transform.position.x)
        {
            CollectLastBackground();
            SetNewBackground();
        }

        if (_foregroundGroup.Last()[0].transform.position.x > transitionObject.transform.position.x)
        {
            CollectLastForeground();
            SetNewForeground();

        }


    }

    void CollectLastBackground()
    {
        var _lastBackgroundGroup = _backgroundGroup.Last();
        foreach (var group in _lastBackgroundGroup)
        {
            group.SetActive(false);
        }
        _backgroundGroup.RemoveAt(_backgroundGroup.Count-1);
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

        _widthLastBackground = _lastBackgroundGroup[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        foreach (var element in _nextBackground) 
        {
            element.transform.position = 
                new Vector3 (_lastBackgroundGroupPostionX + _widthLastBackground,
                            element.transform.position.y,
                            element.transform.position.z);
            element.SetActive(true);
        }
        
    }

    void SetTransitionObject()
    {
        var _lastBackgroundGroup = _backgroundGroup.Last();
        var _lastBackgroundGroupPositionX = _lastBackgroundGroup.First().transform.position.x;
        var _lastForegroundGroup = _foregroundGroup.Last();
        var _lastForegroundGroupPositionX = _lastForegroundGroup.First().transform.position.x;
        transitionObject.SetActive(true);
        transitionObject.transform.position = new Vector3(
            Mathf.Max(_lastBackgroundGroupPositionX, _lastForegroundGroupPositionX)
            , 0f, 0f);
    }

    void CheckForeground()
    {

        _foregroundGroup = _foregroundGroup.OrderBy(t => t.First().transform.position.x).ToList(); //Order by X position

        var _firstForegroundGroup = _foregroundGroup.First();
        var _firstForegroundGroupPostionX = _firstForegroundGroup.First().transform.position.x;

        var _maxDistanceFromCameraBeforeCollection = _firstForegroundGroup[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x + 10f;

        if (_maxDistanceFromCameraBeforeCollection <
            _currentCameraX - _firstForegroundGroupPostionX) //Check if object is beyond allowed distance from camera
        {
            CollectFirstForeground();
            SetNewForeground();
        }
    }

    void CollectLastForeground()
    {
        var _lastForegroundGroup = _foregroundGroup.Last();
        foreach (var group in _lastForegroundGroup)
        {
            group.SetActive(false);
        }
        _foregroundGroup.RemoveAt(_foregroundGroup.Count-1);
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
        _widthLastForeground = _lastForegroundGroup[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        foreach (var element in _nextForeground)
        {
            element.transform.position =
                new Vector3(_lastForegroundGroupPostionX + _widthLastForeground,
                            element.transform.position.y,
                            element.transform.position.z);
            element.SetActive(true);
        }
        
    }

    public void OnStartTransition()
    {
        setTransitionObject = true;
    }


}
