using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 設定された攻撃をランダムに行う
/// </summary>
public class RandamAttackOnlyEnemyBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _attackPositionTr;
    [SerializeField] private AudioSource _audioSouce;
    [SerializeField] private List<AttackBehaviorParams> _attackList;
    [SerializeField] private List<AttackBase> _instatiateAttackList = new();
    [SerializeField] private float _attackIntervalTime = 10.0f;

    private CharacterParam _attacker;


    // Start is called before the first frame update

    private Coroutine _corutine = null;
    private void OnDisable()
    {
        if (_corutine != null)
        {
            StopCoroutine(_corutine);
            _corutine = null;
        }
    }

    private void OnEnable()
    {
        _corutine = StartCoroutine(Attack());
    }


    private void Awake()
    {
        _attacker = GetComponent<CharacterParam>();
        AudioManager a = AudioManager.Instance;
        if (!_attackPositionTr) _attackPositionTr = transform;
        foreach (var attack in _attackList)
        {
            int instatiateNum;
            if (attack.LoopCount == 0) instatiateNum = 1;
            else instatiateNum = attack.LoopCount;

            attack.BeginInstatiateNum = _instatiateAttackList.Count;
            attack.EndInstatiateNum = _instatiateAttackList.Count - 1 + instatiateNum;


            for (int i = 0; i < instatiateNum; i++)
            {
                AttackBase attackBase = Instantiate(attack.Attack, _attackPositionTr.position, Quaternion.identity);
                attackBase.gameObject.SetActive(false);
                _instatiateAttackList.Add(attackBase);
            }
        }

    }

    private IEnumerator Attack()
    {


        while (true)
        {

            ///既にActive(攻撃して出現している 最後の要素がActiveかで判断) 奴は除いて ランダム抽出する
            int attackNum = UnityEngine.Random.Range(0, _attackList.Count);
            while (_instatiateAttackList[_attackList[attackNum].EndInstatiateNum].gameObject.activeSelf)
            {
                attackNum = UnityEngine.Random.Range(0, _attackList.Count);
                yield return null;
            }


            AttackBehaviorParams attackParam = _attackList[attackNum];
            for (int i = 0; i < attackParam.LoopCount; i++)
            {
                if (_audioSouce) _audioSouce.Play();
                _instatiateAttackList[attackParam.BeginInstatiateNum + i].gameObject.SetActive(true);
                _instatiateAttackList[attackParam.BeginInstatiateNum + i].Attack(_attacker);
                StartCoroutine(LateReset(_instatiateAttackList[attackParam.BeginInstatiateNum + i], attackParam.AttackTime));
                yield return StaticCommonParams.Yielders.Get(attackParam.LoopInterVal);

            }
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
        [SerializeField] public float Delay;
        [SerializeField] public int LoopCount = 1;
        [SerializeField] public float LoopInterVal;
        public int BeginInstatiateNum = 0;
        public int EndInstatiateNum = 0;

    }

}


