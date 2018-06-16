using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    public float startingHealth;
    protected virtual float CurrentHealth { get; set; }


    // Use this for initialization
    protected virtual void Start () {
        CurrentHealth = startingHealth;
	}
	
    public virtual void ApplyDamage(float dmg)
    {
        CurrentHealth -= dmg;
        if (CurrentHealth <= 0f)
        {
            Dead();
        }
    }

    protected virtual void Dead()
    {
        gameObject.SetActive(false);
    }

}
