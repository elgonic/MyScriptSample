using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ルート分岐管理
/// </summary>
public class BranchSceneChanger : SceneChangeBase
{
    [SerializeField] private SequentialAcsionStageOrder _beforeMainstageTutorialOrder;
    [SerializeField] private BranchSceneRouteOrder.BranchSceneData _entryStage;
    [SerializeField] private List<BranchSceneRouteOrder> _routeOrderList;



    //テスト用
    public bool IsResultTypeTestMode { get; set; } = false;
    public BranchSceneRouteOrder.ResultType TestResultType { get; set; }
    public bool IsScoreTestMode { get; set; } = false;
    public BranchSceneRouteOrder.Score TestScore { get; set; } = new BranchSceneRouteOrder.Score(0);



    private float _stageStartTime;
    private float _stageEndTime;


    private int _tutorialCount = 0;
    private bool _isDoneEntryStage = false;
    private int _stageCount = 0;
    private int _routeNum = 0;
    private bool _isClear = false;


    public int TutorialCount => _tutorialCount;
    public bool IsDoneEntryStage => _isDoneEntryStage;

    public float StageTime => Time.time - _stageStartTime;
    public int StageCount => _stageCount;
    public int RouteNum => _routeNum;

    public BranchSceneRouteOrder.ResultType ResultType => (BranchSceneRouteOrder.ResultType)Enum.ToObject(typeof(BranchSceneRouteOrder.ResultType), RouteNum);
    public bool IsClear => _isClear;

    private void Start()
    {

        //途中開始対応のため
        //現在のシーンがルートのどこにあるのかを探す
        string currentSceneName = SceneManager.GetActiveScene().name.ToLower();
        int findResult = _beforeMainstageTutorialOrder.SceneOrder.FindIndex(element => element.GetSceneName().ToLower() == SceneManager.GetActiveScene().name.ToLower());
        if (findResult > -1)
        {
            _tutorialCount = findResult + 1;
            _isDoneEntryStage = false;
            _stageCount = 0;
            _routeNum = 0;
            _isClear = false;
            return;
        }
        if (_entryStage.SceneName.GetSceneName().ToLower() == currentSceneName)
        {
            _tutorialCount = _beforeMainstageTutorialOrder.SceneOrder.Count;
            _isDoneEntryStage = true;
            _stageCount = 0;
            _routeNum = 0;
            _isClear = false;
            return;
        }
        for (int i = 0; i < _routeOrderList.Count; i++)
        {

            findResult = _routeOrderList[i].RouteSceneList.FindIndex(element => element.SceneName.GetSceneName().ToLower() == SceneManager.GetActiveScene().name.ToLower());
            if (findResult > -1)
            {
                _tutorialCount = _beforeMainstageTutorialOrder.SceneOrder.Count;
                _isDoneEntryStage = true;
                _stageCount = findResult + 1;
                _routeNum = i;
                _isClear = false;
                return;
            }
        }
        Debugger.Log("現在のシーンがルートのどこにも含まれていません. Tiite , End なら正常 .");
        Debugger.Log("それ以外の場合 Route情報に現在のシーンを含める必要あり(ゲームに使用する場合)");

    }

    public override Coroutine LoadTittleScene()
    {
        _isClear = false;
        _tutorialCount = 0;
        _isDoneEntryStage = false;
        _routeNum = 0;
        _stageCount = 0;

        return base.LoadTittleScene();
    }

    public override Coroutine NextScene()
    {
        return StartCoroutine(NextSceneCoroutine());
    }

    private IEnumerator NextSceneCoroutine()
    {
        //クリア後 ot タイトルに持った時の初期化
        if (_isClear)
        {
            _isClear = false;
            _tutorialCount = 0;
            _isDoneEntryStage = false;
            _routeNum = 0;
            _stageCount = 0;
        }

        _stageEndTime = Time.time;
        //初めのチュートリアルステージ
        if (_tutorialCount < _beforeMainstageTutorialOrder.SceneOrder.Count)
        {
            Debugger.Log($"{GetType().Name} : Tutorial Open : {_beforeMainstageTutorialOrder.SceneOrder[_tutorialCount].GetSceneName()}");
            yield return LoadScene(_beforeMainstageTutorialOrder.SceneOrder[_tutorialCount].GetSceneName());
            _tutorialCount++;
        }

        //共通の試金石ステージ
        else if (!_isDoneEntryStage)
        {
            Debugger.Log($"{GetType().Name} : EntryStage Open : {_entryStage.SceneName.GetSceneName()})");
            yield return LoadScene(_entryStage.SceneName.GetSceneName());
            _stageStartTime = Time.time;
            _isDoneEntryStage = true;
        }

        //ルート分岐
        else
        {
            if (_stageCount < _routeOrderList[_routeNum].RouteSceneList.Count)
            {


                BranchSceneRouteOrder.Score score = GetScore();
                BranchSceneRouteOrder.ResultType resultType = GetResult(score);

                Debugger.Log($"ResultType : {resultType.ToString()} ");
                if (_stageCount == 0)
                {
                    _routeNum = (int)resultType;
                }
                else
                {
                    switch (resultType)
                    {
                        case BranchSceneRouteOrder.ResultType.Excellent:
                            if (_routeNum < (int)BranchSceneRouteOrder.ResultType.Excellent)
                            {
                                _routeNum++;
                                if (_stageCount < _routeOrderList[_routeNum].RouteSceneList.Count)
                                {
                                }
                                else
                                {
                                    Debugger.Log($"{GetType().Name} : {resultType.ToString()} : ですが移行先に{_stageCount}番のステージ存在しないのでルート分岐しない");
                                    //戻す
                                    _routeNum--;
                                }
                            }
                            break;
                        case BranchSceneRouteOrder.ResultType.Great:
                            break;
                        case BranchSceneRouteOrder.ResultType.Good:
                            if (_routeNum > 0)
                            {
                                _routeNum--;
                                if (_stageCount < _routeOrderList[_routeNum].RouteSceneList.Count)
                                {
                                }
                                else
                                {
                                    Debugger.Log($"{GetType().Name} : {resultType.ToString()} : ですが移行先に{_stageCount}番のステージ存在しないのでルート分岐しない");
                                    //戻す
                                    _routeNum++;
                                }

                            }
                            break;
                        default: break;
                    }


                }

                //ルートの次のシーン移行
                Debugger.Log($"{GetType().Name} : NextStage Open : {_routeOrderList[_routeNum].RouteSceneList[_stageCount].SceneName.GetSceneName()})");
                yield return LoadScene(_routeOrderList[_routeNum].RouteSceneList[_stageCount].SceneName.GetSceneName());
                _stageCount++;
                _stageStartTime = Time.time;
            }
            else
            {
                Debugger.Log($"{GetType().Name} : End  Open :{SceneName.END} ");
                yield return LoadScene(SceneName.END);
                _isClear = true;
            }


        }

        //シーン遷移時にGC起動させておく
        GC.Collect();
        yield return Resources.UnloadUnusedAssets();

    }



    private BranchSceneRouteOrder.Score GetScore()
    {
        if (IsScoreTestMode) return TestScore;
        return new BranchSceneRouteOrder.Score(_stageEndTime - _stageStartTime);
    }

    private BranchSceneRouteOrder.ResultType GetResult(BranchSceneRouteOrder.Score score)
    {
        if (IsResultTypeTestMode)
        {
            return TestResultType;
        }
        if (_stageCount == 0)
        {

            return _entryStage.Result(score);
        }
        else
        {
            return _routeOrderList[_routeNum].RouteSceneList[_stageCount].Result(score);
        }
    }


}
