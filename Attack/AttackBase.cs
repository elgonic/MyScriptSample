using UnityEngine;

/// <summary>
/// 攻撃の基本クラス
/// </summary>
public abstract class AttackBase : MonoBehaviour
{
    /// <summary>
    /// 弾の方向 or 攻撃者に対してノックバックする
    /// </summary>
    public bool IsKnockBackDirectionToAttacker = false;

    public bool Attacking { get; protected set; }
    public abstract Coroutine Attack(CharacterParam attacker, CharacterParam target = null);

    /// <summary>
    /// 終了処理
    /// </summary>
    /// <returns>他のクラスから強制終了時にも使う</returns>
    public abstract Coroutine Finish();

}
