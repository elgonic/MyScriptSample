using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// 通り過ぎる
/// </summary>
public class PassThrough : IParry
{
    private Params _params;

    private Coroutine _coroutine;

    public PassThrough(Params @params)
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
        float originTimeScale = Time.timeScale;
        Time.timeScale = Time.timeScale * _params.TimeScale;
        yield return new WaitForSecondsRealtime(_params.ChangeTimeScaleTime);
        Time.timeScale = originTimeScale;
    }

    public void Stop()
    {
        CoroutineHandler.StopStaticCorutine(_coroutine);
    }

    [Serializable]
    public class Params
    {
        public float TimeScale = 1;
        public float ChangeTimeScaleTime = 1;
    }

}
