using LitMotion;
using LitMotion.Extensions;
using System.Collections;
using UnityEngine;


/// <summary>
/// ゴールに使用するシリンダーのアニメーション
/// </summary>
public class CylinderGoalObject : BaseGoalObjet
{
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private GameObject _cylinder;
    [SerializeField] private Vector3 _cylincderTargetScale;
    [SerializeField] private float _transformTime = 0.5f;
    [SerializeField] private Ease _ease;
    [Header("ForTest")]
    [SerializeField] private Vector3 _scale;
    [SerializeField] private bool _test = false;


    private bool _inportTestScale = false;

    private Vector3 _xzZeroScale;
    private void OnValidate()
    {
        if (_cylinder != null && _inportTestScale == false)
        {
            _scale = _cylinder.transform.localScale;
            _inportTestScale = true;

        }
        else
        {
            _inportTestScale = false;
        }
        _cylinder.transform.localScale = _scale;

        if (_test)
        {
            StartCoroutine(Test());
        }
    }


    private void Start()
    {
        SetUp();
        _xzZeroScale = new Vector3(0, _cylincderTargetScale.y, 0);
    }

    private void SetUp()
    {
        _particle.Stop();
        _cylinder.SetActive(false);
        _cylinder.transform.localScale = _xzZeroScale;

    }
    public override Coroutine Enable()
    {
        _cylinder.SetActive(true);
        LMotion.Create(_xzZeroScale, _cylincderTargetScale, _transformTime).WithEase(_ease).BindToLocalScale(_cylinder.transform).ToYieldInteraction();
        _particle.Play();
        return null;
    }


    public override Coroutine Disable()
    {
        return StartCoroutine(DisableCoroutine());
    }

    private IEnumerator DisableCoroutine()
    {
        yield return LMotion.Create(_cylincderTargetScale, _xzZeroScale, _transformTime).WithEase(_ease).BindToLocalScale(_cylinder.transform).ToYieldInteraction();
        SetUp();
    }

    private IEnumerator Test()
    {
        SetUp();
        Enable();
        yield return new WaitUntil(() => _test == false);
        Disable();
    }

}
