using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChangeBackgroundObject : MonoBehaviour {

    public Action ExitTransitionArea;

#pragma warning disable 0649
    [SerializeField] Transform startTunnel, exitTunnel;
#pragma warning restore

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.GetComponent<Player>();
            player.EnterTransitionObject(startTunnel, exitTunnel);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ExitTransitionArea();
        }
    }

}
