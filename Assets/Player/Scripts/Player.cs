using System;
using UnityEngine;

public class Player : Character {

    public Action<String> ControlModeChanged;
    public Action<float> PlayerHealthChanged;
    public Action<float> PlayerEnginePowerChanged;
    
    [SerializeField] AudioSource _myWeaponAudioSource;
    PlayerEngine _myEngine;
    Animator _myAnim;
    PlayerFireWeapon _myWeapon;
    PlayerMovement _myMovement;

    bool _dead;

    private void Awake()
    {
        _myEngine = GetComponent<PlayerEngine>();
        _myAnim = GetComponentInChildren<Animator>();
        _myWeapon = GetComponentInChildren<PlayerFireWeapon>();
        _myMovement = GetComponent<PlayerMovement>();
    }

    // Use this for initialization
    protected override void Start () {
        CurrentHealth = startingHealth;
        _myMovement.ChangeControlMode(ControlMode.Fly);
        ControlModeChanged("Arcade");
    }

    private void OnEnable()
    {
        _myWeapon.AmmoFired += _myEngine.OnWeaponFired;
        _myWeapon.FiredWeapon += OnPlayerFired;
    }

    private void OnDisable()
    {
        _myWeapon.AmmoFired -= _myEngine.OnWeaponFired;
        _myWeapon.FiredWeapon -= OnPlayerFired;
    }

    // Update is called once per frame
    void Update () {
        PlayerEnginePowerChanged(_myEngine.GetPower());
	}

    public override void ApplyDamage(float dmg)
    {
        base.ApplyDamage(dmg);
        PlayerHealthChanged(CurrentHealth);
    }

    protected override void Dead()
    {
        _myMovement.ChangeControlMode(ControlMode.Dead);
        _myAnim.SetTrigger("Dead");
        _dead = true;
        ControlModeChanged("Auto");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_dead)
        {
            Dead();
        }
        var contacts = new ContactPoint2D[10];
        int contactCount = collision.GetContacts(contacts);
        DeadOnInpact(contacts[0].point);
    }

    private void DeadOnInpact(Vector2 position2D)
    {
        var explosionEffect = Instantiate(explosionPrefab);
        explosionEffect.transform.position = new Vector3(position2D.x, position2D.y, 0f);
        Destroy(gameObject);
    }

    void OnPlayerFired()
    {
        _myAnim.SetTrigger("Shooting");
        _myWeaponAudioSource.Play();
    }

    public void EnterTransitionObject(Transform wayPoint1, Transform wayPoint2)
    {
        _myMovement.ChangeToAutoMode(wayPoint1, wayPoint2);
        ControlModeChanged("Auto");
    }

    public void ExitTransitionObject()
    {
        var newAreaIsOpenAir = game.GameManager.instance.newAreaIsOpenAir;

        if (newAreaIsOpenAir)
        {

            _myMovement.ChangeControlMode(ControlMode.Fly);
            ControlModeChanged("Fly");
        }
        else
        {
            
            _myMovement.ChangeControlMode(ControlMode.Tap);
            ControlModeChanged("Tap");
        }
    }



}
