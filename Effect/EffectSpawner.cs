using DesignPatterns.Singleton;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


/// <summary>
/// エフェクトの生成
/// </summary>
public class EffectSpawner : SingletonMonobehaviour<EffectSpawner>
{
    public enum EffectType
    {
        HitAttack = 0,
        Dash = 1,
        Jump = 2,
        Parry = 4,
        EnemyAttackAnticipation = 3,
    }

    [SerializeField] private PooledEffectSource _hitEffect;
    private ObjectPool<PooledSouce> _hitEffectPool;

    [SerializeField] private PooledEffectSource _dashEffect;
    private ObjectPool<PooledSouce> _dashEffectPool;

    [SerializeField] private PooledEffectSource _parryEffect;
    private ObjectPool<PooledSouce> _parryEffectPool;

    [SerializeField] private PooledEffectSource _enemyAttackAnticipationEffect;
    private ObjectPool<PooledSouce> _enemyAttackAnticipationEffectPool;




    private Dictionary<EffectType, ObjectPool<PooledSouce>> @pooledEffectDic = new();

    //シーン変更時に初期化されるので, 初期化された場合は再セットアップする
    private Dictionary<EffectType, ObjectPool<PooledSouce>> _pooledEffectDic
    {
        get
        {
            if (@pooledEffectDic.Count == 0) SetPool();
            return @pooledEffectDic;
        }
    }
    private Dictionary<PooledSouce, ObjectPool<PooledSouce>> _dynamicPooledEffectDic { get; set; } = new();


    private void Awake()
    {
    }

    public void SetPool()
    {
        AddPoolToDic(EffectType.HitAttack, _hitEffect);
        AddPoolToDic(EffectType.Dash, _dashEffect);
        AddPoolToDic(EffectType.Parry, _parryEffect);
        AddPoolToDic(EffectType.EnemyAttackAnticipation, _enemyAttackAnticipationEffect);
    }
    private void AddPoolToDic(EffectType type, PooledEffectSource pooledSouce)
    {
        if (!pooledSouce)
        {
            Debugger.LogWarning($"{GetType().Name} : Not Set {type.ToString()} Effect ");
            return;
        }
        @pooledEffectDic.Add(type, CreatePool(pooledSouce, transform));
    }

    public void CommonPooledEffectPlay(EffectType type, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (_pooledEffectDic.TryGetValue(type, out var source) && source != null)
        {
            PlayPooledEffect(source.Get(), position, rotation, parent);
        }
        else
        {
            Debugger.LogWarning($"{GetType().Name}に{type.ToString()}が設定されていません");
        }
    }

    public void RemovePooled()
    {
        foreach (EffectType type in _pooledEffectDic.Keys)
        {
            _pooledEffectDic[type].Clear();
        }
        _pooledEffectDic.Clear();

        RemoveUniqePooled();
    }

    /// <summary>
    /// EffectTypeには無い各クラス固有の音をプールして鳴らす場合
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="position"></param>
    /// <param name="parent"></param>

    public PooledEffectSource UniqePooledEffectPlay(PooledEffectSource effectSource, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (effectSource == null) return null;
        if (!_dynamicPooledEffectDic.ContainsKey(effectSource))
        {
            _dynamicPooledEffectDic.Add(effectSource, CreatePool(effectSource, transform));
        }
        PooledSouce instantiatePooledSource = _dynamicPooledEffectDic[effectSource].Get();
        PlayPooledEffect(instantiatePooledSource, position, rotation, parent);
        return instantiatePooledSource as PooledEffectSource;

    }


    /// <summary>
    /// </summary>
    /// <remarks>
    /// クラス固有はシーン遷移とかで消したほうがいいので、ステージクリア時に呼ばれるようにする
    /// </remarks>
    public void RemoveUniqePooled()
    {
        foreach (PooledEffectSource effectSource in _dynamicPooledEffectDic.Keys)
        {
            _dynamicPooledEffectDic[effectSource].Clear();
        }
        _dynamicPooledEffectDic.Clear();
    }

    private void PlayPooledEffect(PooledSouce pooledSouce, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        PooledEffectSource pooledEffectSource = pooledSouce as PooledEffectSource;
        pooledEffectSource?.Play(position, rotation, parent);
    }


    /// <summary>
    /// 汎用Effectのオブジェクトプール作製処理
    /// </summary>
    /// <param name="pooledAudioSource">プールしたい音源</param>
    /// <returns>音源のオブジェクトプール</returns>
    private ObjectPool<PooledSouce> CreatePool(PooledEffectSource source, Transform baseTr)
    {
        return ObjectPoolCreator.CreatePool(source, baseTr);
    }

}
