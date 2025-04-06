using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// メインステージ
/// </summary>
public class BossFieldInfo : MonoBehaviour
{
    [SerializeField] private List<PlaneObject> _planes;
    [SerializeField] private Transform _checkBaseTr;
    [SerializeField] private bool _getPlaneInfo = false;

    private List<string> _planeTag = new List<string> { StaticCommonParams.GROUND_TAG, StaticCommonParams.WALL_TAG };
    public List<Plane> FieldPlaneList => _planes.ConvertAll(x => x.Plane);

    public Plane Ground => _planes.Find(x => x.PlaneGameObject.CompareTag(StaticCommonParams.GROUND_TAG)).Plane;


    [Serializable]
    public class PlaneObject
    {

        [SerializeField] private Vector3 _normal;
        [SerializeField] private Vector3 _point;
        [SerializeField] private GameObject _planeObject;
        public PlaneObject(Vector3 normal, Vector3 point, GameObject planeObject)
        {
            _normal = normal;
            _point = point;
            _planeObject = planeObject;
        }

        public GameObject PlaneGameObject => _planeObject;
        public Plane Plane
        {
            get
            {
                return new Plane(_normal, _point);
            }
            set
            {
                _normal = value.normal;
                _point = value.ClosestPointOnPlane(Vector3.zero);
            }
        }

    }




    private void OnValidate()
    {
        if (_getPlaneInfo)
        {
            _getPlaneInfo = false;
            GetFieldPlane();
        }
    }


    private void OnDrawGizmos()
    {
        if (_planes.Count == 0) return;
        foreach (var plane in _planes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(plane.PlaneGameObject.transform.position, plane.Plane.normal * 100);
        }
    }

    /// <summary>
    /// 子要素の壁と地面の表面を取得
    /// </summary>
    /// <remarks>
    /// Rayを飛ばして表面を取得するので絶対にすべての壁から内側に置くこと！
    /// </remarks>
    private void GetFieldPlane()
    {
        _planes.Clear();
        foreach (Transform child in transform)
        {
            foreach (string tag in _planeTag)
            {
                if (!child.CompareTag(tag)) continue;

                //中心から面への方向
                Vector3 direction = (child.position - _checkBaseTr.position);
                RaycastHit[] hits = Physics.RaycastAll(_checkBaseTr.position, direction, direction.magnitude + 100);
                foreach (RaycastHit hit in hits)
                {
                    if (!(hit.transform == child)) continue;
                    Plane plane = new Plane(hit.normal, hit.point);
                    Vector3 nomal = hit.normal;
                    //hit normalの正負が衝突した面の向き基準なので, 中心を正に変換
                    if (Vector3.Angle(_checkBaseTr.position - plane.ClosestPointOnPlane(_checkBaseTr.position), hit.normal) > 90) nomal = -nomal;
                    _planes.Add(new PlaneObject(nomal, hit.point, hit.transform.gameObject));
                }
            }
        }
    }





}
