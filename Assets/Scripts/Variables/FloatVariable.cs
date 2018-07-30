// ----------------------------------------------------------------------------
// Unite 2017 - Game Architecture with Scriptable Objects
// 
// Author: Ryan Hipple
// Date:   10/04/17
// ----------------------------------------------------------------------------

using UnityEngine;

[CreateAssetMenu]
public class FloatVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    public float DefaultValue;

    private float currentValue;

    public float CurrentValue
    {
        get { return currentValue; }
        set { currentValue = value;  }
    }

    private void OnEnable()
    {
        CurrentValue = DefaultValue;
    }

    public void ApplyChange(float amount)
    {
        CurrentValue += amount;
    }

    public void ApplyChange(FloatVariable amount)
    {
        CurrentValue += amount.CurrentValue;
    }
}
