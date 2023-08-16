using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private Rigidbody _bulletRigidbody;
    [SerializeField] private ParticleSystem _bulletParticle;
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private GameObject _bulletSmallModel;
    private bool _isHide;

    private void OnEnable()
    {
        _bulletRigidbody.isKinematic = false;
        _bulletSmallModel.SetActive(true);
        _isHide = false;
    }

    private void OnDisable()
    {
        _bulletRigidbody.isKinematic = true;
    }

    private void Update()
    {
        MoveBulletForward();
    }

    private void MoveBulletForward()
    {
        if (_isHide)
            return;

        transform.Translate(Vector3.forward * (_bulletSpeed * Time.deltaTime), Space.Self);
    }

    private void PlayBulletParticle()
    {
        _bulletParticle.Play();
    }

    private void HideBulletSmallModel()
    {
        _isHide = true;
        _bulletSmallModel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent<IShootable>(out IShootable shootable))
        {
            // todo Damage the Shootable Enemy
            shootable.GetShot(_playerData.GunDamage);
        }

        PlayBulletParticle();
        HideBulletSmallModel();
    }
}