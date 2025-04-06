using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 体力コンポーネント
/// </summary>
public class HpComp : MonoBehaviour
{
    [SerializeField] private int _initialHp = 5;
    [SerializeField, ReadOnly] private int _hp;

    [Header("Test")]
    [SerializeField] private bool _isInvincibleForTest = false;
    [SerializeField] private bool _damage = false;
    [SerializeField] private bool _cure = false;




    /// <summary>
    /// 無敵のON OFF
    /// </summary>
    public bool IsInvincible { get; set; } = false;
    public int InitialHp => _initialHp;
    public int Hp => _hp;

    /// イベント
    public UnityEvent OnInvincible;
    public UnityEvent OnChange;
    public UnityEvent OnDamage;
    public UnityEvent OnCure;
    public UnityEvent OnDeath;


    private void OnValidate()
    {
        if (_damage)
        {
            _damage = false;
            Damage(1);
        }

        if (_cure)
        {
            _cure = false;
            Cure(1);
        }
    }
    private void Awake()
    {
        _hp = _initialHp;
    }

    public int Damage(int damageAmount)
    {
        if (IsInvincible || _isInvincibleForTest)
        {
            OnInvincible?.Invoke();
            return _hp;
        }


        _hp -= damageAmount;

        if (_hp < 0)
        {
            _hp = 0;
        }

        OnChange?.Invoke();

        if (_hp == 0)
        {
            OnDeath?.Invoke();
        }
        else
        {
            OnDamage?.Invoke();
        }
        return _hp;
    }

    public int Cure(int cureAmount)
    {
        _hp += cureAmount;
        OnCure?.Invoke();
        OnChange?.Invoke();
        return _hp;
    }

    public void Reset()
    {
        _hp = _initialHp;
        OnChange?.Invoke();
    }
}
