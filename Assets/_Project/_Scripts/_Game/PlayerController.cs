using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public PlayerStates PlayerState;

    public enum PlayerStates
    {
        Idle,
        Shooting
    }

    [SerializeField] private Transform _aimGunHead;
    [SerializeField] private SkinnedMeshRenderer _gunMeshRenderer;
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private float _gunAimSensitivity = 0.3f;
    private float _durationBetweenBullets;
    private float _gunAimX = 0f;
    private float _gunAimY = 0f;

    private void Update()
    {
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameState.None:
                break;
            case GameState.Start:
                break;
            case GameState.Gameplay:
                SwitchPlayerState();
                DecreaseReloadBulletTimer();
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SwitchPlayerState()
    {
        switch (PlayerState)
        {
            case PlayerStates.Idle:
                break;
            case PlayerStates.Shooting:
                InputForAimAndShoot();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void InputForAimAndShoot()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                case TouchPhase.Moved:
                    _gunAimX += touch.deltaPosition.x * _gunAimSensitivity;
                    _gunAimY += touch.deltaPosition.y * _gunAimSensitivity;
                    StartCoroutine(SpawnBulletFromObjectPool());
                    break;
                case TouchPhase.Stationary:
                    StartCoroutine(SpawnBulletFromObjectPool());
                    break;
                case TouchPhase.Ended:
                    break;
                case TouchPhase.Canceled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        _gunAimY = Mathf.Clamp(_gunAimY, -20, 20);
        _gunAimX = Mathf.Clamp(_gunAimX, -45, 45);

        _aimGunHead.transform.localRotation = Quaternion.Euler(-_gunAimY, _gunAimX, 0);
    }

    private void DecreaseReloadBulletTimer()
    {
        _durationBetweenBullets -= Time.deltaTime;
    }

    private void ResetDurationBetweenBullets()
    {
        _durationBetweenBullets = _playerData.BulletReloadDuration;
    }


    private IEnumerator SpawnBulletFromObjectPool()
    {
        while (_durationBetweenBullets > 0)
        {
            yield break;
        }

        ResetDurationBetweenBullets();
        var objectPool = ObjectPool.Instance;
        var randomIndex = Random.Range(0, objectPool.Pools.Length);
        var spawnedRandomBullet = objectPool.GetPooledObject(randomIndex);
        //spawnedRandomBullet.transform.SetParent(null); // todo it gives null ref?!

        yield return new WaitForSeconds(2f);

        objectPool.SetPooledObject(spawnedRandomBullet, randomIndex);
        //spawnedRandomBullet.transform.SetParent(objectPool.transform); 
        spawnedRandomBullet.transform.ResetLocalPos();
        spawnedRandomBullet.transform.ResetLocalRot();
    }
}