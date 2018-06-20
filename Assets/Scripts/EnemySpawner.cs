using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

#pragma warning disable 0649
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject[] enemies;
#pragma warning restore

    public void SpawnAllEnemies()
    {
        foreach(Transform spawnPoint in spawnPoints)
        {
            var enemy = Instantiate(enemies[0]);
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = spawnPoint.rotation;
        }
    }

    public void SpawnEnemy()
    {
        int index = Random.Range(0, spawnPoints.Length);
        var enemy = Instantiate(enemies[0]);       
        enemy.transform.position = spawnPoints[index].position;
        
    }


}
