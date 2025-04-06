using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 1つの攻撃を延々と繰り返す敵
/// </summary>
public class OneAttackLoopEnemyBehaviour : MonoBehaviour
{
    [Header("この攻撃の挙動の名前)")]
    [SerializeField] private Transform _attackPositionTr;
    [SerializeField] private AttackBehaviorParams _attackParam;
    [SerializeField, ReadOnly] private List<AttackBase> _instatiateAttackList = new();
    [Header("ステージ上には1発だけしか存在しないようにする")]
    [SerializeField] private bool _isEnebleOnlyOne = false;
    [SerializeField] private float _attackIntervalTime = 10.0f;

    private CharacterParam _attacker;



    private int _initialInstatiateNum = 10;
    private Coroutine _coroutine = null;
    private void OnDisable()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private void Start()
    {
        //if (!this.enabled) return;
        _attacker = GetComponent<CharacterParam>();
        AudioManager a = AudioManager.Instance;
        if (!_attackPositionTr) _attackPositionTr = transform;
        for (int i = 0; i < _initialInstatiateNum; i++)
        {
            AttackBase attackBase = Instantiate(_attackParam.Attack, _attackPositionTr.position, Quaternion.identity);
            attackBase.gameObject.SetActive(false);
            _instatiateAttackList.Add(attackBase);
        }

        MainGameSystem.Instance.OnMainStageEntry.AddListener(() => _coroutine = StartCoroutine(Attack()));
        MainGameSystem.Instance.OnPlayerRespwn.AddListener(() => { if (_coroutine != null) StopCoroutine(_coroutine); });
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            if (_isEnebleOnlyOne)
            {
                //有効な奴があれば やめる
                if (_instatiateAttackList.Find(element => element.gameObject.activeSelf == true))
                {
                    yield return null;
                    continue;
                }
            }
            AttackBase attack = _instatiateAttackList.Find(element => element.gameObject.activeSelf == false);
            if (attack == null)
            {
                attack = Instantiate(_attackParam.Attack, _attackPositionTr.position, Quaternion.identity);
                attack.gameObject.SetActive(false);
                _instatiateAttackList.Add(attack);
            }
            attack.Attack(_attacker);
            StartCoroutine(LateReset(attack, _attackParam.AttackTime));
            yield return StaticCommonParams.Yielders.Get(_attackIntervalTime);

        }
    }

    private IEnumerator LateReset(AttackBase attackBase, float destroyTime)
    {
        yield return StaticCommonParams.Yielders.Get(destroyTime);

        yield return attackBase.Finish();
    }
    [Serializable]
    public class AttackBehaviorParams
    {
        [SerializeField] public AttackBase Attack;
        [SerializeField] public float AttackTime;

    }

}


