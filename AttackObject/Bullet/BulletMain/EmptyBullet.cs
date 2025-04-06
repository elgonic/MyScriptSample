using UnityEngine;


/// <summary>
/// Bulletスクリプトと当たり判定オブジェクトが別の弾用の空Bullet
/// </summary>
/// <remarks>
/// レーザーなどに使用
/// </remarks>
public class EmptyBullet : BulletBase
{
    public BulletBase ParentBullet;

    /// <summary>
    /// AttackDataを設定するだけ
    /// </summary>
    /// <param name="fireDirection"></param>
    /// <param name="attackData"></param>
    /// <param name="target"></param>
    public override void Fire(Vector3 fireDirection, AttackData attackData, Transform target = null)
    {
        AttackData = attackData;
    }


    public override void Hit(GameObject hitObject)
    {
        ParentBullet?.Hit(hitObject);
    }

    public override void ResetTransform()
    {
        ParentBullet?.ResetTransform();
    }

    public override void Parryed(AttackObjectBase attackerData)
    {
        ParentBullet?.Parryed(attackerData);
    }


}
