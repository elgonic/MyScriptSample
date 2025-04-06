using UnityEngine;



/// <summary>
/// オブジェクト間の距離測定クラス
/// </summary>
[ExecuteAlways]
public class Measurement : MonoBehaviour
{
    [SerializeField] private GameObject _target1;
    [SerializeField] private GameObject _target2;

    [SerializeField] private bool _getDistance = false;

    private void OnValidate()
    {
    }

    private void Update()
    {
        if (_getDistance)
        {
            GetDistance(_target1, _target2);
        }

    }
    private static void GetDistance(GameObject target1, GameObject target2)
    {
#if UNITY_EDITOR
        Vector3 distanveVector = target2.transform.position - target1.transform.position;
        Debug.DrawLine(target1.transform.position, target1.transform.position + distanveVector, Color.red);
        Debugger.Log($"GetDistance : {distanveVector.magnitude}");
#endif
    }

}
