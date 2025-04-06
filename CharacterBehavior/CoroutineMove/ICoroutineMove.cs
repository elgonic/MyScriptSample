using UnityEngine;

/// <summary>
/// コルーチンを使用した移動処理のインターフェース
/// </summary>
public interface ICoroutineMove
{
    public Coroutine Move(Vector3 direction);
    public void Stop();
}
