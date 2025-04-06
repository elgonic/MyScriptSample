using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
///<summary>
///弾の基底クラス
///</summary>
/// <remarks>
/// 死ぬ時の処理と Objectpoolの処理 , UpdateMnagerへの登録とかを詳細に記載する。
/// 後のことは子クラスにすべて任せる事！
/// </remarks>
public abstract class BulletBase : AttackObjectBase
{

    public UnityEvent OnFire;
    public UnityEvent OnLifeTime;

    /// <summary>
    /// 発射処理   
    /// </summary>
    /// <param name="fireDirection"></param>
    /// <param name="attacker">
    ///ここに値を設定すると , ヒット時のノックバック方向が攻撃した奴との反対方向になる
    /// </param>
    /// <param name="target"></param>
    public abstract void Fire(Vector3 fireDirection, AttackData attackData, Transform target = null);
    public abstract void ResetTransform();



    /// <summary>
    /// 弾に当たったオブジェクトのノックバック方向
    /// </summary>
    /// <param name="hitObject">弾に当たったオブジェクト</param>
    /// <returns></returns>
    public override Vector3 GetHitedObjectKnockBackDirection(GameObject hitObject)
    {
        if (IsKbockBackDirectionToAttacker() && hitObject) return GetHitedObjectKnockBackDirectionToAttacker(hitObject.transform.position);

        CharacterParam characterParam = hitObject.GetComponent<CharacterParam>();
        if (characterParam && characterParam.MoveDirection != Vector3.zero) return -characterParam.MoveDirection;
        if (MoveDirection != Vector3.zero) return MoveDirection;
        return transform.forward;

    }

    /// <summary>
    /// 生存時間チェック
    /// </summary>
    /// <param name="lifeTime"></param>
    /// <param name="fireTime"></param>
    /// <param name="isEnable"></param>
    /// <returns></returns>
    protected bool LifeTimeCheck(float lifeTime, float fireTime, bool isEnable = false)
    {
        if (isEnable && Time.time - fireTime > lifeTime)
        {
            return true;
        }
        return false;

    }

    /// <summary>
    /// 指定したPlaneの外側に出ているか
    /// </summary>
    /// <param name="planes"></param>
    /// <param name="bulletPosition"></param>
    /// <returns>地面や壁のチャックに使うといいかも</returns>
    protected bool CheckPlaneOutSide(List<Plane> planes, Vector3 bulletPosition, bool isEnable = false)
    {
        if (!isEnable) return false;
        if (planes == null) return false;
        foreach (Plane plane in planes)
        {
            if (plane.GetDistanceToPoint(bulletPosition) < 0) return true;
        }
        return false;
    }

    /// <summary>
    /// 発射テスト
    /// </summary>
    /// <param name="fireDirectin"></param>
    /// <param name="target"></param>
    public void FireTest(Vector3 fireDirectin, Transform target = null)
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying) return;
#endif
        Fire(fireDirectin, new AttackData(Vector3.zero, null, false), target);
    }


}
