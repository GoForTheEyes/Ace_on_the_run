using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour {

    enum SpawnType { Individual, WingFormation, MShapeFormation };

    #pragma warning disable 0649

    [Header("SpawnWeight")]
    [SerializeField] float weight_IndividualEnemy;
    [SerializeField] float weight_WingFormation;
    [SerializeField] float weight_MShapeFormation;

    [Header("IndividualSpawner")]
    [SerializeField] GameObject individualSpawnerObject;
    [SerializeField] float timeBetweenSpawn_individual;
    ISpawner individualEnemySpawner;

    [Header("WingFormationSpawner")]
    [SerializeField] GameObject[] wingFormationObjects;
    [SerializeField] float timeBetweenSpawn_wingFormation;
    List<ISpawner> wingFormationSpawners;

    [Header("MShapeFormationSpawner")]
    [SerializeField] GameObject[] mShapeFormationObjects;
    [SerializeField] float timeBetweenSpawn_mShapeFormation;
    List<ISpawner> mShapeFormationSpawners;

#pragma warning restore

    float timeBetweenSpawn;
    float timeSinceLastSpawn;
    float timeElapsed;
    float timeToIncreaseSpawningRate;
    SpawnType currentSpawnType = SpawnType.Individual;
    
    SpawnType newSpawnType;

    float debugNum;

    private void OnEnable()
    {
        InitializeSpawners();
        timeSinceLastSpawn = 0f;
        timeElapsed = 0f;

        timeToIncreaseSpawningRate = 30f;

        UpdateSpawn();
    }

    private void InitializeSpawners()
    {
        individualEnemySpawner = individualSpawnerObject.GetComponent<ISpawner>();
        wingFormationSpawners = new List<ISpawner>();
        mShapeFormationSpawners = new List<ISpawner>();
        foreach (var item in wingFormationObjects)
        {
            wingFormationSpawners.Add(item.GetComponent<ISpawner>());
        }
        foreach (var item in wingFormationObjects)
        {
            mShapeFormationSpawners.Add(item.GetComponent<ISpawner>());
        }
    }

    // Update is called once per frame
    void Update () {
        timeSinceLastSpawn += Time.deltaTime;
        timeElapsed += Time.deltaTime;

        if (timeBetweenSpawn <= timeSinceLastSpawn)
        {
            timeSinceLastSpawn = 0f;
            ManageSpawning();

            UpdateSpawn();
        }

        //cut spawning time by 2/3 every timeToIncreaseSpawningRate seconds
        if (timeElapsed >= timeToIncreaseSpawningRate)
        {
            timeToIncreaseSpawningRate += 30f;
            timeBetweenSpawn_individual *= 0.75f;
            timeBetweenSpawn_wingFormation *= 0.75f;
            timeBetweenSpawn_mShapeFormation *= 0.75f;
        }
        
	}

    void UpdateSpawn()
    {
        newSpawnType = NextSpawnType();

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
                individualEnemySpawner.SpawnEnemies();
                break;
            case SpawnType.WingFormation:
                int indexWing = Random.Range(0, wingFormationSpawners.Count);
                wingFormationSpawners[indexWing].SpawnEnemies();
                break;
            case SpawnType.MShapeFormation:
                int indexMShape = Random.Range(0, mShapeFormationSpawners.Count);
                mShapeFormationSpawners[indexMShape].SpawnEnemies();
                break;
        }
    }

    SpawnType NextSpawnType()
    {
        float selection = Random.Range(0f, weight_IndividualEnemy + weight_MShapeFormation + weight_WingFormation);
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
