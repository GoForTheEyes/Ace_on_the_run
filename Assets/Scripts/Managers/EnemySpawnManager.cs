using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour {

    public enum SpawnType { Individual, WingFormation, MShapeFormation };

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


    public bool spawnEnemies = true;

    float timeBetweenSpawn;
    float timeSinceLastSpawn;
    SpawnType currentSpawnType = SpawnType.Individual;
    SpawnType newSpawnType;

    private void OnEnable()
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
