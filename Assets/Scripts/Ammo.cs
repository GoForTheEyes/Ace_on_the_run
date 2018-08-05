using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Ammo : MonoBehaviour {

    public enum ShotDirection { right, left };
    public enum AmmoTarget { Player, Enemy}

    public float Damage { get; set; }
    public float Speed { get; set; }
    [HideInInspector] public ShotDirection Direction;

    int _directionModifier;

    private void Start()
    {
        if (Direction == ShotDirection.right)
        {
            _directionModifier = 1;
        }
        else
        {
            _directionModifier = -1;
        }
    }

    // Update is called once per frame
    void Update () {
        transform.position += transform.right * _directionModifier * Speed * Time.deltaTime;
	}

    private void OnTriggerEnter2D(Collider2D hitObject)
    {
        if (hitObject.gameObject.GetComponentInChildren<Shield>() != null)
        {
            var shield = hitObject.GetComponentInChildren<Shield>();
            if (shield.ShieldStrength > 0)
            {
                shield.OnImpact(this);
                Impact();
                return;
            }
        }

        if (hitObject.gameObject.GetComponent<Character>() != null)
        {
            hitObject.GetComponent<Character>().ApplyDamage(Damage);
            Impact();
        }  
    }

    public void Impact()
    {
        Destroy(gameObject);
    }

}


