using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// ルート分岐テスト
/// </summary>
public class BranchSceneChangerTester : MonoBehaviour
{
    [SerializeField] private BranchSceneChanger _sceneChanger;


    [SerializeField] private bool _nextSceneUseType;

    [SerializeField] private BranchSceneRouteOrder.ResultType _clearType;

    [SerializeField] private float _clearTime;
    [SerializeField] private bool _nextSceneUseClearTime;


    [SerializeField] private TextMeshProUGUI _debugUI;

    private void Reset()
    {
        _sceneChanger = GetComponent<BranchSceneChanger>();
    }

    private void LateUpdate()
    {
        if (_debugUI == null) return;
        _debugUI.text = $"StageTime :  <mspace=0.7em>{_sceneChanger.StageTime.ToString("f1")}</mspace>\n";
        _debugUI.text += $"TutorialCount : {_sceneChanger.TutorialCount}\n";
        _debugUI.text += $"IsDoneEntryStage: {_sceneChanger.IsDoneEntryStage}\n";
        _debugUI.text += $"StageCount : {_sceneChanger.StageCount}\n";
        _debugUI.text += $"RouteNum : {_sceneChanger.RouteNum} \n";
        _debugUI.text += $"IsClear : {_sceneChanger.IsClear}\n";


    }

    private IEnumerator Start()
    {
        while (true)
        {
            if (_nextSceneUseType)
            {
                Debugger.Log($"{GetType().Name} : ActiveScene = {SceneManager.GetActiveScene().name}");
                if (SceneName.TITTLE.ToLower() == SceneManager.GetActiveScene().name.ToLower())
                {
                    Debugger.Log($"{GetType().Name} : TittleOpen");
                    yield return _sceneChanger.NextScene();
                }
                else if (SceneName.END.ToLower() == SceneManager.GetActiveScene().name.ToLower())
                {
                    Debugger.Log($"{GetType().Name} : EndOpen");
                    yield return _sceneChanger.LoadScene(SceneName.TITTLE);
                }
                else
                {
                    _sceneChanger.IsResultTypeTestMode = true;
                    _sceneChanger.TestResultType = _clearType;
                    Debugger.Log($"{GetType().Name} : NextOpen : {_sceneChanger.TestResultType.ToString()}");
                    yield return _sceneChanger.NextScene();
                    _sceneChanger.IsResultTypeTestMode = false;
                }
                _nextSceneUseType = false;
            }

            if (_nextSceneUseClearTime)
            {
                Debugger.Log($"{GetType().Name} : ActiveScene = {SceneManager.GetActiveScene().name}");
                if (SceneName.TITTLE.ToLower() == SceneManager.GetActiveScene().name.ToLower())
                {
                    Debugger.Log($"{GetType().Name} : TittleOpen");
                    yield return _sceneChanger.NextScene();
                }
                else if (SceneName.END.ToLower() == SceneManager.GetActiveScene().name.ToLower())
                {
                    Debugger.Log($"{GetType().Name} : EndOpen");
                    yield return _sceneChanger.LoadScene(SceneName.TITTLE);
                }
                else
                {
                    _sceneChanger.IsScoreTestMode = true;
                    _sceneChanger.TestScore = new BranchSceneRouteOrder.Score(_clearTime);
                    Debugger.Log($"{GetType().Name} : NextOpen ");
                    yield return _sceneChanger.NextScene();
                    _sceneChanger.IsScoreTestMode = false;
                }
                _nextSceneUseClearTime = false;

            }

            yield return null;
        }
    }
}
