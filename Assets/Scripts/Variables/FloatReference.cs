﻿// ----------------------------------------------------------------------------
// Unite 2017 - Game Architecture with Scriptable Objects
// 
// Author: Ryan Hipple
// Date:   10/04/17
// ----------------------------------------------------------------------------

using System;

[Serializable]
public class FloatReference
{
    public bool UseConstant = true;
    public float ConstantValue;
    public FloatVariable Variable;

    public FloatReference()
    { }

    public FloatReference(float value)
    {
        UseConstant = true;
        ConstantValue = value;
    }

    public float Value
    {
        get { return UseConstant ? ConstantValue : Variable.CurrentValue; }
    }

    public static implicit operator float(FloatReference reference)
    {
        return reference.Value;
    }
}
