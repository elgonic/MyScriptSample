using LitMotion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// お知らせUIの管理クラス
/// </summary>

[RequireComponent(typeof(CanvasGroup))]
public class AnnouncementUIManager : MonoBehaviour
{

    public enum AnnouncementType
    {
        NONE = 0,
        STAGECLEAR = 1,
        MOVE = 2,
        JUMP = 3,
        DASH = 4,
        PARRY = 5,
        LOCK = 6,
        UNLOCK = 7,
        BULLET = 9,
        DASHENEMY = 8,
        LAZER_TUTORIAL = 10,
        LAZER = 11,
        EntryStage = 12
    }


    [SerializeField] private Image _backPanel;
    [SerializeField] private GameObject _hideUI;
    [SerializeField] private GameObject _openUI;
    [SerializeField, NormalizedAnimationCurve(false, true)] private AnimationCurve _animationCureve;
    [SerializeField] private float _openAnimationTime = 1f;
    [SerializeField] private float _closeAnimationTime = 1f;
    [SerializeField] private List<AnnouncementUI> _uiList;


    /// <summary>
    /// 現在開いているタイプの保存用
    /// </summary>
    private AnnouncementType _cacheType = AnnouncementType.NONE;

    private RectTransform _targetRectTransform;
    private CanvasGroup _canvasGroup;

    [Header("Test")]
    [SerializeField] private AnnouncementType _testAnnouncementType;
    [SerializeField] private bool _open = false;
    [SerializeField] private bool _close = false;
    private void OnValidate()
    {
        if (_open)
        {
            _open = false;
            Open(_testAnnouncementType);
        }
        if (_close)
        {
            _close = false;
            Close();
        }
    }
    private void Awake()
    {
        _targetRectTransform = _backPanel.GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 0f;
        _backPanel.gameObject.SetActive(false);

        _uiList = GetComponentsInChildren<AnnouncementUI>(true).ToList();

        foreach (AnnouncementType type in Enum.GetValues(typeof(AnnouncementType)))
        {
            AnnouncementUI[] typrUIs = _uiList.FindAll(element => element.Type == type).ToArray();
            if (typrUIs.Length == 0) Debugger.LogWarning($"{gameObject.name}:{GetType().Name}:{type.ToString()} のUIが設定されていません");
            else if (typrUIs.Length != 1) Debugger.LogError($"{gameObject.name}:{GetType().Name}:{type.ToString()} のUIが複数子要素に存在します.1つにしてください");
            foreach (AnnouncementUI ui in typrUIs)
            {
                ui.gameObject.SetActive(false);
            }
        }

        //Uiが一度表示されるまでは Open UIは非表示でいい
        _openUI.gameObject.SetActive(false);


        //設定処理
        AnnouncementTriggerManager announcementTriggerManager = GameObject.FindObjectOfType<AnnouncementTriggerManager>();
        if (!announcementTriggerManager)
        {

            Debugger.Log($"Scene内に{typeof(AnnouncementTriggerManager).Name}が存在しません。{typeof(AnnouncementUI).Name}を使用しないならOK!");
            return;
        }

        announcementTriggerManager.OnEnterPlayer.AddListener(() => Open(announcementTriggerManager.Type));

        //StageClearした時の共通通知
        MainGameSystem.Instance.OnStageClear.AddListener(() => { Open(AnnouncementType.STAGECLEAR); });

    }

    public Coroutine Toggle()
    {
        Debugger.Log($"{nameof(Toggle)} : {_backPanel.enabled} : {_backPanel.gameObject.activeSelf}");
        if (_backPanel.gameObject.activeSelf) return Close();
        return Open(_cacheType, false);

    }
    public Coroutine Open(AnnouncementType type, bool isNext = true)
    {

        _openUI.gameObject.SetActive(false);

        _cacheType = type;


        AnnouncementUI targetUI = _uiList.Find(element => element.Type == type);
        if (targetUI)
        {
            //ターゲット以外を非表示
            foreach (AnnouncementUI ui in _uiList)
            {
                if (ui == targetUI) continue;
                ui.gameObject.SetActive(false);
            }

            //次のアナウンスに進む場合
            if (isNext)
            {
                bool result = targetUI.Next();
                if (!result)
                {
                    Close();
                }
            }

            //重複Open防止処理 
            if (targetUI.gameObject.activeInHierarchy) return null;
            targetUI.gameObject.SetActive(true);

            //BackpanaelUIが非表示ならばOpen表示処理を1度だけ
            if (!_backPanel.gameObject.activeSelf) return StartCoroutine(OpenCoroutine());
        }
        return null;
    }

    private IEnumerator OpenCoroutine()
    {
        _canvasGroup.alpha = 0f;
        _backPanel.gameObject.SetActive(true);
        yield return LMotion.Create(_canvasGroup.alpha, 1, _openAnimationTime).WithEase(_animationCureve).Bind(x => _canvasGroup.alpha = x).ToYieldInteraction();
    }
    public Coroutine Close()
    {
        return StartCoroutine(CloseCoroutine());
    }

    public IEnumerator CloseCoroutine()
    {
        yield return LMotion.Create(_canvasGroup.alpha, 0, _closeAnimationTime).WithEase(Ease.OutQuint).Bind(x => _canvasGroup.alpha = x).ToYieldInteraction();
        _backPanel.gameObject.SetActive(false);
        _openUI.gameObject.SetActive(true);
    }
}
