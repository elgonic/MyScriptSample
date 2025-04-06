using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 弾き返す
/// </summary>
public class BounceBack : IParry
{
    private Params _params;

    private Coroutine _coroutine;
    public BounceBack(Params @params)
    {
        _params = @params;
    }

    public Coroutine Parry(AttackObjectBase hitAttack)
    {
        BulletBase bullet = hitAttack as BulletBase;
        if (bullet == null) return null;
        return _coroutine = CoroutineHandler.StartStaticCoroutine(ParryCoroutine(bullet));
    }

    private IEnumerator ParryCoroutine(BulletBase hitAttack)
    {
        Vector3 bouceDirection = hitAttack.transform.position - _params.CharacterParam.transform.position;
        yield return _params.KnockBack.Move(-bouceDirection);
    }

    public void Stop()
    {
        CoroutineHandler.StopStaticCorutine(_coroutine);
    }

    [Serializable]
    public class Params
    {
        public ICoroutineMove KnockBack;
        public CharacterParam CharacterParam;

        public Params(ICoroutineMove knockBack, CharacterParam characterParam)
        {
            KnockBack = knockBack;
            CharacterParam = characterParam;
        }
    }
}
