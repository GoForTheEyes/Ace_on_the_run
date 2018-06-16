using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour {

    float _currentPower =1f;

    public virtual float GetPower()
    {
        return _currentPower;
    }

}
