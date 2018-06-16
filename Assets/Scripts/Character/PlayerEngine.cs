using UnityEngine;

public class PlayerEngine : Engine {

    float _playerPower = 1f;
    PlayerFireWeapon _myWeapon;

    private void Start()
    {
        _myWeapon = GetComponent<PlayerFireWeapon>();
        EnableListeners();
    }


    // Update is called once per frame
    void Update () {
        CalculateCurrentPower();
    }


    void EnableListeners()
    {
        _myWeapon.WeaponFired += OnWeaponFired;
    }

    void CalculateCurrentPower()
    {
        float _AngleZ = transform.rotation.eulerAngles.z;

        if (_AngleZ > 180f && _AngleZ < 355f) //Diving - Gain Energy
        {
            _playerPower += Time.deltaTime * 0.1f * (90f - (_AngleZ % 270f)) / 90f;
            _playerPower = Mathf.Clamp(_playerPower, 0f, 1.2f);
        }
        else if (_AngleZ > 5f && _AngleZ < 90f) //Climbing - Lose Energy
        {
            _playerPower -= Time.deltaTime * 0.1f * (_AngleZ / 90f);
        }
        else  //Level - Stabilizing to 100%
        {
            if (_playerPower < 1f)
            {
                _playerPower += Time.deltaTime * 0.01f;
                _playerPower = Mathf.Clamp(_playerPower, 0f, 1f);
            }
            else if (_playerPower > 1f)
            {
                _playerPower -= Time.deltaTime * 0.02f;
            }
        }
    }

    public override float GetPower()
    {
        return _playerPower;
    }

    public void DrainPower(float powerDrained)
    {
        _playerPower -= powerDrained;
        _playerPower = Mathf.Clamp(_playerPower, 0f, 1.2f);
    }

    public void OnWeaponFired(float weaponConsumption)
    {
        DrainPower(weaponConsumption);
    }

}
