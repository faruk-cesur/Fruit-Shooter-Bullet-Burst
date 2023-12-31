using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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

    [SerializeField, BoxGroup("Aim Control")] private float _aimVerticalLimit = 45f;
    [SerializeField, BoxGroup("Aim Control")] private float _aimHorizontalLimit = 45f;
    [SerializeField, BoxGroup("Targeted Fruit Amount")] private int _targetedFruitAmount;
    [SerializeField, BoxGroup("Targeted Fruit Amount"), ReadOnly] private int _currentFruitAmount;

    [SerializeField, BoxGroup("SETUP")] private GameplayData _gameplayData;
    [SerializeField, BoxGroup("SETUP")] private Animator _stickmanAnimator;
    [SerializeField, BoxGroup("SETUP")] private CanvasGroup _aimUICanvas;
    [SerializeField, BoxGroup("SETUP")] private CameraController _cameraController;
    [SerializeField, BoxGroup("SETUP")] private ObjectPool _bulletObjectPool;
    [SerializeField, BoxGroup("SETUP")] private Timer _disableAimTimer;
    [SerializeField, BoxGroup("SETUP")] private Timer _mistouchTimer;
    [SerializeField, BoxGroup("SETUP")] private Transform _playerVisual;
    [SerializeField, BoxGroup("SETUP")] private Transform _bulletSpawnPosition;
    [SerializeField, BoxGroup("SETUP")] private Transform _shootingCamera;
    [SerializeField, BoxGroup("SETUP")] private GameObject _stickmanRig;
    [SerializeField, BoxGroup("SETUP")] private GameObject _stickmanModel;
    [SerializeField, BoxGroup("SETUP")] private AudioClip _gunShootAudio;
    [SerializeField, BoxGroup("SETUP")] private AudioClip _gunReloadAudio;
    [SerializeField, BoxGroup("SETUP")] private TextMeshProUGUI _bulletAmountText;
    [SerializeField, BoxGroup("SETUP")] private TextMeshProUGUI _targetedFruitAmountText;
    [SerializeField, BoxGroup("SETUP")] private CountDown _countDown;
    private float _aimPositionX = 0f;
    private float _aimPositionY = 0f;
    private int _currentBulletAmount;
    private bool _preventAimAfterReload;
    private Tween _aimUICanvasTween;
    private static readonly int Shooting = Animator.StringToHash("Shooting");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Reloading = Animator.StringToHash("Reloading");
    private static readonly int Win = Animator.StringToHash("Win");

    private void Start()
    {
        _disableAimTimer.OnTimerEnded += DisableAim;
        SettingsManager.Instance.OnSaveSettings += ReloadBulletAmount;
        CurrencyManager.Instance.OnMoneyChanged += SetTargetedFruitAmountText;
        SetStartingTargetedFruitAmount();
        ReloadBulletAmount();
    }

    private void OnDestroy()
    {
        SettingsManager.Instance.OnSaveSettings -= ReloadBulletAmount;
        CurrencyManager.Instance.OnMoneyChanged -= SetTargetedFruitAmountText;
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

            if (!_preventAimAfterReload)
            {
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
                        StartCoroutine(GodMode());
                        break;
                    case TouchPhase.Stationary:
                        UpdateAimPosition(touch);
                        _disableAimTimer.ResetCurrentTime();
                        StartCoroutine(GodMode());
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
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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

    private void DisableAim()
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

        if (PlayerState == PlayerStates.Idle && !IsCurrentBulletAmountZero)
        {
            _stickmanAnimator.SetTrigger(Idle);
        }
    }

    private IEnumerator ShootingStateCoroutine()
    {
        if (!_mistouchTimer.IsTimerEnded || IsCurrentBulletAmountZero)
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
            _preventAimAfterReload = false;
        }
    }

    private void ShootOnReleaseTouch()
    {
        if (PlayerState == PlayerStates.Aiming && !IsCurrentBulletAmountZero && _gameplayData.GodMode <= 0)
        {
            StartCoroutine(SpawnBulletFromObjectPool());
            _shootingCamera.DOShakeRotation(_gameplayData.AimShakeDuration, _gameplayData.AimShakeStrength, _gameplayData.AimShakeVibrato, _gameplayData.AimShakeRandomness);
            AudioManager.Instance.PlayAudio(_gunShootAudio, 1f, 0, false);

            if (SettingsManager.Instance.IsVibrationActivated)
            {
                Vibration.Vibrate();
            }
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
        _aimPositionX += touch.deltaPosition.x * _gameplayData.AimSensitivity * Time.deltaTime;
        _aimPositionY += touch.deltaPosition.y * _gameplayData.AimSensitivity * Time.deltaTime;
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

    private IEnumerator SpawnBulletFromObjectPool()
    {
        _currentBulletAmount--;
        _bulletAmountText.text = "x" + _currentBulletAmount;
        if (IsCurrentBulletAmountZero)
        {
            StartCoroutine(ReloadGunBullets());
        }

        var spawnedBullet = _bulletObjectPool.GetPooledObject(0);
        spawnedBullet.transform.position = _bulletSpawnPosition.position;
        spawnedBullet.transform.rotation = _bulletSpawnPosition.rotation;

        yield return new WaitForSeconds(2f);

        _bulletObjectPool.SetPooledObject(spawnedBullet, 0);
        spawnedBullet.transform.ResetLocalPos();
        spawnedBullet.transform.ResetLocalRot();
    }

    private bool IsCurrentBulletAmountZero => _currentBulletAmount <= 0;

    private IEnumerator ReloadGunBullets()
    {
        _preventAimAfterReload = true;
        DisableAim();
        _stickmanAnimator.SetTrigger(Reloading);
        AudioManager.Instance.PlayAudio(_gunReloadAudio, 1f, 0, false);
        yield return new WaitForSeconds(_gameplayData.GunReloadTime);
        ReloadBulletAmount();
        StartCoroutine(ShootingStateCoroutine());
    }

    private void ReloadBulletAmount()
    {
        _currentBulletAmount = _gameplayData.BulletAmount;
        _bulletAmountText.text = "x" + _currentBulletAmount;
    }

    private IEnumerator GodMode()
    {
        if (_gameplayData.GodMode <= 0)
            yield break;

        _bulletAmountText.text = "GOD MODE";

        var spawnedBullet = _bulletObjectPool.GetPooledObject(0);
        spawnedBullet.transform.position = _bulletSpawnPosition.position;
        spawnedBullet.transform.rotation = _bulletSpawnPosition.rotation;

        yield return new WaitForSeconds(2f);

        _bulletObjectPool.SetPooledObject(spawnedBullet, 0);
        spawnedBullet.transform.ResetLocalPos();
        spawnedBullet.transform.ResetLocalRot();
    }

    private void SetStartingTargetedFruitAmount()
    {
        _targetedFruitAmount = Random.Range(10, 111);
        _countDown.TotalTime = _targetedFruitAmount * 4f;
        _targetedFruitAmountText.text = _currentFruitAmount + "/" + _targetedFruitAmount;
    }
    private void SetTargetedFruitAmountText()
    {
        if (GameManager.Instance.CurrentGameState == GameState.Gameplay)
        {
            _currentFruitAmount++;
            _targetedFruitAmountText.text = _currentFruitAmount + "/" + _targetedFruitAmount;
        }

        if (_currentFruitAmount >= _targetedFruitAmount)
        {
            GameManager.Instance.Win(100);
            PlayerState = PlayerStates.Win;
            _stickmanAnimator.SetTrigger(Win);
            ResetAimRotation();
            _playerVisual.DOLocalRotate(Vector3.zero, 0.5f);
        }
    }
}