using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireWeapon : MonoBehaviour {

#pragma warning disable 0649
    [Header("Weapon")]
    [SerializeField]
    GameObject[] weaponRack;
    [SerializeField] float rateOfFirePerMinute;
    [SerializeField] Ammo ammo;
    [SerializeField] float damage;
    [SerializeField] float bulletSpeed;
    float _timeSinceLastShotFired = 0f;
#pragma warning restore

    void Fire()
    {
        foreach (GameObject weaponBay in weaponRack)
        {
            var shot = Instantiate(ammo, weaponBay.transform.position, weaponBay.transform.rotation);
            shot.Speed = bulletSpeed;
            shot.Damage = damage;
            shot.Direction = Ammo.ShotDirection.left;
            shot.gameObject.layer = LayerMask.NameToLayer("Enemies Ammo");
        }
        _timeSinceLastShotFired = 0f;
    }

    private void Update()
    {
        _timeSinceLastShotFired += Time.deltaTime;
        if (_timeSinceLastShotFired >  (60f / rateOfFirePerMinute))
        {
            Fire();
        }
    }
}
