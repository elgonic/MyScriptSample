using UnityEngine;


/// <summary>
/// ダッシュするエリア
/// </summary>
public class DashZone : MonoBehaviour
{
    [SerializeField] private float _velocityValue;

    private void OnCollisionStay(Collision collision)
    {
        Player target = collision.gameObject.GetComponent<Player>();

        if (!target) return;
        target.IsFreeze = true;
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        targetRb.velocity = transform.forward * _velocityValue;
    }

    private void OnCollisionExit(Collision collision)
    {
        Player target = collision.gameObject.GetComponent<Player>();

        if (!target) return;
        target.IsFreeze = false;
    }
}
