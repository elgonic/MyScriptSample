using System.Collections;
using UnityEngine;


/// <summary>
/// 突進攻撃
/// </summary>
public class RushAttack : NearAttackBase
{
    [SerializeField] private float _rushSpeed = 120;
    [Header("武器")]
    [SerializeField] private CombatShield _rushWaponObject;
    /// <summary>
    /// 突進時に使用する、障害物検知用Rayの長さ
    /// </summary>
    [SerializeField] private RayLength _checkObstacleRay;
    [Header("アタックの時だけ武器を有効化する")]
    [SerializeField] private bool _isAttackObjectEnableWhenAttack = false;

    private bool _isParryed = false;
    private bool _isHitted = false;

    private IMove _move;
    private Coroutine _attackCoroutine;



    private void Awake()
    {
        _rushWaponObject.OnParryed.AddListener(Parryed);
        _rushWaponObject.OnHit.AddListener(Hitted);

        if (_isAttackObjectEnableWhenAttack) _rushWaponObject.gameObject.SetActive(false);
    }
    public override Coroutine Attack(CharacterParam attacker, CharacterParam target = null)
    {
        if (!gameObject.activeSelf) return null;
        return _attackCoroutine = StartCoroutine(AttackProcess(attacker, target));
    }

    private IEnumerator AttackProcess(CharacterParam attacker, CharacterParam target = null)
    {

        if (!_rushWaponObject.gameObject.activeSelf)
        {
            Debugger.Log($"{gameObject.name} : {nameof(Attack)} :シールドが破損, 攻撃不可");
            yield break;
        }

        if (!target)
        {
            Debugger.Log($"{gameObject.name} {typeof(RushAttack).Name} Target未設定です。{typeof(RushAttack).Name}発動しません");
            yield break;
        }



        if (_isAttackObjectEnableWhenAttack) _rushWaponObject.gameObject.SetActive(true);


        //Move関係
        RigidVelocityAccelerationMove.Params moveParams = new RigidVelocityAccelerationMove.Params();
        moveParams.MoveSpeed = _rushSpeed;
        moveParams.EnableAntiProcess = false;
        _move = new RigidVelocityAccelerationMove(moveParams, attacker.Rb);



        Vector3 distanceVector = (target.Transform.position - attacker.Transform.position);
        Vector3 direction = new Vector3(distanceVector.x, 0, distanceVector.z).normalized;
        Vector3 rushEndPoint = attacker.transform.position + distanceVector;



        Plane thresholdPlane = new Plane(distanceVector, rushEndPoint);

        _isParryed = false;
        _isHitted = false;
        while (!IsRushStop(direction, attacker, thresholdPlane) && !_isParryed && !_isHitted)
        {
            _move.Move(new Vector2(direction.x, direction.z));
            yield return StaticCommonParams.Yielders.FixedUpdate;
        }

        _move.Stop();

        if (_isAttackObjectEnableWhenAttack) _rushWaponObject.gameObject.SetActive(false);
    }

    /// <summary>
    /// ラッシュ中止条件
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="attacker"></param>
    /// <param name="thresholdPlane"></param>
    /// <returns></returns>
    private bool IsRushStop(Vector3 direction, CharacterParam attacker, Plane thresholdPlane)
    {
        //障害物に当たったら停止
        if (MoveCommonProcess.CheckObstacle(direction, _checkObstacleRay, _rushSpeed, new string[] { StaticCommonParams.WALL_TAG, StaticCommonParams.GROUND_TAG }))
        {
            return true;
        }


        if (thresholdPlane.GetDistanceToPoint(attacker.transform.position) > 0)
        {
            return true;
        }

        return false;
    }

    private void ResetTransform()
    {
        if (_isAttackObjectEnableWhenAttack) _rushWaponObject.gameObject.SetActive(false);
        else _rushWaponObject.gameObject.SetActive(true);
    }



    public override void Hitted()
    {
        _isHitted = true;
        OnHitted?.Invoke();

    }
    public override void Parryed(AttackObjectBase parryer)
    {
        _isParryed = true;
        OnParryed?.Invoke(parryer);
    }

    public override Coroutine Finish()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
        }
        ResetTransform();
        if (_move != null) _move.Stop();

        return null;
    }
}
