using UnityEngine.Events;



/// <summary>
/// ジャンプ挙動のインターフェース
/// </summary>
public interface IJump
{
    public UnityEvent OnJump { get; set; }
    public void Jump();
}
