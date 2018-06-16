using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    [Header("Movement")]
    [SerializeField] float speed;
    [Header("Life")]
    [SerializeField] float hp;
    [Header("Weapon")]
    [SerializeField] GameObject weaponRack;
    [SerializeField] float rateOfFirePerMinute;
    [SerializeField] Ammo ammo;
    [SerializeField] float damage;
    [SerializeField] float bulletSpeed;
    float _timeSinceLastShotFired;

    float _minY, _maxY, _height;
    Rigidbody2D _myBody;
    BoxCollider2D _myCollider;

    

    // Use this for initialization
    void Start () {
        _myBody = GetComponent<Rigidbody2D>();
        _myCollider = GetComponent<BoxCollider2D>();
        _minY = Camera.main.ViewportToWorldPoint(Vector2.zero).y;
        _maxY = Camera.main.ViewportToWorldPoint(new Vector2(0f, 1f)).y;
        _height = _myCollider.size.y/2 * Mathf.Abs(transform.lossyScale.x);
    }

    private void Update()
    {
        _timeSinceLastShotFired += Time.deltaTime;
        if (_timeSinceLastShotFired >= 60f / rateOfFirePerMinute)
        {
            Fire();
            _timeSinceLastShotFired = 0f;
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        HorizontalMovement();
	}

    void HorizontalMovement()
    {
        Vector3 _direction = - transform.right * Mathf.Sign(transform.localScale.x);
        var newPosition = transform.position + _direction * speed * Time.fixedDeltaTime;
        newPosition.y = Mathf.Clamp(newPosition.y, _minY + _height, _maxY - _height);
        _myBody.MovePosition(newPosition);
    }
    
    void Fire()
    {
        var shot = Instantiate(ammo, weaponRack.transform.position, weaponRack.transform.rotation, weaponRack.transform);
        shot.Speed = bulletSpeed;
        shot.Damage = damage;
        shot.Direction = Ammo.ShotDirection.left;
        shot.WhoFiredAmmo = Ammo.WhoFired.Enemy;
    }

    public void ApplyDamage(float dmg)
    {
        hp -= dmg;
        if (hp<=0)
        {
            Destroy(gameObject);
        }
    }

}
