using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFireWeapon : MonoBehaviour {

    public Action<float> AmmoFired;
    public Action FiredWeapon;

#pragma warning disable 0649
    [Header("Weapon")]
    [SerializeField] GameObject[] weaponRack;
    [SerializeField] float rateOfFirePerMinute;
    [SerializeField] Ammo ammo;
    [SerializeField] float damage;
    [SerializeField] float bulletSpeed;
    [SerializeField] float energyConsumption;
    float _timeSinceLastShotFired =0f;

    Player _myPlayer;
#pragma warning restore

    private void Awake()
    {
        _myPlayer = GetComponentInParent<Player>();
    }

    void Fire()
    {
        foreach (GameObject weaponBay in weaponRack)
        {

            var shot = Instantiate(ammo, weaponBay.transform.position, weaponBay.transform.rotation);
            shot.Speed = bulletSpeed;
            shot.Damage = damage;
            shot.Direction = Ammo.ShotDirection.right;
            shot.ammoTarget = Ammo.AmmoTarget.Enemy;
            AmmoFired(energyConsumption);
        }
        
    }

    private void Update()
    {
        if (_myPlayer.PlayerAlive() )
        {
            return;
        }

        _timeSinceLastShotFired += Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            if (_timeSinceLastShotFired >= 60f / rateOfFirePerMinute)
            {
                Invoke("Fire", 0.1f);
                _timeSinceLastShotFired = 0f;
                FiredWeapon();
            }
        }

    }

}
