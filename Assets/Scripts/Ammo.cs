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
    [HideInInspector] public AmmoTarget ammoTarget;

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
        if (hitObject.gameObject.CompareTag(ammoTarget.ToString()))
        {
            hitObject.GetComponent<Character>().ApplyDamage(Damage);
        }
    }

}


