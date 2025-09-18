using UnityEngine;
using TMPro;

//TODO: Make text changes only when Runners amount changes
public class CrowdCounter : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private Transform _runners;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _text.text = _runners.childCount.ToString();
    }

    private void OnDestroy()
    {
        
    }
}
