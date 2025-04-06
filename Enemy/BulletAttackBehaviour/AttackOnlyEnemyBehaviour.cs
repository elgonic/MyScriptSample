using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 一定間隔で攻撃しかしない
/// </summary>
public class AttackOnlyEnemyBehaviour : MonoBehaviour
{
    [Header("この攻撃の挙動の名前)")]
    [SerializeField] private bool _attackWhenEnebale = false;

    [SerializeField] private Transform _attackPositionTr;
    [SerializeField] private AudioSource _audioSouce;
    /// <summary>
    /// このリストから昇順で攻撃を行う
    /// </summary>
    [SerializeField] private List<AttackBehaviorParams> _attackList;
    [SerializeField, ReadOnly] private List<AttackBase> _instatiateAttackList = new();
    [SerializeField] private float _attackIntervalTime = 10.0f;

    // Start is called before the first frame update

    private Coroutine _coroutine = null;

    private CharacterParam _attacker;

    private void OnValidate()
    {

    }

    private void OnEnable()
    {
        if (_attackWhenEnebale)
        {
            _coroutine = StartCoroutine(Attack());
        }
    }

    private void OnDisable()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private void Awake()
    {
        //if (!this.enabled) return;
        _attacker = GetComponent<CharacterParam>();
        AudioManager a = AudioManager.Instance;
        if (!_attackPositionTr) _attackPositionTr = transform;
        foreach (var attack in _attackList)
        {
            int instatiateNum;
            if (attack.LoopCount == 0) instatiateNum = 1;
            else instatiateNum = attack.LoopCount;
            for (int i = 0; i < instatiateNum; i++)
            {
                AttackBase attackBase = Instantiate(attack.Attack, _attackPositionTr.position, Quaternion.identity);
                attackBase.gameObject.SetActive(false);
                _instatiateAttackList.Add(attackBase);
            }
        }
    }

    //Awakeだと componentを falseにしても実行されるので
    private void Start()
    {

        MainGameSystem.Instance.OnMainStageEntry.AddListener(() => _coroutine = StartCoroutine(Attack()));
        MainGameSystem.Instance.OnPlayerRespwn.AddListener(() => { if (_coroutine != null) StopCoroutine(_coroutine); });
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            Debugger.Log("Start Attack");
            int i = 0;
            int beforAttackCount = 0;
            foreach (var attack in _attackList)
            {
                yield return StaticCommonParams.Yielders.Get(attack.Delay);

                do
                {
                    AttackBase attackBase = _instatiateAttackList[i];
                    i++;
                    attackBase.gameObject.SetActive(true);
                    if (_audioSouce) _audioSouce.Play();
                    attackBase.Attack(_attacker);
                    StartCoroutine(LateReset(attackBase, attack.AttackTime));
                    if (attack.LoopCount <= i - beforAttackCount) break;
                    yield return StaticCommonParams.Yielders.Get(attack.LoopInterVal);
                } while (true);
                beforAttackCount = i;
            }
            Debugger.Log("Finish Attack");
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

    }

}


