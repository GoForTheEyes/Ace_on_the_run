﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFireWeapon : MonoBehaviour {

    public Action<float> WeaponFired;

    [Header("Weapon")]
    [SerializeField] GameObject[] weaponRack;
    [SerializeField] float rateOfFirePerMinute;
    [SerializeField] Ammo ammo;
    [SerializeField] float damage;
    [SerializeField] float bulletSpeed;
    [SerializeField] float energyConsumption;
    float _timeSinceLastShotFired =0f;



    void Fire()
    {
        foreach (GameObject weaponBay in weaponRack)
        {
            var shot = Instantiate(ammo, weaponBay.transform.position, weaponBay.transform.rotation);
            shot.Speed = bulletSpeed;
            shot.Damage = damage;
            shot.Direction = Ammo.ShotDirection.right;
            shot.WhoFiredAmmo = Ammo.WhoFired.Player;
            WeaponFired(energyConsumption);
        }
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
