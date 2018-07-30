using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

#pragma warning disable 0649
    [SerializeField] float speed;
#pragma warning restore

    Engine myEngine;
    Rigidbody2D myBody;
    float minY, maxY, height;

    // Use this for initialization
    protected virtual void Start () {
        minY = Camera.main.ViewportToWorldPoint(Vector2.zero).y;
        maxY = Camera.main.ViewportToWorldPoint(new Vector2(0f, 1f)).y;

        myEngine = GetComponent<Engine>();
        myBody = GetComponent<Rigidbody2D>();

        height = GetHeight();
    }

    protected virtual float GetHeight()
    {
        return GetComponent<BoxCollider2D>().size.y / 2 * Mathf.Abs(transform.lossyScale.x);
    }

	// Update is called once per frame
	protected virtual void FixedUpdate () {

        MovementLogic();
	}

    protected virtual void MovementLogic()
    {
        Vector3 _direction = -transform.right * Mathf.Sign(transform.localScale.x);
        var newPosition = transform.position + _direction * speed * Time.fixedDeltaTime;
        newPosition.y = Mathf.Clamp(newPosition.y, minY + height, maxY - height);
        myBody.MovePosition(newPosition);
    }

}
