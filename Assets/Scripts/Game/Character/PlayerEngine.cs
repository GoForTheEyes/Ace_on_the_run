using UnityEngine;

public class PlayerEngine : Engine {

    float _playerPower = 1f; //initial power is 1 * 100%
    float _maxPowerDiving = 1.2f;
    float _maxPowerNotDiving = 1f;
    float _minPower = 0f;

    // Update is called once per frame
    void Update () {
        CalculateCurrentPower();
    }

    void CalculateCurrentPower()
    {
        float _AngleZ = transform.rotation.eulerAngles.z;

        if (_AngleZ > 180f && _AngleZ < 355f) //Diving - Gain Energy
        {
            _playerPower += Time.deltaTime * 0.5f * (90f - (_AngleZ % 270f) ) / 90f;
            _playerPower = Mathf.Clamp(_playerPower, _minPower, _maxPowerDiving);
        }
        else if (_AngleZ > 5f && _AngleZ < 90f) //Climbing - Lose Energy
        {
            _playerPower -= Time.deltaTime * 0.15f * (_AngleZ / 90f);
        }
        else  //Level - Stabilizing to 100%
        {
            if (_playerPower < 1f)
            {
                _playerPower += Time.deltaTime * 0.025f;
                _playerPower = Mathf.Clamp(_playerPower, _minPower, _maxPowerNotDiving);
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
