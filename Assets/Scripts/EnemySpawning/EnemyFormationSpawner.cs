using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFormationSpawner : MonoBehaviour, ISpawner {

#pragma warning disable 0649
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject[] enemies;
#pragma warning restore

    public void SpawnEnemies()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            var enemy = Instantiate(enemies[0]);
            enemy.transform.position = new Vector3 (spawnPoint.position.x, spawnPoint.position.y, 0f);
            enemy.transform.rotation = spawnPoint.rotation;
        }
    }

}
