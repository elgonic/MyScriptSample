using UnityEngine;


/// <summary>
/// Playerのアニメーション制御クラス
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Player _player;
    private CharacterParam _param;
    private SimpleAnimation _animation;

    private string _attackAnimationName = "Attack";
    private string _jumpStartAnimationName = "JumpStart";
    private string _fallAnimationName = "Fall";
    private string _landingAnimationName = "Landing";

    private bool _isFallAnimationPlayed = false;
    private bool _isLandingAnimationPlayed = false;

    private void Start()
    {
        _param = _player.GetComponent<CharacterParam>();
        _animation = GetComponent<SimpleAnimation>();
        _player.Jump.OnJump.AddListener(Jump);
        _player.OnDash.AddListener(Dash);
        _player.OnKnockBack.AddListener(Knockback);

    }

    private void LateUpdate()
    {
        //落下系
        if (_param.IsStandingGround == false)
        {
            _isLandingAnimationPlayed = false;
            //下降
            if (_param.MoveDirection.y < 0 && !_isFallAnimationPlayed)
            {
                ExclusivePlay(_fallAnimationName, 1f);
                _isFallAnimationPlayed = true;
            }
        }
        //地面着地時
        else
        {
            _isFallAnimationPlayed = false;
            if (!_isLandingAnimationPlayed)
            {
                ExclusivePlay(_landingAnimationName);
                _isLandingAnimationPlayed = true;
            }
        }
    }

    private void Jump()
    {
        ExclusivePlay(_jumpStartAnimationName);
    }

    private void Dash(Vector3 direction)
    {
        ExclusivePlay(_attackAnimationName);
    }

    private void Knockback()
    {
        ExclusivePlay(_attackAnimationName);
    }

    /// <summary>
    /// 排他的アニメーション再生
    /// </summary>
    private void ExclusivePlay(string animationName, float speed = 1)
    {
        Debugger.Log(animationName);
        if (_animation.isPlaying) _animation.Stop();
        _animation.GetState(animationName).speed = speed;
        _animation.Play(animationName);
    }

}
