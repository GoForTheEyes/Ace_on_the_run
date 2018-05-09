using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {

    [SerializeField] GameObject[] obstacles;
    [SerializeField] float distanceBetweenSpawns;
    [SerializeField] Transform parent;
    float _yPosition, _lastSpawnX;
    int _index;

    float SpawnPositionX { get { return Camera.main.ViewportToWorldPoint(new Vector2(1.1f, 0f)).x; } }


    private void Start()
    {
        SpawnObstacle();
    }

    // Update is called once per frame
    void Update () {
		if (SpawnPositionX - _lastSpawnX >= distanceBetweenSpawns)
        {
            SpawnObstacle();
        }
	}

    int ObjectIndex()
    {
        return Mathf.FloorToInt(Random.Range(0, obstacles.Length));
    }

    void SpawnObstacle()
    {
        _index = ObjectIndex();
        _yPosition = Random.Range(0.2f, 0.8f);
        var obstacle = Instantiate(obstacles[_index], parent);
        obstacle.transform.position = new Vector3(SpawnPositionX, _yPosition, 0f);
        _lastSpawnX = obstacle.transform.position.x;
    }


}
