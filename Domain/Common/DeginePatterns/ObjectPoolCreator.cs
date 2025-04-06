using UnityEngine;
using UnityEngine.Pool;



/// <summary>
/// オブジェクトプール生成ラッパー
/// </summary>
public class ObjectPoolCreator : MonoBehaviour
{
    /// <summary>
    /// オブジェクトプール作成
    /// </summary>
    /// <param name="source"></param>
    /// <param name="baseTr">インスタンスやリリース時に設定される親オブジェクト</param>
    /// <returns></returns>
    public static ObjectPool<PooledSouce> CreatePool(PooledSouce source, Transform baseTr = null, int MaxPoolSize = 20)
    {
        ObjectPool<PooledSouce> inputPool = null;
        ObjectPool<PooledSouce> pool = new ObjectPool<PooledSouce>(
                    createFunc: () => InstantiateObject(source, inputPool, baseTr),
                    actionOnGet: target => Get(target),
                    actionOnRelease: target => Release(target, baseTr),
                    actionOnDestroy: target => Destroy(target.gameObject),
                    defaultCapacity: 5,
                    maxSize: MaxPoolSize
                  );
        inputPool = pool;
        return pool;
    }

    private static PooledSouce InstantiateObject(PooledSouce source, ObjectPool<PooledSouce> pool, Transform baseTr = null)
    {
        PooledSouce createObj = null;
        if (baseTr) createObj = Instantiate(source, baseTr);
        else createObj = Instantiate(source);
        createObj.Pool = pool;
        return createObj;
    }

    private static void Release(PooledSouce target, Transform baseTr = null)
    {
        if (!target) return;
        target.gameObject.SetActive(false);
        if (baseTr) target.transform.SetParent(baseTr);
    }


    private static void Get(PooledSouce target)
    {
        if (target != null) target.gameObject.SetActive(true);
    }
}
