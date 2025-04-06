using UnityEngine;

/// <summary>
/// ターゲットを追従する弾の挙動
/// </summary>
public class MissleBehavior : IBulletBehavior
{
    private Transform _transform;
    private float _velocity;
    private float _angleVelocity;
    private Transform _target;

    private Vector3 _moveDirection;


    private Vector3 _targetDirection;
    public MissleBehavior(Transform tr, Vector3 fireDirection, float velocity, float angleVelocity, Transform target = null)
    {
        _transform = tr;
        _moveDirection = fireDirection;
        if (_moveDirection.sqrMagnitude != 1.0f) _moveDirection = _moveDirection.normalized;
        _velocity = velocity;
        _target = target;
        _angleVelocity = angleVelocity;

    }
    public void OnUpdate()
    {
        _transform.position += _moveDirection * _velocity * Time.deltaTime;

        //ターゲットが未設定なら
        if (!_target) return;


        //ターゲット方向と進捗方向の外積(垂直ベクトル)周りでターゲットに進捗方向を回転させて追従させる
        _targetDirection = _target.position - _transform.position;
        float targetAngle = Vector3.Angle(_moveDirection, _targetDirection);
        Vector3 rotateAsix = Vector3.Cross(_moveDirection, _targetDirection);
        float rotateValue = _angleVelocity * Time.deltaTime;

        if (targetAngle < rotateValue)
        {
            _moveDirection = Quaternion.AngleAxis(targetAngle, rotateAsix) * _moveDirection;
        }
        else
        {
            _moveDirection = Quaternion.AngleAxis(rotateValue, rotateAsix) * _moveDirection;
        }
    }

}
