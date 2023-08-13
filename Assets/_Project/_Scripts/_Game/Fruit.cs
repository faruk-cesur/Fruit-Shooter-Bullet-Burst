using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Fruit : MonoBehaviour, IShootable
{
    [SerializeField, BoxGroup("Fruit Settings")] private int _moneyReward;
    [SerializeField, BoxGroup("Fruit Setup")] private ParticleSystem _fruitBlastParticle;
    [SerializeField, BoxGroup("Fruit Setup")] private Health _fruitHealth;
    private bool _isFruitGetShot;

    private void Start()
    {
        _fruitHealth.OnDeath += OnFruitBlast;
    }

    private void OnDestroy()
    {
        _fruitHealth.OnDeath -= OnFruitBlast;
    }

    public void GetShot(float gunDamage)
    {
        if (_isFruitGetShot)
            return;

        _isFruitGetShot = true;

        TakeDamage(gunDamage);
    }

    private void TakeDamage(float gunDamage)
    {
        _fruitHealth.Damage(gunDamage);
    }

    private void OnFruitBlast()
    {
        PlayFruitBlastParticle();
        EarnMoneyOnShoot();
        Destroy(gameObject);
    }

    private void PlayFruitBlastParticle()
    {
        _fruitBlastParticle.transform.SetParent(null);
        _fruitBlastParticle.Play();
        Destroy(_fruitBlastParticle.gameObject, 5f);
    }

    private void EarnMoneyOnShoot()
    {
        CurrencyManager.Instance.EarnMoney(_moneyReward);
    }
}