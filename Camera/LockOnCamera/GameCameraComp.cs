using Cinemachine;
using LitMotion;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// InGame中に使用するPlayerを映すカメラ
/// </summary>
public class GameCameraComp : MonoBehaviour
{
    [SerializeField] private CharacterParam _followCharacter;
    [Header("プレーヤー攻撃時のめり込み対策 カメラのフォーカストランスフォーム , 無設定だと Follow Character が設定される")]
    [SerializeField] private Transform followAtTransform;


    public Transform FollowAtTransform
    {
        get
        {
            if (followAtTransform)
            {
                return followAtTransform;
            }
            else
            {
                return _followCharacter.Transform;
            }
        }
    }
    [SerializeField] private CinemachineBrain _playerCameraBrain;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private CinemachineVirtualCamera _freeLookCamera;
    [SerializeField] private LockOnCameraBase _lockOnCamera;
    [Header("アンロック時のカメラ入力無効時間")]
    [SerializeField] private float _disableCameraInputTime = 0.5f;

    [Header("ロックオンカメラのオフセット変更関係")]
    [SerializeField] private float _lockCameraOffsetChangeSpeed = 1f;
    [SerializeField] private Ease _lockCameraOffsetChangeEaseType = Ease.Linear;
    [SerializeField] private Vector3 _lockCameraNormalOffset = Vector3.zero;
    [SerializeField] private Vector3 _lockCameraFarOffset = Vector3.zero;
    public enum LockCameraOffsetType
    {
        Normal,
        Far,
    }


    [Header("頭上のアンロック角度")]
    [SerializeField] private float _headOnUnlockAngle = 10f;

    [Header("ゴール出現時のロックするまでの少しの待ち時間")]
    [SerializeField] private float _goalLockOnWaiteTime = 1f;

    [Header("delayLockON時のLockOnにかかる時間")]
    [SerializeField] private float _delayLockOnBlendTime = 2.0f;

    private Vector3 _defaultFreeLookDump;
    private Vector3 _defaultLockOnDump;


    public Transform Camera => _playerCamera.transform;


    public UnityEvent OnLockOn;
    public UnityEvent OnUnlock;

    //FreeLook用
    private CinemachineFramingTransposer _freeLook_FramingTransposer;
    private CinemachinePOV _freeLookPov;
    //LockOn用
    private CinemachineTransposer _lockOn_Transposer;

    /// <summary>
    /// LockOn状態
    /// </summary>
    public bool IsLockOn { get; private set; }


    public Transform TargetObject
    {
        get { return _lockOnCamera.Target; }
    }

    /// <summary>
    /// 直上処理用のターゲットのキャッシュ
    /// </summary>
    private Transform _overHeadTarget;

    /// <summary>
    /// 初期状態のBlendTime
    /// </summary>
    private float _defaultBlendTime;

    public bool IsOverHead { get; private set; } = false;


    private void Awake()
    {
        _freeLook_FramingTransposer = _freeLookCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _freeLookPov = _freeLookCamera.GetCinemachineComponent<CinemachinePOV>();

        _lockOn_Transposer = _lockOnCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();

        _defaultFreeLookDump = new Vector3(_freeLook_FramingTransposer.m_XDamping, _freeLook_FramingTransposer.m_YDamping, _freeLook_FramingTransposer.m_ZDamping);
        _defaultLockOnDump = new Vector3(_lockOn_Transposer.m_XDamping, _lockOn_Transposer.m_YDamping, _lockOn_Transposer.m_ZDamping);

        _defaultBlendTime = _playerCameraBrain.m_DefaultBlend.m_Time;


        //死亡時にはアンロック
        _followCharacter.Hp.OnDeath.AddListener(() => UnLock());
        //Bossが存在する場合はメインステージ侵入時にBoss Lock ON クリア時にはゴールにLockOn 
        if (MainGameSystem.Instance.Boss != null) MainGameSystem.Instance.OnMainStageEntry.AddListener(() =>
        {
            if (MainGameSystem.Instance.IsClearStage)
            {
                LockOn(MainGameSystem.Instance.Goal.LockOnTarget);
            }
            else LockOn(MainGameSystem.Instance.Boss.transform);
        });
        //ゴール出現しには , ゴールにLockOn (メインステージにいれば(敵の消滅と同時に死んでなければ))
        MainGameSystem.Instance.OnStageClear.AddListener(
            () =>
            {
                DelayLockOn(MainGameSystem.Instance.Goal.LockOnTarget, _goalLockOnWaiteTime);
            }
            );

        StartCoroutine(BehavirCoroutine());

    }


    private IEnumerator BehavirCoroutine()
    {
        while (true)
        {
            yield return null;
            //ターゲットがなくなった時 , 倒したときのノックバックとかの関係ですぐにUnLockすくのでDelayしてUnlockする
            if (_lockOnCamera.Target && _lockOnCamera.Target.gameObject.activeSelf == false && IsLockOn)
            {
                yield return UnLock();
            }
            else if (_lockOnCamera.Target == null && IsLockOn)
            {
                yield return UnLock();
            }
            // 直上処理
            if (IsLockOn && CheckOverhead())
            {
                _overHeadTarget = TargetObject;
                IsOverHead = true;
                yield return UnLock();
            }

            //直上脱出再ロック処理
            if (IsOverHead && !CheckOverhead())
            {
                IsOverHead = false;
                //    yield return LockOn(_overHeadTarget);
            }

            yield return null;
        }
    }


    private Coroutine _delayLockOnCoroutine;
    /// <summary>
    /// 1ボタンでロックオンの切り替えをする処理
    /// </summary>
    public Coroutine DelayLockOn(Transform target, float delay)
    {
        if (_delayLockOnCoroutine != null)
        {
            StopCoroutine(_delayLockOnCoroutine);
            _delayLockOnCoroutine = null;
        }
        return _delayLockOnCoroutine = StartCoroutine(DelayLockOnCorutine(delay, target));
    }

    private IEnumerator DelayLockOnCorutine(float delay, Transform target = null)
    {

        _playerCameraBrain.m_DefaultBlend.m_Time = _delayLockOnBlendTime;
        yield return StaticCommonParams.Yielders.Get(delay);
        if (MainGameSystem.Instance.Status != MainGameSystem.GameStatus.Boss) yield break;
        if (_followCharacter.Hp.Hp <= 0) yield break;

        if (target == null) yield break;
        yield return LockOn(target);

        _playerCameraBrain.m_DefaultBlend.m_Time = _defaultBlendTime;

    }


    /// <summary>
    /// ロックオン処理
    /// </summary>
    /// <param name="target">Nullであればターゲット取得処理が走る</param>
    /// <returns></returns>
    public Coroutine LockOn(Transform target = null)
    {
        return StartCoroutine(LockOnCoroutine(target));
    }

    private IEnumerator LockOnCoroutine(Transform target = null)
    {
        //既にロックオンしている場合かつ target が未指定の場合は コントローラーからの Unlock命令なので Unlock()
        if (IsLockOn && target == null)
        {
            yield return UnLock();
            yield break;
        }

        //ロック恩済み + ターゲット指定はリターゲットなのでアンロック処理が必要
        if (IsLockOn)
        {
            yield return UnLock();
        }
        //Live Blend 進行中に Vcam を変更するとワープしてカクついたように見えてしまうので
        yield return new WaitUntil(() => _playerCameraBrain.IsBlending == false);
        if (_lockOnCamera.LockOn(target))
        {
            OnLockOn?.Invoke();
            IsLockOn = true;

        }
    }


    public Coroutine UnLock()
    {

        if (IsLockOn == false) return null;

        IsLockOn = false;
        OnUnlock?.Invoke();
        ResetFreeLookCamera(_playerCamera.transform);
        _lockOnCamera.UnLock();
        /// アンロック時にカメラが動くと(マウス , コントローラー , ロック処理)が荒ぶるので、アンロック時は少しの間入力によるカメラ移動をやめる
        return StartCoroutine(DisablePlayerInputForShotTime());
    }


    /// <summary>
    /// 切り替え時にLockCameraと同じ位置にFreeLookカメラを移動する
    /// </summary>
    /// <param name="resetTransform"></param>
    private void ResetFreeLookCamera(Transform resetTransform)
    {
        _freeLookCamera.ForceCameraPosition(resetTransform.position, resetTransform.rotation);
    }


    /// <summary>
    /// ロックオンカメラのオフセット変更
    /// </summary>
    /// <param name="offsetType"></param>
    public void ChangeLockCameraOffset(LockCameraOffsetType offsetType)
    {
        switch (offsetType)
        {
            case LockCameraOffsetType.Normal:
                SetLockCameraOffset(_lockCameraNormalOffset);
                break;
            case LockCameraOffsetType.Far:
                SetLockCameraOffset(_lockCameraFarOffset);
                break;
            default:
                SetLockCameraOffset(_lockCameraNormalOffset);
                break;
        }
    }
    private MotionHandle _lockCameraOffsetChange;
    private void SetLockCameraOffset(Vector3 offset)
    {
        if (_lockCameraOffsetChange.IsActive())
        {
            _lockCameraOffsetChange.Cancel();
        }
        Vector3 nowOffset = _lockOn_Transposer.m_FollowOffset;
        float changeTiem = (offset - nowOffset).magnitude / _lockCameraOffsetChangeSpeed;
        _lockCameraOffsetChange = LMotion.Create(nowOffset, offset, changeTiem).WithEase(_lockCameraOffsetChangeEaseType).Bind(value => _lockOn_Transposer.m_FollowOffset = value);
    }

    /// <summary>
    /// 少しの間プレイヤー入力によるカメラ移動を無効
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisablePlayerInputForShotTime()
    {
        CinemachineInputProvider cinemachineInputProvider = _freeLookCamera.GetComponent<CinemachineInputProvider>();
        cinemachineInputProvider.enabled = false;
        yield return StaticCommonParams.Yielders.Get(_disableCameraInputTime);
        cinemachineInputProvider.enabled = true;
    }


    /// <summary>
    ///直上の場合判定
    /// </summary>
    /// <returns></returns>
    private bool CheckOverhead()
    {
        Vector3 overHeadVector = Vector3.up;
        float overHeadDistance = Vector3.Dot(FollowAtTransform.position - TargetObject.position, overHeadVector);
        float angle = Vector3.Angle(FollowAtTransform.position - TargetObject.position, overHeadVector);

        // Debugger.Log($"OverHeadDistance : {overHeadDistance} , OverHeadAngle : {angle}");

        if (angle < _headOnUnlockAngle)
        {
            return true;
        }
        return false;
    }
    public Transform GetActiveCameraTransform()
    {
        return _playerCameraBrain.ActiveVirtualCamera.VirtualCameraGameObject.transform;
    }




}
