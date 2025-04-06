using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;




[Serializable]
public class UrushiJumpParams
{
    public float JumpHight = 1;
    public float JumpHightTime = 0.5f;
    [Header("下降の重力")]
    public float JumpFallGravity = 9.81f;
    [Header("慣性を表現するための上昇点での少しの停止時間")]
    public float InertiaFreezeTime = 0.5f;
    [Header("Buffer Jump 許容時間")]
    public float BufferJumpTime = 0.5f;

    [Header("慣性")]
    public bool IsActiveInertia = true;
    public float InertiaTime = 0.1f;
    public float InertiaHight = 0.5f;
}

/// <summary>
/// プレイヤー専用Jump
/// </summary>
/// <remarks>
/// 入力に比例するが、ジャンプ終了時にイイ感じに慣性を入れたジャンプ
/// </remarks>
public class UrushiJump : IJump
{
    private UrushiJumpParams _params;
    private CharacterParam _characterParam;
    private GravityComp _jumpTargetGravity;
    private Rigidbody _rb;
    private PlayerInput _playerInput;
    private Player _player;

    private float _jumpVelocity;
    private float _jumpDownGravity;

    private Coroutine _bufferCorutine = null;

    private bool _isJumpCorutine = false;

    private float _nonGravityJumpTime;

    public UnityEvent OnJump { get; set; } = new UnityEvent();

    public UrushiJump(UrushiJumpParams jumpParams, CharacterParam characterParam, PlayerInput playerInput)
    {
        _params = jumpParams;
        _characterParam = characterParam;
        _jumpTargetGravity = characterParam.Gravity;
        _rb = characterParam.Rb;
        _playerInput = playerInput;

    }

    /// <summary>
    /// ジャンプ開始
    /// </summary>
    public void Jump()
    {
        if (_characterParam.IsStandingGround) CoroutineHandler.StartStaticCoroutine(JumpCoprutine());
        else
        {
            if (_bufferCorutine != null)
            {
                CoroutineHandler.StopStaticCorutine(_bufferCorutine);
                _bufferCorutine = null;
            }
            _bufferCorutine = CoroutineHandler.StartStaticCoroutine(BufferJump());
        }

    }

    /// <summary>
    /// バッファージャンプ処理
    /// </summary>
    /// <returns>地面についていなくても猶予時間内に着地すればジャンプさせる</returns>
    private IEnumerator BufferJump()
    {
        float elapsedTime = 0;
        while (elapsedTime < _params.BufferJumpTime)
        {
            elapsedTime += Time.deltaTime;
            //if (!_characterParam.IsStandingGround)
            if (!_characterParam.IsStandingGround || _isJumpCorutine)
            {
                yield return null;
                continue;
            }
            CoroutineHandler.StartStaticCoroutine(JumpCoprutine());
            yield break;
        };
    }


    /// <summary>
    /// ジャンプ処理
    /// </summary>
    /// <returns>
    /// 制限高さとの完全一致はしない
    /// 精度を上げたいならfixiedUpdate の間隔を小さくすればいい
    /// </returns>
    private IEnumerator JumpCoprutine()
    {
        if (_isJumpCorutine) yield break;

        OnJump?.Invoke();

        _isJumpCorutine = true;
        float nowHightPosition = _rb.transform.position.y;
        _jumpTargetGravity.Disable();
        _jumpVelocity = _params.JumpHight / _params.JumpHightTime;
        _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y + _jumpVelocity, _rb.velocity.z);


        while (_playerInput.actions[StaticCommonParams.JUMP_INPUT].ReadValue<float>() > 0 && _rb.transform.position.y - nowHightPosition <= _params.JumpHight)
        {
            yield return StaticCommonParams.Yielders.FixedUpdate;
        }

        Debugger.Log($"{_playerInput.actions[StaticCommonParams.JUMP_INPUT].ReadValue<float>() > 0} : {_rb.transform.position.y - nowHightPosition <= _params.JumpHight} : {_rb.transform.position.y - nowHightPosition} : {_rb.transform.position.y - nowHightPosition}");

        //慣性
        if (_params.IsActiveInertia) yield return CoroutineHandler.StartStaticCoroutine(Inertia());
        else _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);


        float gravityValueSave = _jumpTargetGravity.GravityValue;
        //落下速度を変更する
        _jumpTargetGravity.Enable(_params.JumpFallGravity);
        while (!_characterParam.IsStandingGround)
        {
            yield return null;
        }
        _jumpTargetGravity.Enable(gravityValueSave);

        _isJumpCorutine = false;
    }


    /// <summary>
    /// 固定長慣性
    /// </summary>
    /// <returns>
    /// Max = 制限高さ + 慣性 
    /// </returns>
    private IEnumerator Inertia()
    {
        float _inertiaGravity;
        if (_params.InertiaTime <= 0 || _rb.velocity.y == 0) yield break;
        _inertiaGravity = 2 * (_rb.velocity.y * _params.InertiaTime - _params.InertiaHight) / (_params.InertiaTime * _params.InertiaTime);

        float gravityValueSave = _jumpTargetGravity.GravityValue;
        _characterParam.Gravity.Enable(_inertiaGravity);

        float Timer = 0;
        while (Timer <= _params.InertiaTime)
        {
            //WaiteForSecond(_params.InertiaTime) だと揺らぎが出たので正確性を重視してWaiteFirFixidUpdate
            yield return StaticCommonParams.Yielders.FixedUpdate;
            Timer += Time.deltaTime;
        }
        _characterParam.Gravity.Enable(gravityValueSave);
    }

    private void JumpCancel()
    {

    }
}
