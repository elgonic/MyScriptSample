using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AnalogJumpParams
{
    public float JumpHight;
    public float JumpHightTime;
    public float BufferJumpTime;
    public float CoyoteOffset;
}

/// <summary>
/// 入力に比例して跳ぶジャンプ
/// </summary>
public class AnalogJump : IJump
{
    private AnalogJumpParams _params;
    private Rigidbody _rb;
    private LayerMask _groundLayer;
    public UnityEvent OnJump { get; set; }

    public AnalogJump(AnalogJumpParams jumpParams, Rigidbody rb, LayerMask groundLayer)
    {
        _params = jumpParams;
        _rb = rb;
        _groundLayer = groundLayer;
    }

    public void Jump()
    {
        OnJump.Invoke();
    }

}
