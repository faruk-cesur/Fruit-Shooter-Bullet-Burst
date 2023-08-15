using System;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _idleCamera;
    [SerializeField] private CinemachineVirtualCamera _shootingCamera;
    [SerializeField] private CinemachineBrain _cinemachineBrain;
    public bool IsCameraBlendCompleted => _cinemachineBrain.IsBlending && (_cinemachineBrain.ActiveBlend.TimeInBlend + 0.05f >= _cinemachineBrain.ActiveBlend.Duration || !_cinemachineBrain.ActiveBlend.IsValid);

    public void EnableShootingCamera()
    {
        _idleCamera.Priority = 0;
        _shootingCamera.Priority = 1;
    }

    public void EnableIdleCamera()
    {
        _idleCamera.Priority = 1;
        _shootingCamera.Priority = 0;
    }
}