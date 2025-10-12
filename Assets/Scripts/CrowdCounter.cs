using UnityEngine;
using TMPro;

public class CrowdCounter : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("Label that displays the number of runners.")]
    private TMP_Text _text;

    [SerializeField, Tooltip("CrowdSystem to subscribe for count changes. If not set, you can use Runners Transform fallback.")]
    private CrowdSystem _crowd;

    [SerializeField, Tooltip("Fallback: parent of runners (used only if CrowdSystem is not provided).")]
    private Transform _runners;

    private int _last = int.MinValue;

    private void Reset()
    {
        if(!_text) _text = GetComponentInChildren<TMP_Text>(true);
        if(!_crowd) _crowd = FindObjectOfType<CrowdSystem>();
    }

    private void OnEnable()
    {
        if(_crowd)
        {
            _crowd.OnCountChanged.AddListener(HandleCountChanged);
            HandleCountChanged(_crowd.Count); // initial
        }
        else
        {
            // Fallback: do a one-time init; then only update when count changes (polled).
            RefreshFallback();
        }
    }

    private void OnDisable()
    {
        if(_crowd)
            _crowd.OnCountChanged.RemoveListener(HandleCountChanged);
    }

    private void Update()
    {
        // Only used in fallback mode (no CrowdSystem reference).
        if(_crowd) return;
        int current = _runners ? _runners.childCount : 0;
        if(current != _last) HandleCountChanged(current);
    }

    private void HandleCountChanged(int count)
    {
        if(_text && count != _last)
        {
            _text.text = count.ToString();
            _last = count;
        }
    }

    private void RefreshFallback()
    {
        int current = _runners ? _runners.childCount : 0;
        HandleCountChanged(current);
    }
}
