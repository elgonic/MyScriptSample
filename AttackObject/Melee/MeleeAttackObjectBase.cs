using UnityEngine;

/// <summary>
/// 近接武器基本クラス
/// </summary>
public abstract class MeleeAttackObjectBase : AttackObjectBase
{
    [Header("攻撃者の死亡時に非表示にする")]
    [SerializeField] private bool _isDisableWhenAttackerDeath = true;
    private AttackData _attackData;



    /// <summary>
    /// AttackData絶対設定する
    /// </summary>
    /// <remarks>
    /// 近接攻撃はBulletと違ってFireてきな起動コマンドがないのでAttackDataが不定になるので, 個々で絶対に設定する
    /// </remarks>
    public override AttackData AttackData
    {
        get
        {
            if (_attackData == null)
            {
                CharacterParam _attacker = null;
                Transform target = transform;
                _attacker = target.GetComponent<CharacterParam>();
                while (transform.root != target && _attacker == null)
                {
                    target = GetParent(target);
                    _attacker = target.GetComponent<CharacterParam>();
                }
                if (_attacker == null)
                {
                    Debugger.LogWarning($"{gameObject.name} : 親オブジェクトに{nameof(CharacterParam)}が存在しません");
                }

                _attackData = new AttackData(transform.position, _attacker);
            }
            return _attackData;

            Transform GetParent(Transform target)
            {
                return target.parent;
            }
        }
        protected set
        {
            _attackData = value;
        }
    }



    protected virtual void Awake()
    {
        if (_isDisableWhenAttackerDeath && AttackData?.Attacker) AttackData.Attacker.OnDeath.AddListener((attackObject) => { gameObject.SetActive(false); });
    }


}
