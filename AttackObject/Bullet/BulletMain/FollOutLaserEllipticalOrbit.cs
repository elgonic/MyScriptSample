using System.Collections;
using UnityEngine;


/// <summary>
/// 上から発射されて、円軌道や振り子軌道を行うレーザー
/// </summary>
public class FollOutLaserEllipticalOrbit : BulletBase
{

    [SerializeField] private bool _reverseRotate = false;
    [SerializeField] private float _angleVelocity;
    [SerializeField] private float _ellipticalWidth;
    [SerializeField] private float _ellipticalHeight;
    [SerializeField] private Transform _orbitPoint;
    [SerializeField] private Laser _laser;
    [SerializeField] private float _fireDelay = 0;
    [SerializeField] private float _firstAngle = 0;
    [SerializeField] private bool _fireTest = false;
    [SerializeField] private bool _setPosition = false;


    public float FireDelay { set { _fireDelay = value; } }


    private Transform _transform;
    private Vector3 _resetLocalPosition;
    private Quaternion _resetLocalRotation;

    private bool _isFire = false;
    private Vector3 _fireDirection;

    private Transform _attacker;


    //中心への法線
    private Vector3 _nomalVector;
    // 中心方向との垂直線
    private Vector3 _crossVector;

    private float _angle = 0;

    private void OnValidate()
    {
        if (_fireTest)
        {
            _fireTest = false;
            FireTest(transform.forward);
        }
        if (_setPosition)
        {
            _setPosition = false;
            SetPosition();
        }
    }

    //位置調整
    private void SetPosition()
    {
        _orbitPoint.position = new Vector3(
            transform.position.x,
            -transform.position.y + _laser.Length,
            transform.position.z
            );
        _orbitPoint.localPosition += new Vector3(0, 0, 1) * _ellipticalWidth * 0.5f;
        _orbitPoint.localPosition += new Vector3(1, 0, 0) * _ellipticalHeight * 0.5f;

        _laser.transform.LookAt(_orbitPoint);
    }

    private IEnumerator Start()
    {
        _angle = _firstAngle;
        _transform = transform;
        SetPosition();
        _nomalVector = (new Vector3(
            _transform.position.x,
            -transform.position.z + _laser.Length,
            _transform.position.z
            ) - _orbitPoint.position).normalized;
        _crossVector = Vector3.Cross(_nomalVector, transform.up).normalized;

        Debug.DrawLine(transform.position, _nomalVector * 10 + transform.position, Color.blue, 1000f);
        Debug.DrawLine(transform.position, _crossVector * 10 + transform.position, Color.red, 1000f);

        yield return StaticCommonParams.Yielders.Get(_fireDelay);
        _isFire = true;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!_isFire) return;
        if (_reverseRotate)
        {
            _reverseRotate = false;
            _angleVelocity *= -1;
        }
        _orbitPoint.localPosition = new Vector3(
                                               MyMathf.Sin(_angle) * _ellipticalHeight * 0.5f,
                                               -_laser.Length,
                                               MyMathf.Cos(_angle) * _ellipticalWidth * 0.5f
                                               );

        _angle += _angleVelocity * Time.deltaTime;
        _laser.transform.LookAt(_orbitPoint);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (_orbitPoint) Gizmos.DrawSphere(_orbitPoint.position, 1);
    }

    /// <summary>
    /// 発射
    /// </summary>
    /// <param name="fireDirection"> 楕円軌道の細いほうの先端の方向</param>
    /// <param name="attackData"></param>
    /// <param name="target"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public override void Fire(Vector3 fireDirection, AttackData attackData, Transform target = null)
    {
        throw new System.NotImplementedException();
    }


    public override void Hit(GameObject hitObject)
    {
        Debugger.Log($"{GetType().Name} の {System.Reflection.MethodBase.GetCurrentMethod().Name}は空です");
    }
    public override void Parryed(AttackObjectBase attackerData)
    {
        Debugger.Log($"{GetType().Name} の {System.Reflection.MethodBase.GetCurrentMethod().Name}は空です");
    }
    public override void ResetTransform()
    {
        Debugger.Log($"{GetType().Name} の {System.Reflection.MethodBase.GetCurrentMethod().Name}は空です");

    }



}
