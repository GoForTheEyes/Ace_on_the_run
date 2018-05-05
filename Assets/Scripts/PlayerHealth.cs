using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour {

    [SerializeField] float _startingHealth;

    public Action<float> PlayerHealthChanged;
    public Action<float> PlayerEnginePowerChanged;

    float _playerHealth;
    float _playerPower;

    float _playerFiredShot;

	// Use this for initialization
	void Start () {
        _playerHealth = _startingHealth;
        _playerPower = 1f;
	}

    private void Update()
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
        PlayerEnginePowerChanged(_playerPower);
    }

    public void ApplyDamage(float dmg)
    {
        _playerHealth -= dmg;
        PlayerHealthChanged(_playerHealth);
        if (_playerHealth <= 0f)
        {
            PlayerDead();
        }
    }
    
    void PlayerDead()
    {
        gameObject.SetActive(false);
    }

    void PlayerFired(float powerDrained)
    {
        _playerPower -= powerDrained;
    }

}
