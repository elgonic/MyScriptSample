using UnityEngine;


/// <summary>
/// キャラクター用のコライダーデリゲート
/// </summary>
/// <remarks>
/// 地面や障害物の当たり判定と攻撃の当たり判定を別に出来る
/// </remarks>
public class CharacterHitCollider : ColliderDelegate
{
    [SerializeField] private CharacterParam _characterParam;
    public CharacterParam CharacterParam
    {
        get { return _characterParam; }
        set { _characterParam = value; }
    }

    protected override void Reset()
    {
        base.Reset();
        Transform target = transform;

        CharacterParam = target.GetComponent<CharacterParam>();
        while (target != transform.root && CharacterParam == null)
        {
            target = GetParent(target);
            CharacterParam = target.GetComponent<CharacterParam>();
        }

        if (CharacterParam == null)
        {
            Debugger.LogError($"{gameObject.name} : 親オブジェクトに{nameof(CharacterParam)}が存在しません");
        }

        Transform GetParent(Transform target)
        {
            return target.parent;
        }
    }
}
