using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


/// <summary>
/// 映像を使用した場面遷移
/// </summary>
[RequireComponent(typeof(VideoPlayer))]
public class MovieTransitioin : MonoBehaviour, ITransition
{
    [SerializeField] private VideoClip _hideVideo;
    [SerializeField] private VideoClip _openVideo;
    [SerializeField] private AudioSource _hideSE;
    [SerializeField] private AudioSource _openSE;

    [SerializeField] private RawImage _targetImage;

    [SerializeField] private bool _test;

    private VideoPlayer __videoPlayer;
    private VideoPlayer _videoPlayer
    {
        get
        {
            if (!__videoPlayer) __videoPlayer = GetComponent<VideoPlayer>();
            return __videoPlayer;
        }
    }
    private void OnValidate()
    {
        if (_test)
        {
            _test = false;
            StartCoroutine(Test());
        }
    }

    private void Start()
    {

        //初期状態では1回目はRenderTextureのせいで暗くなるので
        _targetImage.enabled = false;

    }

    public Coroutine Hide()
    {

        _videoPlayer.clip = _hideVideo;
        AudioManager.Instance.UniqePooledSEPlay(_hideSE);
        return StartCoroutine(Play(_videoPlayer));

    }

    private IEnumerator HideCoroutine()
    {
        yield return null;
    }

    public Coroutine Open()
    {
        return StartCoroutine(OpenCoroutine());
    }

    private IEnumerator OpenCoroutine()
    {
        _videoPlayer.clip = _openVideo;
        AudioManager.Instance.UniqePooledSEPlay(_openSE);
        yield return StartCoroutine(Play(_videoPlayer));
        _targetImage.enabled = false;

    }

    private IEnumerator Play(VideoPlayer videoPlayer)
    {
        //ここでPrepareして準備しないと Playしても isPlayが trueにならない(https://docs.unity3d.com/ja/2020.3/ScriptReference/Video.VideoPlayer-isPlaying.html)
        videoPlayer.enabled = true;
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);
        //再生時の一瞬の黒画面防止
        yield return new WaitForSecondsRealtime(0.3f);
        videoPlayer.Play();

        _targetImage.enabled = true;

        yield return new WaitUntil(() => videoPlayer.isPlaying == false);
        videoPlayer.enabled = false;
    }
    private IEnumerator Test()
    {
        Hide();
        yield return new WaitForSeconds(2f);
        Open();
    }
}
