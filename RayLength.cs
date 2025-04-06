using UnityEngine;



/// <summary>
/// Rayの長さ用クラス
/// </summary>
/// <remarks>
/// 長さを視覚的に設定したほうが楽そうなので
/// </remarks>
public class RayLength : MonoBehaviour
{
    [SerializeField] private float _length;
    [SerializeField] private float _gizmoRadius;

    public Vector3 Direction => transform.forward;
    public Vector3 StartPosition => transform.position;
    public Vector3 EndPosition => transform.position + transform.forward * _length;
    public float Length => _length;



    private GameObject _start;
    private GameObject _end;


    private void OnDrawGizmos()
    {
        //Start
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(StartPosition, _gizmoRadius);

        //End
        Gizmos.DrawSphere(EndPosition, _gizmoRadius);

        //Line
        Gizmos.DrawLine(StartPosition, EndPosition);
    }



}
