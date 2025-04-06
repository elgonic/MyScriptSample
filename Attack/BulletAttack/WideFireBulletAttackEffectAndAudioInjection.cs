using UnityEngine;

/// <summary>
/// WideFireBulletにエフェクトと音を注入
/// </summary>
public class WideFireBulletAttackEffectAndAudioInjection : MonoBehaviour
{
    [SerializeField] private WideFireBullet _attack;

    [Header("SE")]
    [Header("発射ごとのSE")]
    [SerializeField] private AudioSource _fireSE;




    private void Reset()
    {
        _attack = GetComponent<WideFireBullet>();
        if (_attack == null) Debugger.LogError($" {GetType().Name} :{gameObject.name}: に {GetType().Name}:が存在しません");
    }

    private void Awake()
    {
        _attack.OnBulletFire.AddListener(OnFire);
    }



    private void OnFire()
    {
        AudioManager.Instance.UniqePooledSEPlay(_fireSE, _attack.transform.position);
    }
}
