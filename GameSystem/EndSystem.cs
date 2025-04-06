using DesignPatterns.Singleton;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


/// <summary>
/// ゲームクリア画面
/// </summary>
public class EndSystem : SingletonMonobihaviourV2<EndSystem>
{
    public UnityEvent OnEnd;
    public AudioManager AudioManager => _audioManager;
    public EffectSpawner EffectSpawner => _effectSpawner;
    public SceneChangeBase SceneChanger => _sceneChanger;

    private CommonSceneManager _commonSceneManager;
    private SceneChangeBase _sceneChanger;
    private AudioManager _audioManager;
    private EffectSpawner _effectSpawner;
    private OutGameUIContoroller _outGameUIController;
    private CursorManger _cursorManger;
    private IEnumerator Start()
    {
        //共通シーン読み込み
        if (!_commonSceneManager)
        {
            yield return StartCoroutine(CommonSceneManager.LoadCommonScene());
        }

        Scene scene = SceneManager.GetSceneByName(SceneName.COMMON);
        _commonSceneManager = Array.Find(scene.GetRootGameObjects(), element => element.GetComponent<CommonSceneManager>()).GetComponent<CommonSceneManager>();
        _sceneChanger = _commonSceneManager.SceneChanger;
        _audioManager = _commonSceneManager.AudioManager;
        _effectSpawner = _commonSceneManager.EffectSpawner;
        _outGameUIController = _commonSceneManager.OutGameUIContoroller;
        _cursorManger = _commonSceneManager.CursorManger;

        _cursorManger.UI();
    }

    public void BackTittle()
    {
        OnEnd.Invoke();
        _sceneChanger.LoadTittleScene();
    }
}
