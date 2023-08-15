using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStates PlayerState;

    public enum PlayerStates
    {
        Idle,
        Shooting,
        Aiming,
        Win
    }

    [SerializeField, BoxGroup("SETTINGS")] private float _aimSensitivity = 0.3f;
    [SerializeField, BoxGroup("SETTINGS")] private float _aimVerticalLimit = 25f;
    [SerializeField, BoxGroup("SETTINGS")] private float _aimHorizontalLimit = 45f;

    [SerializeField, BoxGroup("SETUP")] private Animator _stickmanAnimator;
    [SerializeField, BoxGroup("SETUP")] private CanvasGroup _aimUICanvas;
    [SerializeField, BoxGroup("SETUP")] private CameraController _cameraController;
    [SerializeField, BoxGroup("SETUP")] private ObjectPool _bulletObjectPool;
    [SerializeField, BoxGroup("SETUP")] private Timer _gunReloadTimer;
    [SerializeField, BoxGroup("SETUP")] private Timer _disableAimTimer;
    [SerializeField, BoxGroup("SETUP")] private Timer _mistouchTimer;
    [SerializeField, BoxGroup("SETUP")] private Transform _playerVisual;
    [SerializeField, BoxGroup("SETUP")] private GameObject _stickmanRig;
    [SerializeField, BoxGroup("SETUP")] private GameObject _stickmanModel;
    [SerializeField, BoxGroup("SETUP")] private PlayerData _playerData;
    private float _aimPositionX = 0f;
    private float _aimPositionY = 0f;
    private Tween _aimUICanvasTween;
    private static readonly int Shooting = Animator.StringToHash("Shooting");
    private static readonly int Idle = Animator.StringToHash("Idle");

    private void Start()
    {
        _disableAimTimer.OnTimerEnded += OnDisableAimTimerEnded;
    }

    private void Update()
    {
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameState.None:
                break;
            case GameState.Start:
                break;
            case GameState.Gameplay:
                HandleTouchInput();
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (_mistouchTimer.IsTimerEnded)
                    {
                        SwitchPlayerState();
                    }

                    break;
                case TouchPhase.Moved:
                    UpdateAimPosition(touch);
                    _disableAimTimer.ResetCurrentTime();
                    //StartCoroutine(SpawnBulletFromObjectPool());
                    break;
                case TouchPhase.Stationary:
                    UpdateAimPosition(touch);
                    _disableAimTimer.ResetCurrentTime();
                    //StartCoroutine(SpawnBulletFromObjectPool());
                    break;
                case TouchPhase.Ended:
                    _mistouchTimer.StartTimer();
                    ResetAimRotation();
                    SwitchPlayerState();
                    _disableAimTimer.ResetCurrentTime();
                    ShootOnReleaseTouch();
                    break;
                case TouchPhase.Canceled:
                    _mistouchTimer.StartTimer();
                    ResetAimRotation();
                    SwitchPlayerState();
                    _disableAimTimer.ResetCurrentTime();
                    ShootOnReleaseTouch();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        LimitAndRotateAim();
    }

    private void SwitchPlayerState()
    {
        switch (PlayerState)
        {
            case PlayerStates.Idle:
                StartCoroutine(ShootingStateCoroutine());
                break;
            case PlayerStates.Shooting:
                StartCoroutine(IdleStateCoroutine());
                break;
            case PlayerStates.Aiming:
                break;
            case PlayerStates.Win:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnDisableAimTimerEnded()
    {
        StartCoroutine(IdleStateCoroutine());
        ResetAimRotation();
    }

    private IEnumerator IdleStateCoroutine()
    {
        PlayerState = PlayerStates.Idle;
        _cameraController.EnableIdleCamera();
        SetAimUICanvas(0f, 0.25f);
        SetStickmanVisual(true);

        yield return new WaitUntil(() => _cameraController.IsCameraBlendCompleted);

        if (PlayerState == PlayerStates.Idle)
        {
            _stickmanAnimator.SetTrigger(Idle);
        }
    }

    private IEnumerator ShootingStateCoroutine()
    {
        if (!_mistouchTimer.IsTimerEnded)
            yield break;

        PlayerState = PlayerStates.Shooting;
        _cameraController.EnableShootingCamera();
        _stickmanAnimator.SetTrigger(Shooting);

        yield return new WaitUntil(() => _cameraController.IsCameraBlendCompleted);

        if (PlayerState == PlayerStates.Shooting)
        {
            PlayerState = PlayerStates.Aiming;
            _disableAimTimer.StartTimer();
            SetStickmanVisual(false);
            SetAimUICanvas(1f, 0.25f);
        }
    }

    private void ShootOnReleaseTouch()
    {
        if (PlayerState == PlayerStates.Aiming)
        {
            Debug.LogError("Shoot");
        }
    }

    private void ResetAimRotation()
    {
        if (PlayerState != PlayerStates.Aiming)
        {
            DOTween.To(() => _aimPositionX, x => _aimPositionX = x, 0, 1f);
            DOTween.To(() => _aimPositionY, y => _aimPositionY = y, 0, 1f);
        }
    }

    private void LimitAndRotateAim()
    {
        _aimPositionY = Mathf.Clamp(_aimPositionY, -_aimVerticalLimit, _aimVerticalLimit);
        _aimPositionX = Mathf.Clamp(_aimPositionX, -_aimHorizontalLimit, _aimHorizontalLimit);
        _playerVisual.transform.localRotation = Quaternion.Euler(-_aimPositionY, _aimPositionX, 0);
    }

    private void UpdateAimPosition(Touch touch)
    {
        _aimPositionX += touch.deltaPosition.x * _aimSensitivity * Time.deltaTime;
        _aimPositionY += touch.deltaPosition.y * _aimSensitivity * Time.deltaTime;
    }

    private void SetAimUICanvas(float endValue, float duration)
    {
        _aimUICanvasTween.Kill();
        _aimUICanvasTween = _aimUICanvas.DOFade(endValue, duration);
    }

    private void SetStickmanVisual(bool value)
    {
        _stickmanModel.SetActive(value);
        _stickmanRig.SetActive(value);
    }

    private void ResetDurationBetweenBullets()
    {
        _gunReloadTimer.SetTimer(_playerData.GunReloadTime);
    }

    private IEnumerator SpawnBulletFromObjectPool()
    {
        while (_gunReloadTimer.CurrentTime > 0)
        {
            yield break;
        }

        ResetDurationBetweenBullets();
        var spawnedBullet = _bulletObjectPool.GetPooledObject(0);
        //spawnedRandomBullet.transform.SetParent(null); // todo it gives null ref?!

        yield return new WaitForSeconds(2f);

        _bulletObjectPool.SetPooledObject(spawnedBullet, 0);
        //spawnedRandomBullet.transform.SetParent(objectPool.transform); 
        spawnedBullet.transform.ResetLocalPos();
        spawnedBullet.transform.ResetLocalRot();
    }
}