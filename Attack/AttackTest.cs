using System.Collections;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 攻撃テストコンポーネント
/// </summary>
public class AttackTest : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private bool _attackTest;
    [SerializeField] private CharacterParam _attacker;
    [SerializeField] private CharacterParam _target;
    [SerializeField] private bool _isLoop = false;
    [SerializeField] private float _loopInterval = 2f;
    private void Update()
    {
        if (_attackTest)
        {
            AttackBase attack = GetComponent<AttackBase>();
            if (!attack) return;
            StartCoroutine(Test(attack));
            _attackTest = false;
        }
    }

    private void OnEnable()
    {
        if (_attackTest)
        {
            AttackBase attack = GetComponent<AttackBase>();
            StartCoroutine(Test(attack));
            _attackTest = false;
        }

    }
    private IEnumerator Test(AttackBase attack)
    {
        if (!EditorApplication.isPlaying) yield break;
        while (true)
        {
            attack.Attack(_attacker, _target);
            Debugger.Log("Attack");
            if (!_isLoop) yield break;
            yield return StaticCommonParams.Yielders.Get(_loopInterval);
            attack.Finish();
        }
    }
#endif
}
