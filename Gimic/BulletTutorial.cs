using System.Collections;
using UnityEngine;


/// <summary>
/// チュートリアル用の弾を発射するギミック
/// </summary>
[RequireComponent(typeof(CharacterParam))]
public class BulletTutorial : MonoBehaviour
{
    [SerializeField] private BulletBase _bulelt;
    [SerializeField] private ColliderDelegate _fireTrigger;
    [SerializeField] private float _fireIntervalTime = 1f;

    private CharacterParam _characterParam;
    private Coroutine _coroutine;

    private void Start()
    {
        _characterParam = GetComponent<CharacterParam>();

        _fireTrigger.OnTriggerEnterAction.AddListener(TriggerEnter);
        MainGameSystem.Instance.OnMainStageEntry.AddListener(() =>
        {
            if (_coroutine != null)
            {
                Debugger.Log("Stop");
                StopCoroutine(_coroutine);
            }
        });
    }
    private void TriggerEnter(Collider hitObject, Transform tr)
    {
        if (hitObject.transform.CompareTag(StaticCommonParams.PLAYER_TAG) && _coroutine == null)
        {
            _coroutine = StartCoroutine(Fire(hitObject.transform));
        }
    }

    private IEnumerator Fire(Transform target)
    {
        while (true)
        {
            BulletBase bullet = Instantiate(_bulelt, transform);
            bullet.transform.position = transform.position;
            bullet.Fire((target.position - transform.position).normalized, new AttackData(transform.position, _characterParam, true));
            yield return StaticCommonParams.Yielders.Get(_fireIntervalTime);
        }
    }
}
