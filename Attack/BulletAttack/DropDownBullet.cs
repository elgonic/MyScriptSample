using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 上から降ってくる弾
/// </summary>
public class DropDownBullet : AttackBase
{

    [SerializeField] private float _time = 5f;
    [Header("着弾地点のばらけ半径")]
    [SerializeField] private float _dropDownHitAreaRadius = 50f;
    [Header("発射地点のばらけ半径")]
    [SerializeField] private float _dropDownStartAreaRadius = 50f;
    [Header("弾生成後の猶予時間")]
    [SerializeField] private float _dropDownInertiaTime = 1f;
    [Header("弾生成の高さ")]
    [SerializeField] private float _dropStartHight = 50f;
    [Header("一度の攻撃時に発射する弾数")]
    [SerializeField] private int _rapidFireCount = 1;
    [Header("攻撃間隔")]
    [SerializeField] private float _fireInterval = 0.3f;
    [Header("着弾地点の中心をターゲットに設定する")]
    [SerializeField] private bool _isHitPositionCenterToTarget = false;
    [Header("発射地点の中心をターゲットに設定する")]
    [SerializeField] private bool _isStartPositionCenterToTarget = false;

    [Header("ロックオンエフェクト関係")]
    [SerializeField] private PooledEffectSource _lockOnEffect;
    [SerializeField] private float _lockOnEffectHightOffset = 0.01f;


    [SerializeField] private BulletBase _bulletPrefab;

    private List<BulletBase> _bulletList = new List<BulletBase>();

    private Transform _target = null;

    public float AttackTime => _time;



    private float? @groundHight;


    /// <summary>
    /// 地面の高さ
    /// </summary>
    private float _groundHight
    {
        get
        {
            if (!@groundHight.HasValue)
            {
                RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, 1000);
                foreach (var hit in hits)
                {
                    if (hit.transform.CompareTag(StaticCommonParams.GROUND_TAG))
                    {
                        @groundHight = hit.point.y;
                    }
                }
            }
            if (@groundHight.HasValue)
            {
                return @groundHight.Value;
            }
            else
            {
                Debugger.Log($"{GetType().Name} : Groundが検知できません");
                return 0;
            }
        }
    }


    private Coroutine _attackCoroutine;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, _groundHight, transform.position.z), _dropDownHitAreaRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, _groundHight + _dropStartHight, transform.position.z), _dropDownStartAreaRadius);


    }


    private void Update()
    {
        if (_target && _isStartPositionCenterToTarget) transform.position = _target.position;
    }
    public override Coroutine Attack(CharacterParam attacker, CharacterParam target = null)
    {


        if (!gameObject.activeSelf) return null;

        Finish();
        if (target) _target = target.transform;

        Debugger.Log("Drop Fire");
        _attackCoroutine = StartCoroutine(AttackCoroutine(attacker, target));
        return _attackCoroutine;
    }

    private IEnumerator AttackCoroutine(CharacterParam attacker, CharacterParam target = null)
    {
        float attackStartTime = Time.time;
        while (Time.time - attackStartTime < _time)
        {
            Debugger.Log($"{Time.time - attackStartTime}");
            for (int i = 0; i < _rapidFireCount; i++)
            {
                StartCoroutine(FireCoroutine(attacker, target));
                yield return null;
            }
            yield return StaticCommonParams.Yielders.Get(_fireInterval);
        }

    }

    private IEnumerator FireCoroutine(CharacterParam attacker, CharacterParam target = null)
    {
        BulletBase bullet = CreateBullet(_bulletPrefab);
        Vector3 hitPosition = GetHitPosition();
        Vector3 startPosition = GetStartPosition(hitPosition);
        bullet.transform.position = startPosition;

        if (_lockOnEffect)
        {
            PooledEffectSource instantiateLockOnEffect = MainGameSystem.Instance.EffectSpawner.UniqePooledEffectPlay(_lockOnEffect, hitPosition + new Vector3(0, _lockOnEffectHightOffset, 0), Quaternion.identity);
            bullet.OnHit.AddListener(() => { if (instantiateLockOnEffect && instantiateLockOnEffect.gameObject.activeSelf) instantiateLockOnEffect.Release(); });
            bullet.OnLifeTime.AddListener(() => { if (instantiateLockOnEffect && instantiateLockOnEffect.gameObject.activeSelf) instantiateLockOnEffect.Release(); });
        }
        else
        {
            Debugger.Log($"{GetType().Name} : {_lockOnEffect} is NUll!");
        }

        yield return StaticCommonParams.Yielders.Get(_dropDownInertiaTime);
        bullet.Fire((hitPosition - startPosition), new AttackData(transform.position, attacker, IsKnockBackDirectionToAttacker), target?.Transform);
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


    /// <summary>
    /// 着弾地点の取得処理
    /// </summary>
    /// <returns></returns>
    private Vector3 GetHitPosition()
    {
        Vector2 xzPosition;

        if (_target && _isHitPositionCenterToTarget)
        {
            xzPosition = Random.insideUnitCircle * _dropDownHitAreaRadius + new Vector2(_target.position.x, _target.position.z);
        }
        else
        {
            xzPosition = Random.insideUnitCircle * _dropDownHitAreaRadius + new Vector2(transform.position.x, transform.position.z);
        }
        return new Vector3(xzPosition.x, _groundHight, xzPosition.y);
    }

    /// <summary>
    /// 発射地点の取得処理
    /// </summary>
    /// <param name="hitPosition"></param>
    /// <returns></returns>
    private Vector3 GetStartPosition(Vector3 hitPosition)
    {
        if (_dropDownHitAreaRadius <= 0)
        {
            Vector2 xzPosition = Random.insideUnitCircle * _dropDownStartAreaRadius + new Vector2(transform.position.x, transform.position.z);
            return new Vector3(xzPosition.x, _groundHight + _dropStartHight, xzPosition.y);
        }
        if (_dropDownStartAreaRadius <= 0)
        {
            return new Vector3(transform.position.x, _groundHight + _dropStartHight, transform.position.z);
        }

        float magnification = _dropDownStartAreaRadius / _dropDownHitAreaRadius;
        Vector3 offsetCenter = (hitPosition - transform.position);
        return new Vector3(transform.position.x, hitPosition.y + _dropStartHight, transform.position.z) + new Vector3(offsetCenter.x * magnification, 0, offsetCenter.z * magnification);
    }


    private BulletBase CreateBullet(BulletBase bulletPreafab)
    {
        return Instantiate(bulletPreafab);
    }

}
