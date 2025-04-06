using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// シーン遷移システムの基底クラス
/// </summary>
public abstract class SceneChangeBase : MonoBehaviour
{
    /// トランジション
    private ITransition _transiton;
    public ITransition Transition
    {
        get
        {
            if (_transiton == null) _transiton = GetComponent<ITransition>();
            return _transiton;
        }
    }
    public Coroutine LoadScene(SceneName.MainGameSceneType loadSceneType)
    {
        return StartCoroutine(LoadSceneCorutine(SceneName.GetSceneName(loadSceneType)));
    }

    public Coroutine LoadScene(string loadSceneName)
    {
        return StartCoroutine(LoadSceneCorutine(loadSceneName));
    }

    private IEnumerator LoadSceneCorutine(string sceneName)
    {
        yield return Transition?.Hide();

        //現在のシーンアンロード
        string unloadSceneName = SceneManager.GetActiveScene().name;
        AsyncOperation async = SceneManager.UnloadSceneAsync(unloadSceneName);
        yield return new WaitUntil(() => async.isDone == true);

        //次のシーンロード
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        Scene scene = SceneManager.GetSceneByName(sceneName);
        yield return new WaitUntil(() => scene.isLoaded);
        Transition?.Open();
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));


    }
    public Coroutine ReloadScene()
    {
        return StartCoroutine(LoadSceneCorutine(SceneManager.GetActiveScene().name));
    }

    public virtual Coroutine LoadTittleScene()
    {
        return LoadScene(SceneName.TITTLE);
    }

    /// <summary>
    /// 次のステージへの遷移開始
    /// </summary>
    public abstract Coroutine NextScene();

}
