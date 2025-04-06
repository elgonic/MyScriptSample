using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Play時のFPS確認用クラス
/// </summary>
public class Fps : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fps;
    [SerializeField] private float _updateInterval = 0.5f;

    private WaitForSecondsRealtime _waitForSecondsRealtime;
    private void Reset()
    {
        _fps = GetComponent<TextMeshProUGUI>();
    }

    private IEnumerator Start()
    {
        _waitForSecondsRealtime = new WaitForSecondsRealtime(_updateInterval);
        while (true)
        {
            _fps.text = $"FPS:{(1f / Time.deltaTime).ToString("0.0")}";
            yield return _waitForSecondsRealtime;

        }
    }
}
