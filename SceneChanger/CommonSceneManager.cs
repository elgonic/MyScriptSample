using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// 共通シーン管理クラス
/// </summary>
public class CommonSceneManager : MonoBehaviour
{
    private SceneChangeBase _sceneChanger;
    public SceneChangeBase SceneChanger
    {
        get
        {
            if (!_sceneChanger) _sceneChanger = GetComponentInChildren<SceneChangeBase>();
            if (!_sceneChanger) Debugger.LogWarning($"{gameObject.scene.name}:{typeof(SceneChangeBase).Name}が{gameObject.name}の子要素に存在しません");
            return _sceneChanger;
        }
    }

    private EffectSpawner _effectSpawner;
    public EffectSpawner EffectSpawner
    {
        get
        {
            if (!_effectSpawner) _effectSpawner = GetComponentInChildren<EffectSpawner>();
            if (!_effectSpawner) Debugger.LogWarning($"{gameObject.scene.name}:{typeof(EffectSpawner).Name}が{gameObject.name}の子要素に存在しません");
            return _effectSpawner;
        }
    }


    private AudioManager _audioManager;
    public AudioManager AudioManager
    {
        get
        {
            if (!_audioManager) _audioManager = GetComponentInChildren<AudioManager>();
            if (!_audioManager) Debugger.LogWarning($"{gameObject.scene.name}:{typeof(AudioManager).Name}が{gameObject.name}の子要素に存在しません");
            return _audioManager;
        }
    }

    private OutGameUIContoroller _outGameUIContoroller;
    public OutGameUIContoroller OutGameUIContoroller
    {
        get
        {
            if (!_outGameUIContoroller) _outGameUIContoroller = GetComponentInChildren<OutGameUIContoroller>();
            if (!_outGameUIContoroller) Debugger.LogWarning($"{gameObject.scene.name}:{typeof(OutGameUIContoroller).Name}が{gameObject.name}の子要素に存在しません");
            return _outGameUIContoroller;

        }
    }

    private CursorManger _cursorManager;
    public CursorManger CursorManger
    {
        get
        {
            if (!_cursorManager) _cursorManager = GetComponentInChildren<CursorManger>();
            if (!_cursorManager) Debugger.LogWarning($"{gameObject.scene.name}:{typeof(CursorManger).Name}が{gameObject.name}の子要素に存在しません");
            return _cursorManager;

        }
    }
    /// <summary>
    /// 共通シーンの読み込み処理
    /// </summary>
    public static IEnumerator LoadCommonScene()
    {

        Scene scene = SceneManager.GetSceneByName(SceneName.COMMON);
        //重複ロード防止
        if (!scene.IsValid())
        {
            SceneManager.LoadScene(SceneName.COMMON, LoadSceneMode.Additive);
            scene = SceneManager.GetSceneByName(SceneName.COMMON);

        }
        if (scene.IsValid())
        {
            //ロードされるまで待つ
            yield return new WaitUntil(() => scene.isLoaded == true);
        }
    }

    private void Awake()
    {
        OutGameUIContoroller.InputDeviceConfig.LoadConfig();
    }
}
