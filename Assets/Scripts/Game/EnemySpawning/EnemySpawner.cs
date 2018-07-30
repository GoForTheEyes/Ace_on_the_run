using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, ISpawner {

#pragma warning disable 0649
    [SerializeField] GameObject enemy;
    float height, width;
    float minY, maxY;
#pragma warning restore

    private void Awake()
    {
        height = GetHeight();
        minY = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0.0275f)).y + height; //Can't go lower than 2.75% of screen
        maxY = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0.975f)).y - height; //Can't go lower than 97.5% of screen
        //Objective is to spawn outside the current view in at a distance of 10% of the screen
        //as this object moves with the camera, it is already centered at Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0f))
        //Adding 0.6f to the viewport conversion gets us to 1.1f
        width = Camera.main.ViewportToWorldPoint(new Vector2(0.6f, 0f)).x - Camera.main.ViewportToWorldPoint(new Vector2(0f, 0f)).x;
    }

    float GetHeight()
    {
        return enemy.GetComponent<BoxCollider2D>().size.y / 2 * Mathf.Abs(transform.lossyScale.x);
    }


    public void SpawnEnemies()
    {
        float positionY = Random.Range(minY, maxY);
        var newEnemy = Instantiate(enemy);
        newEnemy.transform.position = new Vector3(transform.position.x + width, positionY, 0f);
    }
}
