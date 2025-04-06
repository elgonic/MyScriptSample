using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// レーザー
/// </summary>
public class Laser : BulletBase
{
    /// <summary>
    /// 発射時の予兆の有効化
    /// </summary>
    [SerializeField] private bool _enableAnticipation = false;
    [SerializeField] private float _anticipationTime = 0.5f;
    [SerializeField] private float _anticipationRadius = 0.01f;
    [SerializeField] private float _anticipationAnimationTime = 1.0f;
    [SerializeField][NormalizedAnimationCurve(false, true)] private AnimationCurve _anticipationAnimationCurve;

    [SerializeField] private float _radius = 1f;
    [SerializeField] private float _length = 5f;

    [SerializeField] private bool _isEnabledLifeTime = false;
    [SerializeField] private float _lifeTime = 2f;


    [SerializeField] private EmptyBullet _laserObject;
    [SerializeField] private bool _fireTest = false;

    public UnityEvent OnAnticipation;

    public float AnticipationTime
    {
        get
        {
            if (!_enableAnticipation) return 0;
            else return _anticipationTime;
        }
        set { _anticipationTime = value; }
    }
    public float AnticipationAnimationTime => _anticipationAnimationTime;
    public float Length => _length;


    public bool IsEnabledLifeTime { get { return _isEnabledLifeTime; } set { _isEnabledLifeTime = value; } }
    public float LifeTime { get { return _lifeTime; } set { _lifeTime = value; } }

    [SerializeField, HideInInspector] private Collider _laserCollider;


    private void OnValidate()
    {
        SetLength(_length);
        if (_enableAnticipation) SetRadius(_anticipationRadius);
        else SetRadius(_radius);
        if (_fireTest)
        {
            _fireTest = false;
            FireTest(transform.forward);
        }
        if (_laserObject) _laserCollider = _laserObject.GetComponentInChildren<Collider>();
    }


    public void SetRadius(float radius)
    {
        transform.localScale = new Vector3(radius, radius, transform.localScale.z);
    }

    public void SetLength(float length)
    {
        float radius = transform.localScale.x;
        transform.localScale = new Vector3(radius, radius, length);
    }

    public override void Fire(Vector3 fireDirection, AttackData attackData, Transform target = null)
    {
        gameObject.SetActive(true);
        StartCoroutine(FireCoroutine(fireDirection, attackData, target));
    }

    private IEnumerator FireCoroutine(Vector3 fireDirection, AttackData attackData, Transform target = null)
    {


        transform.rotation.SetLookRotation(fireDirection + transform.position);

        if (_enableAnticipation)
        {
            OnAnticipation?.Invoke();
            yield return StartCoroutine(AnticipationCoroutine());
        }

        AttackData = attackData;

        OnFire?.Invoke();
        _laserObject?.Fire(fireDirection, attackData, target);
        if (_laserObject) _laserObject.ParentBullet = this;

        if (!_isEnabledLifeTime) yield break;

        yield return StaticCommonParams.Yielders.Get(_lifeTime);

        yield return Finish();

    }

    private IEnumerator AnticipationCoroutine()
    {
        float timer = 0;
        float radiusDiff = _radius - _anticipationRadius;
        float radius = _anticipationRadius;

        _laserCollider.enabled = false;
        SetRadius(_anticipationRadius);

        yield return StaticCommonParams.Yielders.Get(_anticipationTime);

        while (timer <= _anticipationAnimationTime)
        {
            radius = _anticipationRadius + _anticipationAnimationCurve.Evaluate(timer / _anticipationAnimationTime) * radiusDiff;
            SetRadius(radius);
            yield return null;
            timer += Time.deltaTime;
        }
        _laserCollider.enabled = true;

    }

    public Coroutine Finish()
    {
        if (!gameObject.activeSelf) return null;
        OnLifeTime?.Invoke();
        return StartCoroutine(HideCoroutine());
    }
    private IEnumerator HideCoroutine()
    {
        float timer = 0;
        float radiusDiff = 0 - _radius;
        float radius = _radius;

        _laserCollider.enabled = false;
        SetRadius(_radius);
        while (timer <= _anticipationAnimationTime)
        {
            radius = _radius + _anticipationAnimationCurve.Evaluate(timer / _anticipationAnimationTime) * radiusDiff;
            SetRadius(radius);
            yield return null;
            timer += Time.deltaTime;
        }
        ResetTransform();
    }


    public override void Hit(GameObject hitObject)
    {
        OnHit?.Invoke();
    }

    public override void Parryed(AttackObjectBase attackerData)
    {
        OnParryed?.Invoke(attackerData);
    }

    public override void ResetTransform()
    {
        gameObject.SetActive(false);
        SetRadius(_anticipationRadius);
    }

}
