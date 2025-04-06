using UnityEngine;


/// <summary>
/// CinemachineのHardLookAtを使用したロックオンカメラ
/// </summary>
public class LockOnCamera_Transposer_HardLookAt : LockOnCameraBase
{
    /// <summary>
    /// HardLock用のLookAtようオブジェクト
    /// </summary>
    [SerializeField] private LookAtTarget _lookAtTarget;

    //Lockonパラメーター
    [SerializeField] private GetLockOnTarget.Param param;

    /// <summary>
    /// LockOn状態
    /// </summary>
    public bool IsLockOn { get; private set; }



    private IGetLockOnTarget __getLockOnTarget = null;
    private IGetLockOnTarget _getLockOnTarget
    {
        get
        {
            if (__getLockOnTarget == null)
            {
                __getLockOnTarget = new GetLockOnTarget(param);
            }
            return __getLockOnTarget;
        }
        set
        {
            __getLockOnTarget = value;
        }
    }


    //LockOnCamera 切り替えよう
    private readonly int LockonCameraEnablePriority = 11;
    private readonly int LockonCameraDisablePriority = 0;


    private void OnValidate()
    {
        __getLockOnTarget = new GetLockOnTarget(param);
    }

    /// <summary>
    /// ロックオン処理
    /// </summary>
    /// <returns>
    /// LockOnの成功か否か
    /// </returns>
    /// <param name="target">Nullであればターゲット取得処理が走る</param>
    public override bool LockOn(Transform target = null)
    {
        if (target) Target = target;
        else Target = _getLockOnTarget.GetTarget();

        if (!Target)
        {
            Debugger.Log($"{typeof(LockOnCamera_Transposer_HardLookAt).Name} : {nameof(LockOn)} : Not Found Enable Lockon Object");
            IsLock = false;
            return false;
        }

        IsLock = true;
        //chinemachineの設定
        CinemachineVirtualCamera.ForceCameraPosition(param.Camera.position, param.Camera.transform.rotation);
        CinemachineVirtualCamera.Priority = LockonCameraEnablePriority;
        CinemachineVirtualCamera.LookAt = Target.transform;

        _lookAtTarget.Target = Target;
        return true;
    }

    public override void UnLock()
    {
        IsLock = false;
        _lookAtTarget.Target = null;
        CinemachineVirtualCamera.Priority = LockonCameraDisablePriority;
    }

}
