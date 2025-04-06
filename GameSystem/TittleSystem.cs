using DesignPatterns.Singleton;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトル
/// </summary>
public class TittleSystem : SingletonMonobihaviourV2<TittleSystem>
{

    public UnityEvent OnStart;


    private TittleUI _tittleUI;

    //Commonから取得
    private CommonSceneManager _commonSceneManager;
    private SceneChangeBase _sceneChanger;
    private AudioManager _audioManager;
    private EffectSpawner _effectSpawner;
    private OutGameUIContoroller _outGameUIController;
    private CursorManger _cursorManger;


    public AudioManager AudioManager => _audioManager;
    public EffectSpawner EffectSpawner => _effectSpawner;
    public OutGameUIContoroller OutGameUIController => _outGameUIController;

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

        //カーソル設定
        _cursorManger.UI();

        //MainGameから移動したときは TimeScale  = 0の可能性があるので
        Time.timeScale = 1.0f;

        //TittleUI
        _tittleUI = FindAnyObjectByType<TittleUI>();
        _tittleUI?.Setup();
        _outGameUIController.ConfigUI.Back.onClick.AddListener(_tittleUI.Open);

        //効果音
        AudioSource bgm = AudioManager.Instance.PlayAnyAudio(AudioManager.Instance.Bgm);
        OnStart.AddListener(() => AudioManager.Instance.Stop(bgm));



    }
    public void GameStart()
    {
        OnStart?.Invoke();
        _sceneChanger.NextScene();
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }

}
