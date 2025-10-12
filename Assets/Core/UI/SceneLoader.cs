using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField, Tooltip("CanvasGroup used for screen fade.")]
    private CanvasGroup _fade;

    [SerializeField, Tooltip("Seconds for fade in/out.")]
    private float _fadeTime = 0.25f;

    private bool _busy;

    private void Awake()
    {
        if(_fade)
        {
            _fade.alpha = 1f;
            StartCoroutine(Fade(0f));
        }
    }

    public void LoadScene(string sceneName)
    {
        if(_busy) return;
        StartCoroutine(LoadRoutine(sceneName));
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        _busy = true;
        if(_fade) yield return Fade(1f);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while(!op.isDone) yield return null;

        _busy = false;
    }

    private IEnumerator Fade(float target)
    {
        if(!_fade) yield break;
        float start = _fade.alpha;
        float t = 0f;
        while(t < _fadeTime)
        {
            t += Time.unscaledDeltaTime;
            _fade.alpha = Mathf.Lerp(start, target, t / _fadeTime);
            yield return null;
        }
        _fade.alpha = target;
    }
}
