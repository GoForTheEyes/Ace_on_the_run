using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject[] enemies;


	// Use this for initialization
	void Start () {
        SpawnAll();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SpawnAll()
    {
        foreach(Transform spawnPoint in spawnPoints)
        {
            var enemy = Instantiate(enemies[0]);
            enemy.transform.position = spawnPoint.position;
        }
    }



}
