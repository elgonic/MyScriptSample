using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 縄跳び用 , 周囲レーザー
/// </summary>
public class CircleLaserAttack : AttackBase
{
    [Header("攻撃時に使用")]
    [SerializeField] private float _anticipationTime = 0.5f;
    [SerializeField] private float _attackTime = 3f;
    [SerializeField] private float _rotateSpeed = 0f;
    [SerializeField] private bool _useSin = false;
    [SerializeField] private float _swingWidth = 1f;

    [Header("配置時に使用")]
    [SerializeField] private float _radius = 1.0f;
    [SerializeField] private float _bulletAngularSpacing = 5.0f;


    [SerializeField] private Laser _bulletPrefab;


    [SerializeField, ReadOnly] private List<Laser> _bulletList = new();


    /// <summary>
    /// 攻撃するオブジェクトのポジション
    /// このポジション弾はこのポジションと弾の方向に対して発射される。
    /// 何も設定しない場合は平面の中心から放射状に発射される；
    /// </summary>
    [Header("弾の発射の中心座標")]
    [SerializeField] private Transform _attackPositionTransform = null;

    private Quaternion _resetRotation;
    public Laser Bullet { set { _bulletPrefab = value; } }
    public Transform AttackPositionTransform { set { _attackPositionTransform = value; } }
    public float Radius { set { _radius = value; } }
    public float BulletAngleSpacing { set { _bulletAngularSpacing = value; } }

    private bool _isAttacking = false;
    private Coroutine _attackCoroutine;
    private float _timer = 0f;

    /// <summary>
    /// 発射のたびに代わるアングル
    /// </summary>
    private static float _attackAngle = 0;



    private void OnValidate()
    {
        SetRadius(_radius);

    }

    private void Awake()
    {
        foreach (var bullet in _bulletList)
        {
            bullet.gameObject.SetActive(false);
        }

    }

    private void Start()
    {
        _resetRotation = transform.rotation;

        //親の回転受けたくないのでrootに配置
        gameObject.transform.SetParent(null);
    }
    private void Update()
    {
        if (!_isAttacking) return;


        if (_useSin)
        {
            _timer += Time.deltaTime;
            if (_rotateSpeed != 0) transform.rotation *= Quaternion.AngleAxis(_swingWidth * MyMathf.Sin(_timer), Vector3.up);
        }
        else
        {
            if (_rotateSpeed != 0) transform.rotation *= Quaternion.AngleAxis(_rotateSpeed * Time.deltaTime, Vector3.up);
        }

    }
    public override Coroutine Attack(CharacterParam attacker = null, CharacterParam target = null)
    {
        if (attacker)
        {
            _attackPositionTransform = attacker.transform;
        }
        else
        {
            _attackPositionTransform = attacker.transform;
        }

        //ポジション設定
        transform.position = new Vector3(_attackPositionTransform.position.x, transform.position.y, _attackPositionTransform.position.z);

        if (_bulletPrefab == null)
        {

            return null;
        }

        if (_bulletList.Count == 0) Debugger.Log($"{GetType().Name} : BulletPrefabが設定されていません");


        Finish();

        _attackCoroutine = StartCoroutine(AttackCoroutine(attacker, target));



        return _attackCoroutine;
    }

    private IEnumerator AttackCoroutine(CharacterParam attacker = null, CharacterParam target = null)
    {
        transform.rotation = Quaternion.AngleAxis(_attackAngle, Vector3.up);

        foreach (var bullet in _bulletList)
        {
            bullet.IsEnabledLifeTime = true;


            bullet.LifeTime = _attackTime;
            bullet.AnticipationTime = _anticipationTime;
            bullet.Fire((bullet.transform.position - _attackPositionTransform.position), new AttackData(transform.position, attacker, IsKnockBackDirectionToAttacker), null);
        }

        yield return StaticCommonParams.Yielders.Get(_bulletPrefab.AnticipationAnimationTime);


        _isAttacking = true;



        yield return StaticCommonParams.Yielders.Get(_attackTime);

        _isAttacking = false;

        yield return StaticCommonParams.Yielders.Get(_bulletPrefab.AnticipationAnimationTime);

    }

    /// <summary>
    /// 終了処理
    /// </summary>
    /// <returns></returns>
    public override Coroutine Finish()
    {
        Coroutine _forFinishWaite = null;

        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
        }

        foreach (var bullet in _bulletList)
        {
            if (_forFinishWaite == null) _forFinishWaite = bullet.Finish();
            else bullet.Finish();
        }
        return _forFinishWaite;
    }


    public void ArrangementBullet()
    {
        foreach (var bullet in _bulletList)
        {
            if (!bullet) continue;
#if UNITY_EDITOR
            DestroyImmediate(bullet.gameObject);
#else

            Destroy(bullet);
#endif

        }
        _bulletList.Clear();
        _bulletList = ArrangementBullets<Laser>.CircularArrangement(_radius, _bulletAngularSpacing, _bulletPrefab, transform, _bulletList);
    }

    public void SetRadius(float radius)
    {
        if (_bulletList == null) return;
        foreach (BulletBase b in _bulletList)
        {
            Vector3 direction = (b.gameObject.transform.position - gameObject.transform.position).normalized;
            b.gameObject.transform.position = gameObject.transform.position + direction * radius;
        }
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(CircleLaserAttack))]
    [CanEditMultipleObjects]
    public class ThisEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var targetComp = (CircleLaserAttack)target;
            if (GUILayout.Button("円状に弾配置"))
            {
                targetComp.ArrangementBullet();
                //これが居ないとPlay時に値が保存されない
                EditorUtility.SetDirty(targetComp);
            }

        }
    }
#endif
}
