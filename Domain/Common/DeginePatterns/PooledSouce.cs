using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// ObjectPoolされる奴は継承する事！
/// </summary>
public abstract class PooledSouce : MonoBehaviour
{
    public ObjectPool<PooledSouce> Pool { get; set; }
    public abstract void Release();

}
