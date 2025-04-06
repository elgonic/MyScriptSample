using DesignPatterns.Singleton;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


/// <summary>
/// アクションステージのシステム
/// </summary>
[DefaultExecutionOrder(-1)]
public class MainGameSystem : SingletonMonobihaviourV2<MainGameSystem>
{
    public enum GameStatus
    {
        Tutorial,
        Action,
        Boss
    }

    [SerializeField, ReadOnly] private CharacterParam _bossParams = null;


    [SerializeField] private bool _loopThisStage = false;

    [SerializeField, ReadOnly] private GameStatus _status;

    public bool IsClearStage { get; private set; } = false;


    private CharacterParam _playerCharacterParams;


    private Transform _startTransform;
    private Goal _goal;



    private bool _isPause = false;

    public bool IsPause => _isPause;

    public GameStatus Status => _status;
    public CharacterParam Player => _playerCharacterParams;
    public CharacterParam Boss
    {
        get
        {
            if (_bossParams == null)
            {
                //Enemyが見つからなければ null
                _bossParams = FindFirstObjectByType<Enemy>()?.CharacterParam;
            }
            return _bossParams;
        }
    }

    private BossFieldInfo _bossFieldInfo;
    public BossFieldInfo BossFieldInfo
    {
        get
        {
            if (_bossFieldInfo == null)
            {
                _bossFieldInfo = GameObject.FindObjectOfType<BossFieldInfo>();
            }
            return _bossFieldInfo;
        }

    }

    public Goal Goal
    {
        get
        {
            if (_goal == null)
            {
                _goal = GameObject.FindFirstObjectByType<Goal>();
                if (!_goal)
                {
                    Debugger.LogError($"{typeof(Goal)}: Sceneに存在していません");
                    return null;
                }
            }
            return _goal;
        }
    }





    //CommonSceneから取得系
    private CommonSceneManager _commonSenceManager;
    private SceneChangeBase _sceneChanger;
    private AudioManager _audioManager;
    private EffectSpawner _effectSpawner;
    private OutGameUIContoroller _outGameUIContoroller;
    private CursorManger _cursorManger;
    public AudioManager AudioManager => _audioManager;
    public EffectSpawner EffectSpawner => _effectSpawner;
    public OutGameUIContoroller OutGameUIContoroller => _outGameUIContoroller;
    public CursorManger CursorManger => _cursorManger;



    //イベント
    public UnityEvent OnGameStart = new();
    public UnityEvent OnMainStageEntry = new();
    public UnityEvent OnStageClear = new();
    public UnityEvent OnPlayerRespwn = new();
    public UnityEvent OnGameOver = new();
    public UnityEvent OnChangeNextStage = new();
    public UnityEvent OnOpenPause = new();
    public UnityEvent OnClosePause = new();
    public UnityEvent OnBacKTittle = new();


    private void Reset()
    {
        _playerCharacterParams = GameObject.FindFirstObjectByType<Player>().GetComponent<CharacterParam>();
    }

    public override void Awake()
    {
        base.Awake();
        _playerCharacterParams = GameObject.FindFirstObjectByType<Player>().GetComponent<CharacterParam>();
        if (_playerCharacterParams == null)
        {
            Debugger.LogError($"PlayerがSceneに存在しません");
            return;
        }

        //Player初期位置設定
        _startTransform = GameObject.FindWithTag(StaticCommonParams.STARTPOSITION_TAG).transform;
        if (!_startTransform) Debugger.LogError($"{gameObject.name} :{GetType().Name} : StartPositionがScneneにありません!");
        _playerCharacterParams.transform.position = _startTransform.position;
        OnMainStageEntry.AddListener(() => _playerCharacterParams.Hp.Reset());

        IsClearStage = false;

    }
    private IEnumerator Start()
    {
        yield return StartCoroutine(CommmonSeneceSetting());

        Time.timeScale = 1.0f;

        GamseStageSetting();

        GameStart();
        yield return null;
    }

    private void OnDisable()
    {
        //CommmonSceneへ登録したイベントの解除
        _outGameUIContoroller.PauseUI.ContinueButton.onClick.RemoveListener(Pause);
        _outGameUIContoroller.PauseUI.BackTittleButton.onClick.RemoveListener(BackTittle);
        _outGameUIContoroller.PauseUI.ReStartButton.onClick.RemoveListener(ReStartStage);

    }


    private IEnumerator CommmonSeneceSetting()
    {
        //共通シーン読み込み
        if (!_commonSenceManager)
        {
            yield return StartCoroutine(CommonSceneManager.LoadCommonScene());
        }
        Scene scene = SceneManager.GetSceneByName(SceneName.COMMON);
        _commonSenceManager = Array.Find(scene.GetRootGameObjects(), element => element.GetComponent<CommonSceneManager>()).GetComponent<CommonSceneManager>();
        _sceneChanger = _commonSenceManager.SceneChanger;
        _audioManager = _commonSenceManager.AudioManager;
        _effectSpawner = _commonSenceManager.EffectSpawner;
        _outGameUIContoroller = _commonSenceManager.OutGameUIContoroller;
        _cursorManger = _commonSenceManager.CursorManger;

        //UI関係のセッティング
        OnGameStart.AddListener(_cursorManger.InGame);
        OnOpenPause.AddListener(_cursorManger.UI);
        OnClosePause.AddListener(_cursorManger.InGame);
        OnOpenPause.AddListener(_outGameUIContoroller.PauseUI.Open);
        OnClosePause.AddListener(_outGameUIContoroller.AllUIClose);




        _outGameUIContoroller.PauseUI.ContinueButton.onClick.AddListener(Pause);
        _outGameUIContoroller.PauseUI.BackTittleButton.onClick.AddListener(BackTittle);
        _outGameUIContoroller.PauseUI.ReStartButton.onClick.AddListener(ReStartStage);

    }


    private void GamseStageSetting()
    {



        //ゴール設定
        if (Goal != null)
        {
            Goal.OnGoal.AddListener(ChangeNextStage);
            OnStageClear.AddListener(() => Goal.Ebable());

            //最初は非表示
            Goal.Disable();
        }

        //Bossがいるなら
        if (_bossParams && _bossParams.gameObject.activeSelf)
        {
            _bossParams.Hp.OnDeath.AddListener(StageClear);
        }
        //BossがいないならMainStage入場時にステージクリア
        else
        {
            OnMainStageEntry.AddListener(StageClear);
        }


    }


    private void GameStart()
    {
        _status = GameStatus.Tutorial;
        Debugger.Log("GameStart");
        OnGameStart?.Invoke();
    }

    private void ReStartStage()
    {
        //ステージで使用したエフェクトの削除
        _audioManager.RemovePooled();

        _effectSpawner.RemovePooled();

        _sceneChanger.ReloadScene();
    }

    /// <summary>
    /// PlayerがRespownした時に呼ばれる
    /// </summary>
    public void PlayerRespown()
    {
        _status = GameStatus.Tutorial;

        OnPlayerRespwn?.Invoke();
    }
    public void MainStageEntry()
    {
        _status = GameStatus.Boss;
        Debugger.Log("MainStageEntry");
        OnMainStageEntry?.Invoke();
    }

    public void StageClear()
    {
        Debugger.Log("StageClear");
        IsClearStage = true;
        OnStageClear?.Invoke();
    }
    public void GameOver()
    {
        Debugger.Log("GameOver");
        OnGameOver?.Invoke();
    }

    public void ChangeNextStage()
    {
        Debugger.Log("ChangeNextStage");
        OnChangeNextStage?.Invoke();
        //先にコルーチンハンドラーを止めておかないと , コルーチンがScsneChange時に先に破棄されたObjectにアクセスしようとしてエラーになる
        CoroutineHandler.StopAllCorutine();

        //ステージで使用したエフェクトの削除
        _audioManager.RemovePooled();
        _effectSpawner.RemovePooled();


        if (_loopThisStage) _sceneChanger.ReloadScene();
        else _sceneChanger.NextScene();
    }


    private float timeScale;
    public void Pause()
    {
        if (_isPause == false)
        {
            OnOpenPause?.Invoke();
            _isPause = true;
            Debugger.Log("Pause");
            timeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            OnClosePause?.Invoke();
            _isPause = false;
            Debugger.Log("End Pause");
            Time.timeScale = timeScale;
        }
    }


    public void BackTittle()
    {
        Debugger.Log("Back Tittle");
        OnBacKTittle?.Invoke();
        _sceneChanger.LoadTittleScene();
    }




}
