using System;
using UnityEngine;

public class Player : Character {

    //public new float startingHealth;

    public Action<float> PlayerHealthChanged;
    public Action<float> PlayerEnginePowerChanged;

    Engine _myEngine;

    // Use this for initialization
    protected override void Start () {
        CurrentHealth = startingHealth;
        _myEngine = GetComponent<Engine>();
	}

    // Update is called once per frame
    void Update () {
        PlayerEnginePowerChanged(_myEngine.GetPower());
	}

    public override void ApplyDamage(float dmg)
    {
        base.ApplyDamage(dmg);
        PlayerHealthChanged(CurrentHealth);
    }

    protected override void Dead()
    {
        base.Dead();
    }



}
