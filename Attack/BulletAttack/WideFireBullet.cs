using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 横方向に並べた弾を発射する
/// </summary>
public class WideFireBullet : AttackBase
{

    /// <summary>
    /// 発射時の基本方向
    /// </summary>
    private enum LookAtType
    {
        None,
        AttackerFront,
        AttackerBack,
        Target
    }

    [Header("並べる弾の数")]
    [SerializeField] private int _wideFireCount = 1;
    [Header("並べたときの角度の間隔")]
    [SerializeField] private float _distanceAngle = 10f;
    [Header("連続発射回数")]
    [SerializeField] private int _rapidFireCount = 1;
    [Header("連続発射間隔")]
    [SerializeField] private float _fireInterval = 0.3f;
    [Header("ロックンタイプ")]
    [SerializeField] private LookAtType _lookAtType = LookAtType.None;
    [Header("発射する玉のプレハブ")]
    [SerializeField] private BulletBase _bulletPrefab;

    private List<BulletBase> _bulletList = new List<BulletBase>();


    private Coroutine _attackCoroutine;



    //イベント
    public UnityEvent OnStartFire;
    public UnityEvent OnBulletFire;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        bool isWideFireCountOdd = (_wideFireCount % 2 != 0);

        float startAngle;

        if (isWideFireCountOdd)
        {
            startAngle = -Mathf.Floor(_wideFireCount / 2) * _distanceAngle;
        }
        else
        {
            startAngle = -((((float)_wideFireCount - 1) / 2)) * _distanceAngle;
        }
        for (int i = 0; i < _wideFireCount; i++)
        {
            Vector3 fireDirection = Quaternion.AngleAxis(startAngle + (_distanceAngle * i), transform.up) * GetFireFrontDirection(_lookAtType);
            Gizmos.DrawLine(transform.position, (fireDirection * 10 + transform.position));
        }
    }


    private void Start()
    {

    }
    public override Coroutine Attack(CharacterParam attacker, CharacterParam target = null)
    {
        if (!gameObject.activeSelf) return null;
        Finish();
        _attackCoroutine = StartCoroutine(AttackCoroutine(attacker, target));
        return _attackCoroutine;
    }

    private IEnumerator AttackCoroutine(CharacterParam attacker, CharacterParam target = null)
    {


        bool isWideFireCountOdd = (_wideFireCount % 2 != 0);

        float startAngle;

        if (isWideFireCountOdd)
        {
            startAngle = -Mathf.Floor(_wideFireCount / 2) * _distanceAngle;
        }
        else
        {
            startAngle = -((_wideFireCount / 2) - 1) * _distanceAngle + _distanceAngle / 2;
        }


        for (int j = 0; j < _rapidFireCount; j++)
        {
            Vector3 fireBaseDirection = GetFireFrontDirection(_lookAtType, attacker, target);

            OnBulletFire?.Invoke();

            for (int i = 0; i < _wideFireCount; i++)
            {
                BulletBase bullet = CreateBullet(_bulletPrefab);
                bullet.transform.position = transform.position;
                Vector3 fireDirection = Quaternion.AngleAxis(startAngle + (_distanceAngle * i), Vector3.up) * fireBaseDirection;


                bullet.Fire(fireDirection, new AttackData(transform.position, attacker, IsKnockBackDirectionToAttacker), target?.Transform);

                _bulletList.Add(bullet);
            }
            yield return StaticCommonParams.Yielders.Get(_fireInterval);

        }

    }

    private Vector3 GetFireFrontDirection(LookAtType lookAtType, CharacterParam attacker = null, CharacterParam target = null)
    {
        switch (lookAtType)
        {
            //オブジェクトの前方方向
            case LookAtType.None:
                return new Vector3(transform.forward.x, 0, transform.forward.z);
            //攻撃者の前方方向
            //攻撃者未設定の場合は None
            case LookAtType.AttackerFront:
                if (!attacker) return GetFireFrontDirection(LookAtType.None);
                return new Vector3(attacker.Transform.forward.x, 0, attacker.Transform.forward.z);
            //攻撃者の前方方向
            //攻撃者未設定の場合は None
            case LookAtType.AttackerBack:
                if (!attacker) return GetFireFrontDirection(LookAtType.None);
                return -new Vector3(attacker.Transform.forward.x, 0, attacker.Transform.forward.z);
            //ターゲットの方向
            //ターゲット未設定の場合はNone
            case LookAtType.Target:
                if (!target) return GetFireFrontDirection(LookAtType.None);
                Vector3 tmp = target.Transform.position - transform.position;
                return new Vector3(tmp.x, 0, tmp.z);
            default: return GetFireFrontDirection(LookAtType.None);
        }
    }

    public override Coroutine Finish()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
        }
        foreach (BulletBase bullet in _bulletList)
        {
            if (bullet) bullet.ResetTransform();
        }
        _bulletList.Clear();
        return null;
    }


    private BulletBase CreateBullet(BulletBase bulletPreafab)
    {
        return Instantiate(bulletPreafab);
    }

}
