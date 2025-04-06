using Cinemachine;
using UnityEngine;

/// <summary>
/// ロックオンカメラの基底クラス
/// </summary>
public abstract class LockOnCameraBase : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
    public CinemachineVirtualCamera CinemachineVirtualCamera => _cinemachineVirtualCamera;

    public bool IsLock { get; protected set; } = false;
    public Transform Target { get; protected set; }


    public abstract bool LockOn(Transform target = null);
    public abstract void UnLock();
}
