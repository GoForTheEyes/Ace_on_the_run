using UnityEngine;
using System;

public class EarlyTransitionTrigger : MonoBehaviour {

    public Action NearTransitionArea;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            NearTransitionArea();
        }
    }


}
