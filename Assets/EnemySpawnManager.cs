using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour {

    public enum SpawnType { Individual, WingFormation, MShapeFormation };

    [Header("SpawnWeight")]
    public float weight_IndividualEnemy;
    public float weight_WingFormation;
    public float weight_MShapeFormation;

    [Header("IndividualSpawner")]
    public EnemySpawner individualEnemySpawner;
    public float timeBetweenSpawn_individual;

    [Header("WingFormationSpawner")]
    public EnemySpawner[] wingFormationSpawners;
    public float timeBetweenSpawn_wingFormation;

    [Header("MShapeFormationSpawner")]
    public EnemySpawner[] mShapeFormationSpawners;
    public float timeBetweenSpawn_mShapeFormation;


    public bool spawnEnemies = true;

    float timeBetweenSpawn;
    float timeSinceLastSpawn;
    SpawnType currentSpawnType = SpawnType.Individual;
    SpawnType newSpawnType;

    private void Start()
    {
        timeSinceLastSpawn = 0f;
    }


    // Update is called once per frame
    void Update () {
        timeSinceLastSpawn += Time.deltaTime;
        if (spawnEnemies)
        {
            if (timeBetweenSpawn <= timeSinceLastSpawn)
            {
                UpdateSpawn();
                ManageSpawning();
                timeSinceLastSpawn = 0f;
            }
        }
        
	}

    void UpdateSpawn()
    {
        newSpawnType = NextSpawnType();

        if (newSpawnType == currentSpawnType)
        {
            return;
        }

        switch (newSpawnType)
        {
            case SpawnType.Individual:
                timeBetweenSpawn = timeBetweenSpawn_individual;
                break;
            case SpawnType.WingFormation:
                timeBetweenSpawn = timeBetweenSpawn_wingFormation;
                break;
            case SpawnType.MShapeFormation:
                timeBetweenSpawn = timeBetweenSpawn_mShapeFormation;
                break;
        }
        currentSpawnType = newSpawnType;
    }
    
    void ManageSpawning()
    {
        switch (currentSpawnType)
        {
            case SpawnType.Individual:
                individualEnemySpawner.SpawnEnemy();
                break;
            case SpawnType.WingFormation:
                int indexWing = Random.Range(0, wingFormationSpawners.Length);
                wingFormationSpawners[indexWing].SpawnAllEnemies();
                break;
            case SpawnType.MShapeFormation:
                int indexMShape = Random.Range(0, mShapeFormationSpawners.Length);
                mShapeFormationSpawners[indexMShape].SpawnAllEnemies();
                break;
        }
    }

    SpawnType NextSpawnType()
    {
        float selection = Random.Range(0f, 1f);
        if (selection <= weight_IndividualEnemy)
        {
            return SpawnType.Individual;
        }
        else if (selection <= (weight_WingFormation + weight_IndividualEnemy))
        {
            return SpawnType.WingFormation;
        }
        else
        {
            return SpawnType.MShapeFormation;
        }
    }


}
