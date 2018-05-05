using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFireWeapon : MonoBehaviour {

    [Header("Weapon")]
    [SerializeField] GameObject weaponRack;
    [SerializeField] float rateOfFirePerMinute;
    [SerializeField] Ammo ammo;
    [SerializeField] float damage;
    [SerializeField] float bulletSpeed;
    float _timeSinceLastShotFired =0f;

    void Fire()
    {
        var shot = Instantiate(ammo, weaponRack.transform.position, weaponRack.transform.rotation);
        shot.Speed = bulletSpeed;
        shot.Damage = damage;
        shot.Direction = Ammo.ShotDirection.right;
    }

    private void Update()
    {
        _timeSinceLastShotFired += Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            if (_timeSinceLastShotFired >= 60f / rateOfFirePerMinute)
            {
                Fire();
                _timeSinceLastShotFired = 0f;
            }
        }



    }

}
