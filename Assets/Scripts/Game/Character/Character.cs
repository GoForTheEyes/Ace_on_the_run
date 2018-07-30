using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    public float startingHealth;
    public GameObject explosionPrefab;
    protected virtual float CurrentHealth { get; set; }


    // Use this for initialization
    protected virtual void Start () {
        CurrentHealth = startingHealth;
	}
	
    public virtual void ApplyDamage(float dmg)
    {
        CurrentHealth = Mathf.Max(0f, CurrentHealth - dmg);
        if (CurrentHealth == 0f)
        {
            Dead();
        }
    }

    protected virtual void Dead()
    {
        var explosionEffect = Instantiate(explosionPrefab);
        explosionEffect.transform.position = transform.position;
        gameObject.SetActive(false);
    }

}
