using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵クラス
/// </summary>
[RequireComponent(typeof(CharacterParam))]
public class Enemy : MonoBehaviour
{

    [SerializeField] private GameObject _enemyObject;
    [SerializeField] private List<GameObject> _dethedWhenDisableObjectList;

    /// <summary>
    /// 当たり判定用のコライダ
    /// </summary>
    private ColliderDelegate _hitColliderDelegate;


    private CharacterParam _param;
    public CharacterParam CharacterParam
    {
        get
        {
            if (_param == null) _param = GetComponent<CharacterParam>();
            return _param;
        }
    }

    private void Awake()
    {
        CharacterParam.Hp.OnDeath.AddListener(Death);

        _hitColliderDelegate = GetComponentInChildren<ColliderDelegate>();

        if (_hitColliderDelegate)
        {
            _hitColliderDelegate.OnTriggerEnterAction.AddListener(HitColliderHitAction);
        }
        else Debugger.LogWarning($"{gameObject.name}の子要素に当たり判定用{typeof(ColliderDelegate).Name}がアタッチされたコライダーがないです");

    }


    private void Death()
    {
        _hitColliderDelegate.gameObject.SetActive(false);
        _enemyObject.SetActive(false);
        foreach (GameObject obj in _dethedWhenDisableObjectList)
        {
            obj.SetActive(false);
        }
    }

    private void HitColliderHitAction(Collider other, Transform hitter = null)
    {
        HitObject hitObject = new HitObject(CharacterParam, other.gameObject);
        if (hitObject.Attack && hitObject.IsDamegeObject)
        {
            CharacterParam.Damage(1, hitObject.Attack);
            hitObject?.Attack.Hit(gameObject);
        }
    }
}
