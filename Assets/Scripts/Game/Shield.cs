using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{

    public float ShieldStrength { get; private set; }

    Animator myAnimator;
    AudioSource myAudioSource;
    float maxShield = 100f;
    float regenShieldPerSecond = 2.5f;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myAudioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start () {
        ShieldStrength = maxShield;
	}
	
	// Update is called once per frame
	void Update () {
        if (ShieldStrength < maxShield)
        {
            ShieldStrength += regenShieldPerSecond * Time.deltaTime;
            ShieldStrength = Mathf.Clamp(ShieldStrength, 0f, maxShield);
        }
	}


    public void OnImpact(Ammo incomingProjectile)
    {
        if (ShieldStrength <= 0) return; //Shield is offline
        ReduceShield(incomingProjectile.Damage);
        ShieldImpactEffect();
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (ShieldStrength <= 0) return; //Shield is offline
    //    if (!collision.gameObject.GetComponent<Ammo>()) return; //Shield only reacts to ammo

    //    Ammo incomingProjectile = collision.GetComponent<Ammo>();
    //    if (incomingProjectile.ammoTarget == Ammo.AmmoTarget.Enemy) return; //Shield ignores player ammo (ammo seeking enemies)

    //    ReduceShield(incomingProjectile.Damage);
    //    incomingProjectile.Impact();
    //    ShieldImpactEffect();
    //}

    void ReduceShield(float incomingDamage)
    {
        ShieldStrength -= incomingDamage;
        ShieldStrength = Mathf.Clamp(ShieldStrength, 0f, maxShield);
    }
    
    void IncreaseShield(float shieldBoost)
    {
        ShieldStrength += shieldBoost;
        ShieldStrength = Mathf.Clamp(ShieldStrength, 0f, maxShield);
    }

    void ShieldImpactEffect()
    {
        if (ShieldStrength <0.3)
        {
            myAnimator.SetTrigger("LowShield");
        }
        else if (ShieldStrength<0.65)
        {
            myAnimator.SetTrigger("MediumShield");
        }
        else
        {
            myAnimator.SetTrigger("FullShield");
        }
        myAudioSource.Play();
    }


}
