using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject[] enemies;

    public void SpawnAllEnemies()
    {
        foreach(Transform spawnPoint in spawnPoints)
        {
            var enemy = Instantiate(enemies[0]);
            enemy.transform.position = spawnPoint.position;
        }
    }

    public void SpawnEnemy()
    {
        int index = Random.Range(0, spawnPoints.Length);
        var enemy = Instantiate(enemies[0]);
        enemy.transform.position = spawnPoints[index].position;
    }


}
