using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _idleCamera;
    [SerializeField] private CinemachineVirtualCamera _shootingCamera;

    public void EnableShootingCamera()
    {
        _idleCamera.gameObject.SetActive(true);
        _shootingCamera.gameObject.SetActive(false);
    }

    public void EnableIdleCamera()
    {
        _shootingCamera.gameObject.SetActive(true);
        _idleCamera.gameObject.SetActive(false);
    }
}