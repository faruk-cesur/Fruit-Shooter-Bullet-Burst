using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Health), typeof(Rigidbody))]
public class Fruit : MonoBehaviour, IShootable
{
    [SerializeField, BoxGroup("FRUIT SETTINGS")] private int _moneyReward;
    [SerializeField, BoxGroup("FRUIT SETTINGS")] private float _minJumpForce;
    [SerializeField, BoxGroup("FRUIT SETTINGS")] private float _maxJumpForce;
    [SerializeField, BoxGroup("FRUIT SETUP")] private ParticleSystem _fruitExplosionParticle;
    [SerializeField, BoxGroup("FRUIT SETUP")] private Health _fruitHealth;
    [SerializeField, BoxGroup("FRUIT SETUP")] private Rigidbody _fruitRigidbody;
    [SerializeField, BoxGroup("FRUIT SETUP")] private GameObject _fruitModel;
    private bool _isFruitGetShot;

    private void Start()
    {
        _fruitHealth.OnDeath += OnFruitExplosion;
    }

    private void OnEnable()
    {
        _fruitRigidbody.isKinematic = false;
        FruitJump();
    }

    private void OnDisable()
    {
        _fruitRigidbody.isKinematic = true;
        _isFruitGetShot = false;
        SetFruitExplosionParticle(false);
        SetFruitModelVisual(true);
    }

    private void FruitJump()
    {
        float force = Random.Range(_minJumpForce, _maxJumpForce);
        _fruitRigidbody.AddForce(transform.up * force, ForceMode.Impulse);
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

    private void OnFruitExplosion()
    {
        EarnMoneyOnShoot();
        SetFruitExplosionParticle(true);
        SetFruitModelVisual(false);
    }

    private void SetFruitModelVisual(bool value)
    {
        _fruitModel.SetActive(value);
    }

    private void SetFruitExplosionParticle(bool value)
    {
        _fruitExplosionParticle.gameObject.SetActive(value);
    }

    private void EarnMoneyOnShoot()
    {
        CurrencyManager.Instance.EarnMoney(_moneyReward);
    }
}