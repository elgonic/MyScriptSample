using UnityEngine;



/// <summary>
/// 直線に発射される弾の挙動
/// </summary>
public class LineBulleltBehavior : IBulletBehavior
{
    private Transform _transform;
    private Vector3 _fireDirection;
    private float _velocity;

    public LineBulleltBehavior(Transform tr, Vector3 fireDirection, float velocity)
    {
        _transform = tr;
        _fireDirection = fireDirection;
        if (_fireDirection.sqrMagnitude != 1.0f) _fireDirection = _fireDirection.normalized;
        _velocity = velocity;
    }

    public void OnUpdate()
    {
        _transform.localPosition = _transform.localPosition + _fireDirection * _velocity * Time.deltaTime;
    }

}
