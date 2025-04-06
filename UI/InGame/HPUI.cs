using LitMotion;
using LitMotion.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// HPのUI
/// </summary>
public class HPUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Image _hpIcon;
    [SerializeField] private HpComp _targetHp;
    [SerializeField] private Ease _dispAnimationType = Ease.InBack;
    [SerializeField] private float _dispAnimationTime = 0.3f;
    [SerializeField] private Ease _hideAnimationType = Ease.InBack;
    [SerializeField] private float _hideAnimationTime = 0.3f;
    [SerializeField] private Ease _idleAnimationType = Ease.InBack;
    [SerializeField] private float _idleAnimationTime = 0.3f;
    [SerializeField] private float _idleAnimationScaleCoefficient = 0.8f;
    [SerializeField] private float _idleAnimationDurationTime = 3f;
    [SerializeField] private List<Image> _hpIcons = new();



    private Vector3? baseScale = null;

    private void Start()
    {
        //panel自身を除いた検索
        _hpIcons = _panel.GetComponentsInChildren<Image>().Where(element => element.gameObject != _panel.gameObject).ToList();

        if (_targetHp)
        {
            _targetHp.OnChange.AddListener(OnChange);
            OnChange();
        }

    }

    private void OnEnable()
    {
        StartCoroutine(OnEnableCoroutine());
    }

    private IEnumerator OnEnableCoroutine()
    {
        foreach (var icon in _hpIcons)
        {
            LMotion.Create(Vector3.zero, _hpIcon.rectTransform.localScale, _dispAnimationTime).WithEase(_dispAnimationType).BindToLocalScale(icon.rectTransform).AddTo(icon);
        }
        yield return StaticCommonParams.Yielders.Get(_dispAnimationTime);
        StartCoroutine(IdleIconCoroutine());
    }



    private IEnumerator IdleIconCoroutine()
    {
        while (true)
        {
            if (_hpIcons.Count <= 0)
            {
                yield return null;
                continue;
            }
            foreach (var icon in _hpIcons)
            {
                StartCoroutine(IdleIconAnimation(icon));
            }
            yield return StaticCommonParams.Yielders.Get(_idleAnimationDurationTime);
        }
    }


    private IEnumerator IdleIconAnimation(Image icon)
    {
        if (!baseScale.HasValue) baseScale = icon.transform.localScale;
        yield return LMotion.Create(icon.transform.localScale, baseScale.Value * _idleAnimationScaleCoefficient, _idleAnimationTime / 2).WithEase(_idleAnimationType).BindToLocalScale(icon.rectTransform).AddTo(icon).ToYieldInteraction();
        yield return LMotion.Create(icon.transform.localScale, baseScale.Value, _idleAnimationTime / 2).WithEase(_idleAnimationType).BindToLocalScale(icon.rectTransform).AddTo(icon).ToYieldInteraction();
    }


    public void SetTarget(HpComp target = null)
    {
        //前ターゲットからイベント削除
        if (_targetHp)
        {
            _targetHp.OnChange.RemoveListener(OnChange);
        }
        // target が nullじゃなければ HPUI 表示 
        if (target)
        {
            _targetHp = target;
            _targetHp.OnChange.AddListener(OnChange);
            OnChange();
        }
        else
        {
            foreach (Image image in _hpIcons) Destroy(image.gameObject);
        }
    }
    private void OnChange()
    {
        if (!gameObject.activeSelf) return;

        int hpDiff = _targetHp.Hp - _hpIcons.Count;

        if (hpDiff == 0) return;

        if (hpDiff > 0)
        {
            for (int i = 0; i < hpDiff; i++)
            {
                _hpIcons.Add(AddImage());
            }
        }
        if (hpDiff < 0)
        {
            for (int i = 0; i < -hpDiff; i++)
            {
                Image deleteImage = _hpIcons.Last();
                _hpIcons.RemoveAt(_hpIcons.Count - 1);
                DeleteImage(deleteImage);
            }
        }
    }





    private Image AddImage()
    {

        Image image = Instantiate(_hpIcon, _panel.transform);
        Vector3 targetScale = image.rectTransform.localScale;
        LMotion.Create(Vector3.zero, targetScale, _dispAnimationTime).WithEase(_dispAnimationType).BindToLocalScale(image.rectTransform);
        return image;
    }

    private void DeleteImage(Image deleteImage)
    {
        Vector3 targetScale = deleteImage.rectTransform.localScale;
        LMotion.Create(targetScale, Vector3.zero, _hideAnimationTime).WithEase(_hideAnimationType).WithOnComplete(() => Destroy(deleteImage.gameObject)).BindToLocalScale(deleteImage.rectTransform);
    }
}
