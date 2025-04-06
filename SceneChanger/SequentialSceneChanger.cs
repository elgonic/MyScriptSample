using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// 連続的なシーンの移行(分岐しない)
/// </summary>
public class SequentialSceneChanger : SceneChangeBase
{
    [SerializeField] private SequentialAcsionStageOrder _actionStageOrder;

    private int? __nextSceneNum;
    private int? _nextSceneNum
    {
        get
        {
            if (!__nextSceneNum.HasValue)
            {
                __nextSceneNum = GetNowScene();
            }
            return __nextSceneNum;
        }
        set
        {
            __nextSceneNum = value;
        }
    }


    private int? GetNowScene()
    {
        int findResult = _actionStageOrder.SceneOrder.FindIndex(element => element.GetSceneName().ToLower() == SceneManager.GetActiveScene().name.ToLower());
        if (findResult == -1)
        {
            Debugger.Log($"{GetType().Name} : {System.Reflection.MethodBase.GetCurrentMethod().Name} : 該当するシーン名が{typeof(SceneName.MainGameSceneType)}に存在しません");
            Debugger.Log($"アクションゲームステージでないなら OK !");
            return null;
        }
        return findResult + 1;
    }

    public override Coroutine NextScene()
    {
        return StartCoroutine(NextSceneCorutine());
    }

    private IEnumerator NextSceneCorutine()
    {
        //最終ステージクリア後は最終シーンへ移行
        if (_nextSceneNum.Value >= _actionStageOrder.SceneOrder.Count)
        {
            _nextSceneNum = null;
            yield return LoadScene(SceneName.END);
        }
        else
        {
            string loadSceneName = _actionStageOrder.SceneOrder[_nextSceneNum.Value].GetSceneName();
            yield return LoadScene(loadSceneName);

            _nextSceneNum++;
        }


        //シーン遷移時にGC起動させておく
        GC.Collect();
        yield return Resources.UnloadUnusedAssets();

    }

}
