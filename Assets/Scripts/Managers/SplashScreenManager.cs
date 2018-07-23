using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenManager : MonoBehaviour {


    private void Update()
    {
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            LoadingManager.Instance.ChangeScene();
        }
    }


}
