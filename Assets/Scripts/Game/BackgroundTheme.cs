using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTheme : MonoBehaviour {

    public List<List<GameObject>> BackgroundGroup { get; private set; }
    public List<List<GameObject>> ForegroundGroup { get; private set; }
    [SerializeField] float worldUnitWidth;
    public float Width { get; private set; }

    public int BackgroundIndex { get; set; }
    public int NextAvailableBackgroundIndex { get { return BackgroundIndex % BackgroundGroup.Count; }}
    public int ForegroundIndex { get; set; }
    public int NextAvailableForegroundIndex { get { return ForegroundIndex % ForegroundGroup.Count; } }

    public void IncreaseBackgroundIndex()
    {
        BackgroundIndex = (BackgroundIndex + 1) % BackgroundGroup.Count;
    }

    public void IncreaseForegroundIndex()
    {
        ForegroundIndex = (ForegroundIndex + 1) % ForegroundGroup.Count;
    }

    void Awake () {
        BackgroundIndex = 0;
        ForegroundIndex = 0;
        InitializeLists();
        if (BackgroundGroup != null)
        {
            Width = BackgroundGroup[0][0].GetComponent<SpriteRenderer>().sprite.bounds.extents.x * 2f;
            //Width = worldUnitWidth;
        }
	}
	
    void InitializeLists()
    {
        var _groupB = new List<List<GameObject>>();
        var _groupF = new List<List<GameObject>>();
        foreach (Transform child in transform)
        {
            var _tempB = new List<GameObject>();
            var _tempF = new List<GameObject>();
            foreach (Transform backgroundElement in child)
            {
                if (backgroundElement.CompareTag("Background"))
                {
                    _tempB.Add(backgroundElement.gameObject);
                }
                else if (backgroundElement.CompareTag("Foreground"))
                {
                    _tempF.Add(backgroundElement.gameObject);
                }
            }
            _groupB.Add(_tempB);
            _groupF.Add(_tempF);
        }
        BackgroundGroup = _groupB;
        ForegroundGroup = _groupF;
    }


}
