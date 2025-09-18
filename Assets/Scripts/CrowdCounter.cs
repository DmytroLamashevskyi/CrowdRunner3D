using UnityEngine;
using TMPro;

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
