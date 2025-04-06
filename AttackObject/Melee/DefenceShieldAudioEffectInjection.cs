using UnityEngine;


/// <summary>
/// 防御用シールド
/// </summary>
public class DefenseShieldAudioEffectInjection : MonoBehaviour
{

    [SerializeField] private DefenseShield _defenseShield;

    [Header("SE")]
    [SerializeField] private AudioSource _hitSE;


    private void Reset()
    {
        _defenseShield = GetComponent<DefenseShield>();
    }


    private void Awake()
    {
        _defenseShield.OnHit.AddListener(Hit);
        _defenseShield.OnParryed.AddListener(Parry);
    }

    private void Hit()
    {
        AudioManager.Instance.UniqePooledSEPlay(_hitSE, _defenseShield.gameObject.transform.position);
    }

    private void Parry(AttackObjectBase attackObject)
    {
        Hit();
    }


}
