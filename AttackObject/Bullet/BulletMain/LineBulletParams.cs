using UnityEngine;

[CreateAssetMenu(fileName = nameof(LineBullet) + "Param", menuName = StaticCommonParams.BulletBaseMenuName + nameof(LineBullet))]
public class LineBulletParams : ScriptableObject
{
    [SerializeField] private float _velocity = 3.0f;
    [SerializeField] private bool _isEnabledLifeTime = true;
    [SerializeField] private float _lifeTime = 10f;
    [SerializeField] private bool _isEnablePlaneHitCheck = true;
    [SerializeField] private bool _isDestroyWhenDisable = false;

    /// <summary>
    /// 攻撃者の移動速度を追加する
    /// </summary>
    [SerializeField] private bool _isConsiderationAttackerVelocity = false;
    public float Velocity => _velocity;
    public bool IsEnableLifeTime => _isEnabledLifeTime;
    public float LifeTime => _lifeTime;
    public bool IsEnablePlaneHitCheck => _isEnablePlaneHitCheck;

    public bool IsDestroyWhenDisable => _isDestroyWhenDisable;

    public bool IsConsiderationAttackerVelocity => _isConsiderationAttackerVelocity;



}

