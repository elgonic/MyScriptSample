using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


/// <summary>
/// Playerクラス
/// </summary>
[RequireComponent(typeof(CharacterParam))]
public class Player : MonoBehaviour
{
    [Header("Playerの3Dモデル")]
    [SerializeField] private GameObject _playerObject;
    public GameObject PlayerGameObject { get { return _playerObject; } }

    [Header("障害物検知用のRay情報")]
    [SerializeField] private RayLength _checkObstacleRay;
    [Header("障害物検知用のSphere半径")]
    [SerializeField] private float _checkObstacleRadius = 0.4f;

    [Header("Respawn")]
    [SerializeField] private float _respawnDelayTime = 1f;

    [SerializeField] private PlayerInput _playerInput;
    [Header("UI")]
    [SerializeField] private InGameUIController _uIController;
    [Header("Camera関係")]
    [SerializeField] private GameCameraComp _gameCamera;
    [Header("入力ドリフト対処用バッファー")]
    [SerializeField] private float _inputThreshold = 0.125f;
    [SerializeField] private bool _debugMode = false;
    [Header("Move")]
    [SerializeField] private RigidVelocityAccelerationMove.Params _accelerationMoveParams;
    [SerializeField] private RigidVelocityAccelerationMove.Params _accelerationDashParams;

    public float MoveSpeed => _accelerationMoveParams.MoveSpeed;

    [SerializeField] private float _dashTime;
    [SerializeField] private float _dashIntervalTime;

    public UnityEvent<Vector3> OnDash;

    [Header("Parry")]
    ///<summary>
    ///パリィ出来る時間
    ///</summary>
    ///<remarks>
    /// ダッシュ時間より多くするとダッシュ後も無敵になるで注意
    ///</remarks>
    [SerializeField] private float _canParryTime = 0.2f;
    [SerializeField] private PassThrough.Params _passThroughParams;
    [SerializeField] private Material _parryMaterial;


    public UnityEvent<Vector3> OnParryBegin;
    public UnityEvent OnParryEnd;

    [Header("Attack")]
    private AttackObjectBase _attackBase;
    [Header("KnockBack")]
    [SerializeField] private KnockBack.Params _knockBackParams;

    public UnityEvent OnKnockBack;

    [Header("Boss Attack NockBack")]
    [SerializeField] private KnockBack.Params _boosKnockBackParams;
    [Header("Jump")]
    [SerializeField] private UrushiJumpParams _jumpParam;

    [Header("Respawn")]
    private Vector3 _respawnPosition;
    private Quaternion _respawnRotation;
    private float _lowerLimitForRespawn = -50;

    [Header("状態")]
    public bool IsFreeze = false;


    /// <summary>
    /// 当たり判定
    /// </summary>
    private ColliderDelegate _hitColliderDelegate;

    private bool _canParryFlag = false;
    public bool CanParryFlag => _canParryFlag;
    private bool _isParry = false;

    private float _beforeDashTime = 0;
    private float _dashTimeWatcher = 0;
    private bool _isDash = false;
    private bool _doneOnDash = false;

    private Vector2 _dashDirection = Vector2.zero;

    private bool _isKnockBack = false;



    //イベント
    public UnityEvent OnRespawn;



    /// <summary>
    /// 通常移動挙動
    /// </summary>
    public IMove NormalMove { get; private set; }
    /// <summary>
    /// ダッシュ移動挙動
    /// </summary>
    public IMove DashMove { get; private set; }
    /// <summary>
    /// ジャンプ挙動
    /// </summary>
    public IJump Jump { get; private set; }
    /// <summary>
    /// 通常ノックバック挙動
    /// </summary>
    public ICoroutineMove NormalKnockBack { get; private set; }
    /// <summary>
    /// Boss接触時ノックバック
    /// </summary>
    public ICoroutineMove BossKnockBack { get; private set; }
    /// <summary>
    /// 反射Parry挙動
    /// </summary>
    public IParry BounceBack { get; private set; }
    /// <summary>
    /// すり抜けParry挙動
    /// </summary>
    public IParry PassThrough { get; private set; }

    private GetInputValue _inputValue;

    public bool DebugMode => _debugMode;


    /// <summary>
    /// キャラクターのパラメーター
    /// </summary>
    public CharacterParam CharacterParam { get; private set; }

    private void OnValidate()
    {
        if (Jump != null && _jumpParam != null && CharacterParam && _playerInput) Jump = new UrushiJump(_jumpParam, CharacterParam, _playerInput);
    }

    private void Awake()
    {
        if (!_gameCamera) _gameCamera = GameObject.FindFirstObjectByType<GameCameraComp>();
        if (!_playerInput) _playerInput = GetComponent<PlayerInput>();



        _attackBase = GetComponent<AttackObjectBase>();

        CharacterParam = GetComponent<CharacterParam>();
        CharacterParam.Hp.OnDeath.AddListener(Respawn);
        CharacterParam.Hp.OnChange.AddListener(() =>
        {
            if (CharacterParam.Hp.Hp <= 0)
            {
                _playerObject.gameObject.SetActive(false);
                _hitColliderDelegate.gameObject.SetActive(false);
            }
            else
            {
                _playerObject.gameObject.SetActive(true);
                _hitColliderDelegate.gameObject.SetActive(true);
            }
        });

        NormalMove = new RigidVelocityAccelerationMove(_accelerationMoveParams, CharacterParam.Rb);
        DashMove = new RigidVelocityAccelerationMove(_accelerationDashParams, CharacterParam.Rb);
        Jump = new UrushiJump(_jumpParam, CharacterParam, _playerInput);
        NormalKnockBack = new KnockBack(_knockBackParams, CharacterParam);
        BossKnockBack = new KnockBack(_boosKnockBackParams, CharacterParam);
        BounceBack = new BounceBack(new BounceBack.Params(NormalKnockBack, CharacterParam));
        PassThrough = new PassThrough(_passThroughParams);
        _inputValue = new GetInputValue(_playerInput, _inputThreshold);

    }

    private void Start()
    {
        _respawnPosition = transform.position;
        _respawnRotation = transform.rotation;

        _hitColliderDelegate = GetComponentInChildren<ColliderDelegate>();

        if (_hitColliderDelegate)
        {
            _hitColliderDelegate.OnTriggerEnterAction.AddListener(PlayerColliderTriggerHit);
        }

        else Debugger.LogWarning($"{gameObject.name}の子要素に当たり判定用{typeof(ColliderDelegate).Name}がアタッチされたコライダーがないです");

        //クリア時にPlayerは動作不能させるさせる
        MainGameSystem.Instance.OnChangeNextStage.AddListener(() =>
        {
            StopAllCoroutines();
            CharacterParam.Freeze();
        });

    }


    private void FixedUpdate()
    {
        CheckPositionLowerLimit();
        if (CharacterParam.IsFreeze) return;
        if (_isKnockBack) return;
        if (_isDash) Dash();
        else
        {
            _dashTimeWatcher = 0;
            Move();
        }

        //慣性で動いてるとき用の壁判定抜け + 壁は張り付き 防止
        Vector3 moveXZValue = new Vector3(CharacterParam.Rb.velocity.x, 0, CharacterParam.Rb.velocity.z);
        if (moveXZValue != Vector3.zero)
        {
            CharacterParam.Rb.velocity = MoveCommonProcess.AntiObstacleProcess(CharacterParam.Rb.velocity, _checkObstacleRay, _checkObstacleRadius, _accelerationMoveParams.LimitSloapAngle, new string[] { StaticCommonParams.WALL_TAG, StaticCommonParams.GROUND_TAG });
        }


    }

    private void OnEnable()
    {
        _playerInput.actions[StaticCommonParams.JUMP_INPUT].performed += OnJump;
        _playerInput.actions[StaticCommonParams.DASH_INPUT].performed += OnAttack;
        _playerInput.actions[StaticCommonParams.LOCKONTOGGLE_INPUT].performed += OnLockon;
        _playerInput.actions[StaticCommonParams.TOGGLEUI_INPUT].performed += OnToggleUI;
        _playerInput.actions[StaticCommonParams.PAUSE_INPUT].performed += OnPause;


    }

    private void OnDisable()
    {
        _playerInput.actions[StaticCommonParams.JUMP_INPUT].performed -= OnJump;
        _playerInput.actions[StaticCommonParams.DASH_INPUT].performed -= OnAttack;
        _playerInput.actions[StaticCommonParams.LOCKONTOGGLE_INPUT].performed -= OnLockon;
        _playerInput.actions[StaticCommonParams.TOGGLEUI_INPUT].performed -= OnToggleUI;
        _playerInput.actions[StaticCommonParams.PAUSE_INPUT].performed -= OnPause;

    }


    /// <summary>
    /// ポジション下限確認
    /// </summary>
    private void CheckPositionLowerLimit()
    {
        if (transform.position.y <= _lowerLimitForRespawn)
        {
            //死ぬ
            CharacterParam.Hp.Damage(CharacterParam.Hp.Hp);
        }
    }

    private void Respawn()
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        CharacterParam.Rb.velocity = Vector3.zero;


        yield return StaticCommonParams.Yielders.Get(_respawnDelayTime);


        OnRespawn?.Invoke();
        CharacterParam.Hp.Reset();

        transform.position = _respawnPosition;
        transform.rotation = _respawnRotation;

        MainGameSystem.Instance.PlayerRespown();
    }





    private void OnLockon(InputAction.CallbackContext context)
    {
        if (!IsEnableMoveInput()) return;

        //Bossとの敵対時だけ LockOnできる
        if (MainGameSystem.Instance.Status != MainGameSystem.GameStatus.Boss) return;
        _gameCamera.LockOn();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!IsEnableMoveInput()) return;
        if (IsFreeze) return;
        if (!_isKnockBack)
        {
            Jump?.Jump();
        }
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (!IsEnableMoveInput()) return;


        if (!_isDash && !_isKnockBack && Time.time - _beforeDashTime > _dashIntervalTime)
        {
            EnableParryFlag();
            _isDash = true;
            _doneOnDash = false;
            _dashDirection = _inputValue.GetInputMoveValueNomalize();
            _beforeDashTime = Time.time;
            _dashTimeWatcher = 0;
        }
    }

    private void OnToggleUI(InputAction.CallbackContext context)
    {
        if (!IsEnableMoveInput()) return;
        _uIController.AnnouncementUIManager.Toggle();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        MainGameSystem.Instance.Pause();
    }


    private bool IsEnableMoveInput()
    {
        if (MainGameSystem.Instance.IsPause) return false;
        return true;
    }

    private void Dash()
    {
        Vector2 direction = GetMoveDirection(_dashDirection);

        Vector3 moveVector = DashMove.Move(direction);

        if (_doneOnDash == false && (new Vector2(moveVector.x, moveVector.z) != Vector2.zero))
        {
            _doneOnDash = true;
            OnDash?.Invoke(moveVector);
        }

        _dashTimeWatcher += Time.deltaTime;

        if (_dashTimeWatcher >= _dashTime)
        {
            _isDash = false;

        }

    }

    private void PlayerColliderTriggerHit(Collider hitObject, Transform hitter = null)
    {
        HitProcess(new HitObject(CharacterParam, hitObject.gameObject));
    }


    private void HitProcess(HitObject hitObject)
    {

        //ノックバック中は無視(多段ヒット防止もかねて)
        if (_isKnockBack) return;


        //敵 or 攻撃に接触 , 自傷防止
        if (hitObject.Attack && hitObject.IsDamegeObject)
        {
            HitAttackProcess(hitObject.Attack);
        }


        //障害物に接触
        if (hitObject.Obstacle)
        {
            HitObstacleProcess(hitObject.Obstacle);
        }
    }

    private void HitAttackProcess(AttackObjectBase hitAttackObject)
    {
        if (_canParryFlag)
        {
            StartCoroutine(Parry(hitAttackObject));
        }
        else if (!_isParry && !_isKnockBack)
        {
            StartCoroutine(KnockBack(NormalKnockBack, hitAttackObject.GetHitedObjectKnockBackDirection(gameObject)));
            hitAttackObject.Hit(gameObject);
            Damaged(hitAttackObject.AttackData);
        }
    }
    private void HitObstacleProcess(GameObject hitCollision)
    {
        Vector3 knockBackDirection;
        knockBackDirection = -CharacterParam.MoveDirection;
        knockBackDirection = knockBackDirection.normalized;
        StartCoroutine(KnockBack(NormalKnockBack, knockBackDirection));
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    private void Damaged(AttackData attackData)
    {
        CharacterParam.Damage(1, _attackBase);
    }



    private Coroutine _parryCoroutine;
    private void EnableParryFlag()
    {
        if (_parryCoroutine != null)
        {
            StopCoroutine(_parryCoroutine);
        }
        _parryCoroutine = StartCoroutine(EnableParryFlagCoroutine());
    }

    private void DisableParryFlag()
    {
        if (_parryCoroutine != null)
        {
            StopCoroutine(_parryCoroutine);
        }
        _canParryFlag = false;
    }

    private IEnumerator EnableParryFlagCoroutine()
    {
        _canParryFlag = true;
        yield return StaticCommonParams.Yielders.Get(_canParryTime);
        _canParryFlag = false;

        //PassThrough用
        if (_isParry)
        {
            OnParryEnd?.Invoke();
            _isParry = false;
        }

    }


    private IEnumerator Parry(AttackObjectBase hitObject)
    {
        if (_isParry) yield break;
        //ここでDash を止めておかないと , ParryでKnockBackしない
        _isDash = false;
        _isParry = true;

        if (hitObject.ParryMode == AttackObjectBase.ParryKinds.Bounce)
        {
            OnParryBegin?.Invoke(CharacterParam.Transform.position);
            _isKnockBack = true;
            hitObject.Parryed(_attackBase);
            yield return BounceBack.Parry(hitObject);



            DisableParryFlag();
            _isKnockBack = false;
            OnParryEnd?.Invoke();
            _isParry = false;
        }
        else if (hitObject.ParryMode == AttackObjectBase.ParryKinds.PassThrough)
        {
            OnParryBegin?.Invoke(CharacterParam.Transform.position);
            hitObject.Hit(gameObject);
        }
        else if (hitObject.ParryMode == AttackObjectBase.ParryKinds.KnockBack)
        {
            OnParryBegin?.Invoke(CharacterParam.Transform.position);
            StartCoroutine(KnockBack(NormalKnockBack, hitObject.GetHitedObjectKnockBackDirection(gameObject)));
            hitObject.Parryed(_attackBase);




            DisableParryFlag();
            OnParryEnd?.Invoke();
            _isParry = false;
        }
        else if (hitObject.ParryMode == AttackObjectBase.ParryKinds.DisAble)
        {
            StartCoroutine(KnockBack(NormalKnockBack, hitObject.GetHitedObjectKnockBackDirection(gameObject)));
            hitObject.Parryed(_attackBase);

            DisableParryFlag();
            OnParryEnd?.Invoke();
            _isParry = false;
        }

    }

    private void Move()
    {
        Vector3 moveDirection = GetMoveDirection(_inputValue.GetInputMoveValueNomalize());
        NormalMove.Move(moveDirection);
    }

    /// <summary>
    /// ノックバック処理
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private IEnumerator KnockBack(ICoroutineMove knockBackMove, Vector3 direction)
    {
        //ここでDash を止めておかないと , KnockBack後にDashが復活する
        _isDash = false;
        _isKnockBack = true;
        OnKnockBack?.Invoke();
        yield return knockBackMove.Move(direction);

        _isKnockBack = false;
    }




    /// <summary>
    /// 入力値をカメラの向きを前にするように回転したベクトルを出力
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMoveDirection(Vector2 moveInput)
    {
        if (_gameCamera.IsLockOn)
        {
            return LockonDirection(moveInput);
        }
        return FreeLocKDirection(moveInput);

    }

    /// <summary>
    /// フリールック状態の方向
    /// </summary>
    /// <returns></returns>
    private Vector2 FreeLocKDirection(Vector2 inputVector)
    {
        Vector2 moveFront = new Vector2(_gameCamera.Camera.forward.x, _gameCamera.Camera.forward.z);
        //ワールドの座標(2次元)の前とカメラの前方方向の角度差分
        float offsetAngle = Vector2.SignedAngle(Vector2.up, moveFront) * (Mathf.PI / 180);
        //2次元の回転
        float sin = Mathf.Sin(offsetAngle);
        float cos = Mathf.Cos(offsetAngle);
        return new Vector2(inputVector.x * cos - inputVector.y * sin, inputVector.x * sin + inputVector.y * cos);
    }

    /// <summary>
    /// ロックオン状態の方向
    /// </summary>
    /// <returns></returns>
    private Vector2 LockonDirection(Vector2 inputVector)
    {
        if (_gameCamera.TargetObject == null) return Vector3.zero;
        Vector3 targetDirection = _gameCamera.TargetObject.transform.position - _gameCamera.FollowAtTransform.position;
        Vector2 moveFront = new Vector2(targetDirection.x, targetDirection.z);
        //ワールドの座標(2次元)の前とカメラの前方方向の角度差分
        float offsetAngle = Vector2.SignedAngle(Vector2.up, moveFront) * (Mathf.PI / 180);
        //2次元の回転
        float sin = Mathf.Sin(offsetAngle);
        float cos = Mathf.Cos(offsetAngle);
        return new Vector2(inputVector.x * cos - inputVector.y * sin, inputVector.x * sin + inputVector.y * cos);
    }


}
