using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 攻撃に使用されるオブジェクトの基底クラス
/// </summary>
[RequireComponent(typeof(MoveDirection))]
public abstract class AttackObjectBase : MonoBehaviour
{
    public enum ParryKinds
    {
        DisAble,
        Bounce,
        PassThrough,
        KnockBack
    }
    private MoveDirection _moveDirection;

    /// <summary>
    /// 現在の移動方向
    /// </summary>
    public Vector3 MoveDirection
    {
        get
        {
            if (!_moveDirection)
            {
                _moveDirection = GetComponent<MoveDirection>();
            }
            return _moveDirection.Direction;
        }
    }

    /// <summary>
    /// パリッ時の挙動
    /// </summary>
    public ParryKinds ParryMode = ParryKinds.DisAble;


    public virtual AttackData AttackData { get; protected set; }

    public abstract void Hit(GameObject hitObject);
    public abstract void Parryed(AttackObjectBase attackerData);

    /// <summary>
    /// 当たった対象物のノックバック方向
    /// </summary>
    /// <returns></returns>
    public abstract Vector3 GetHitedObjectKnockBackDirection(GameObject hitObjefct);



    /// <summary>
    /// 当たったノックバック方向を攻撃者とするとき
    /// </summary>
    /// <remarks>
    /// 攻撃者と攻撃が離れているときに使うことが想定
    /// </remarks>
    /// <param name="hitedObjectPosition">
    /// 当たったオブジェクトの位置
    /// </param>
    public Vector3 GetHitedObjectKnockBackDirectionToAttacker(Vector3 hitedObjectPosition)
    {
        if (AttackData.Attacker) return (hitedObjectPosition - AttackData.Attacker.transform.position).normalized;
        else
        {
            Debugger.Log($"{GetType().Name} : {gameObject.name} : Attackerが未設定攻撃なので KnockBak方向は0を返します");
            return Vector3.zero;
        }
    }

    /// <summary>
    /// 攻撃者に対するノックバックの判断
    /// </summary>
    /// <returns></returns>
    public bool IsKbockBackDirectionToAttacker()
    {
        if (AttackData != null && AttackData.IsKnockBackDirectionToAttacker) return true;
        return false;
    }



    public UnityEvent OnHit;
    public UnityEvent<AttackObjectBase> OnParryed;

}
