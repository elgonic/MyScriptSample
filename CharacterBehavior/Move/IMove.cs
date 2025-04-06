using UnityEngine;

/// <summary>
/// 移動挙動のインターフェース
/// </summary>
public interface IMove
{
    /// <summary>
    /// 移動
    /// </summary>
    /// <param name="moveDirection"></param>
    /// <returns>移動ベクトル(単位ベクトルではない)</returns>
    public Vector3 Move(Vector2 moveDirection);

    public void Stop();
}
