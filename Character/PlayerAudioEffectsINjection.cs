using UnityEngine;

/// <summary>
/// Playerに効果音とエフェクトを設定するクラス
/// </summary>
public class PlayerAudioEffectsInjection : MonoBehaviour
{
    [SerializeField] private AudioSource _dashSE;
    [SerializeField] private AudioSource _jumpSE;
    [SerializeField] private AudioSource _parrySE;
    [SerializeField] private AudioSource _damageSE;
    [SerializeField] private AudioSource _deathSE;

    [SerializeField] private PooledEffectSource _parryEffect;
    [SerializeField] private PooledEffectSource _dashEffect;
    [SerializeField] private PooledEffectSource _damageEffect;

    [SerializeField] private Transform _dashEffectPositionDiff;
    [SerializeField] private bool test = false;
    private Player _player;
    private float _dashEffectPositionLength;

    private void OnValidate()
    {
        if (test)
        {
            test = false;
            Parry(_player.transform.position);
        }
    }

    private void Start()
    {
        _player = GetComponent<Player>();
        if (!_player) return;
        _player.OnParryBegin.AddListener(Parry);
        _player.CharacterParam.Hp.OnDamage.AddListener(Damage);
        _player.CharacterParam.Hp.OnDeath.AddListener(Death);
        _player.OnDash.AddListener(Dash);
        _player.Jump.OnJump.AddListener(Jump);

        _dashEffectPositionLength = (_player.CharacterParam.Transform.position - _dashEffectPositionDiff.position).magnitude;
    }

    private void Update()
    {
    }


    private void Attack()
    {

    }

    private void Parry(Vector3 position)
    {
        if (!_parryEffect) EffectSpawner.Instance.CommonPooledEffectPlay(EffectSpawner.EffectType.Parry, position, Quaternion.identity);
        else EffectSpawner.Instance.UniqePooledEffectPlay(_parryEffect, position, Quaternion.identity);

        if (!_parrySE) AudioManager.Instance.CommonPooledSEPlay(AudioManager.AudioType.ParrySE, position);
        else AudioManager.Instance.UniqePooledSEPlay(_parrySE, position);
    }
    private void Damage()
    {
        if (!_damageEffect) EffectSpawner.Instance.CommonPooledEffectPlay(EffectSpawner.EffectType.HitAttack, _player.CharacterParam.Transform.position, Quaternion.identity, _player.CharacterParam.Transform);
        else EffectSpawner.Instance.UniqePooledEffectPlay(_damageEffect, _player.CharacterParam.Transform.position, Quaternion.identity, _player.CharacterParam.Transform);
        if (!_damageSE) AudioManager.Instance.CommonPooledSEPlay(AudioManager.AudioType.Damage, _player.CharacterParam.Transform.position);
        else AudioManager.Instance.UniqePooledSEPlay(_damageSE, _player.CharacterParam.Transform.position);
    }

    private void Death()
    {
        if (!_damageEffect) EffectSpawner.Instance.CommonPooledEffectPlay(EffectSpawner.EffectType.HitAttack, _player.transform.position, Quaternion.identity, _player.CharacterParam.Transform);
        else EffectSpawner.Instance.UniqePooledEffectPlay(_damageEffect, _player.CharacterParam.Transform.position, Quaternion.identity, _player.CharacterParam.Transform);
        if (!_deathSE) AudioManager.Instance.CommonPooledSEPlay(AudioManager.AudioType.Death, _player.CharacterParam.Transform.position);
        else AudioManager.Instance.UniqePooledSEPlay(_deathSE, _player.CharacterParam.Transform.position);
    }

    private void Dash(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
        }
        else
        {
            if (!_dashEffect) EffectSpawner.Instance.CommonPooledEffectPlay(EffectSpawner.EffectType.Dash, _player.PlayerGameObject.transform.position, Quaternion.LookRotation(direction), _player.PlayerGameObject.transform);
            else EffectSpawner.Instance.UniqePooledEffectPlay(_dashEffect, _player.PlayerGameObject.transform.position, Quaternion.LookRotation(direction), _player.PlayerGameObject.transform);

            if (!_dashSE) AudioManager.Instance.CommonPooledSEPlay(AudioManager.AudioType.DashSE, _player.PlayerGameObject.transform.position, _player.PlayerGameObject.transform);
            else AudioManager.Instance.UniqePooledSEPlay(_dashSE, _player.PlayerGameObject.transform.position);
        }
    }

    private void Jump()
    {
        if (!_jumpSE) AudioManager.Instance.CommonPooledSEPlay(AudioManager.AudioType.JumpSE, _player.PlayerGameObject.transform.position);
        else AudioManager.Instance.UniqePooledSEPlay(_jumpSE, _player.PlayerGameObject.transform.position);
    }

}
