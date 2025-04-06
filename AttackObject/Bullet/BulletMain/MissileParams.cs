using UnityEngine;

[CreateAssetMenu(fileName = nameof(Missile) + "Param", menuName = StaticCommonParams.BulletBaseMenuName + nameof(Missile))]

public class MissileParams : ScriptableObject
{
    [SerializeField] private float _velocity = 3.0f;
    [SerializeField] private float _angleVelocity = 10f;
    [SerializeField] private bool _isEnableLifeTime = true;
    [SerializeField] private float _lifeTime = 10f;
    [SerializeField] private bool _isEnablePlaneHitCheck = true;
    [SerializeField] private bool _isDestroyWhenDisable = false;
    public float Velocity => _velocity;
    public float AngleVelocity => _angleVelocity;
    public bool IsEnableLifeTime => _isEnableLifeTime;
    public float LifeTime => _lifeTime;
    public bool IsEnablePlaneHitCheck => _isEnablePlaneHitCheck;
    public bool IsDestroyWhenDisable => _isDestroyWhenDisable;


}
