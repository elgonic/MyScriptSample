using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// コライダーのイベントのデリゲート用クラス
/// メインのオブジェクトとコライダーが別な時に使う
/// </summary>
/// <remarks>
/// Rigidbodyは無くてもいいけど、無い(Rbがコライダオブジェクトにアタッチされていないと)と衝突オブジェクトにRigitbidyが無いと発火しないよ
/// </remarks>
[RequireComponent(typeof(Collider))]
public class ColliderDelegate : MonoBehaviour
{
    [SerializeField] private Collider ColliderComponent;

    public UnityEvent<Collider, Transform> OnTriggerEnterAction;
    public UnityEvent<Collider, Transform> OnTriggerExitAction;
    public UnityEvent<Collider, Transform> OnTriggerStayAction;

    public UnityEvent<Collision, Transform> OnCollisonEnterAction;
    public UnityEvent<Collision, Transform> OnCollisonExitAction;
    public UnityEvent<Collision, Transform> OnCollisonStayAction;

    protected virtual void Reset()
    {

        ColliderComponent = GetComponent<Collider>();
    }
    private void Awake()
    {
        if (!ColliderComponent) ColliderComponent = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterAction?.Invoke(other, transform);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitAction?.Invoke(other, transform);
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerStayAction?.Invoke(other, transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisonEnterAction?.Invoke(collision, transform);
    }

    private void OnCollisionExit(Collision collision)
    {
        OnCollisonExitAction?.Invoke(collision, transform);
    }
    private void OnCollisionStay(Collision collision)
    {
        OnCollisonStayAction?.Invoke(collision, transform);
    }
}
