using UnityEngine;

/// <summary>
/// 場面遷移のインターフェース
/// </summary>
public interface ITransition
{
    public Coroutine Hide();
    public Coroutine Open();
}
