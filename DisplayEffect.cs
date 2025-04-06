using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


/// <summary>
/// 画面効果
/// </summary>
public class DisplayEffect : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float _lensDistractionIntesity = -1f;
    [SerializeField] private float _changeTime;
    [SerializeField, NormalizedAnimationCurve] private AnimationCurve _changeCurve;


    private Volume __volume;
    private Volume _volume
    {
        get
        {
            if (__volume == null) __volume = GameObject.FindFirstObjectByType<Volume>();
            return __volume;
        }
    }

    private LensDistortion _lensDistortion;
    private Player _player;

    private void Start()
    {
        _player = MainGameSystem.Instance.Player.GetComponent<Player>();

        _volume.profile.TryGet(out _lensDistortion);
        _player?.OnParryBegin.AddListener(OnParry);
        _player?.OnParryEnd.AddListener(ResetParam);
    }

    private void OnParry(Vector3 position)
    {
        StartCoroutine(OnParryCorutine(_lensDistractionIntesity, _changeTime, _changeCurve));
    }


    private void ResetParam()
    {

        StartCoroutine(OnParryCorutine(0, _changeTime, _changeCurve));
    }

    private IEnumerator OnParryCorutine(float tagetValue, float changeTIme, AnimationCurve changeAnimationCurve)
    {
        float startTime = Time.realtimeSinceStartup;
        float firstValue = _lensDistortion.intensity.value;
        while (Time.realtimeSinceStartup - startTime <= _changeTime)
        {
            _lensDistortion.intensity.value = firstValue + (tagetValue - firstValue) * _changeCurve.Evaluate((Time.realtimeSinceStartup - startTime) / _changeTime);
            yield return null;
        }
        _lensDistortion.intensity.value = tagetValue;
    }
}
