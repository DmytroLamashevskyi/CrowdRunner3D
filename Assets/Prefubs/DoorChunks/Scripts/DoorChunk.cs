using Core.Triggers;
using DoorChunks.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class DoorChunk : MonoBehaviour
{
    [Header("Config (ScriptableObject)")]
    [SerializeField, Tooltip("Door pair configuration (left/right values and types).")]
    private DoorPairConfig _config;

    [Header("Child References")]
    [SerializeField, Tooltip("Visual controller for both doors.")]
    private DoorsView _doorsView;

    [SerializeField, Tooltip("Trigger zone that detects entering objects (e.g., Player).")]
    private TriggerSource3D _triggerZone;

    [SerializeField, Tooltip("Collider that blocks/permits passing through the doors.")]
    private Collider _doorsCollider;

    [Header("Trigger Settings")]
    [SerializeField, Tooltip("Tag required to trigger the door (usually 'Player').")]
    private string _requiredTag = "Player";

    [SerializeField, Tooltip("Allow only one choice; disables collider after use.")]
    private bool _oneShot = true;

    [SerializeField, Tooltip("If one-shot is disabled, seconds before the door re-arms (0 = immediately).")]
    private float _rearmDelay = 0f;

    [System.Serializable]
    public class ChosenEvent : UnityEvent<int, BonusTypes> { }

    [Header("Events")]
    [Tooltip("Invoked when a door is chosen: (value, type).")]
    public ChosenEvent OnDoorChosen;

    [Tooltip("Invoked after any door is used.")]
    public UnityEvent OnDoorUsed;

    private bool _used;

    private void Reset()
    {
        _doorsView = GetComponentInChildren<DoorsView>(true);
        _triggerZone = GetComponentInChildren<TriggerSource3D>(true);
        _doorsCollider = GetComponentInChildren<Collider>(true);
    }

    private void Awake()
    {
        if(!_doorsView) _doorsView = GetComponentInChildren<DoorsView>(true);
        if(!_triggerZone) _triggerZone = GetComponentInChildren<TriggerSource3D>(true);
        if(!_doorsCollider) _doorsCollider = GetComponentInChildren<Collider>(true);
    }

    private void OnEnable()
    {
        if(_triggerZone) _triggerZone.onEnter.AddListener(HandleEnter);
        _used = false;
        if(_doorsCollider) _doorsCollider.enabled = true;
        ApplyConfig();
    }

    private void OnDisable()
    {
        if(_triggerZone) _triggerZone.onEnter.RemoveListener(HandleEnter);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(!Application.isPlaying) ApplyConfig();
    }
#endif

    public void SetConfig(DoorPairConfig cfg)
    {
        _config = cfg;
        ApplyConfig();
    }

    private void ApplyConfig()
    {
        if(_config && _doorsView) _doorsView.Apply(_config);
    }

    private void HandleEnter(Collider other)
    {
        if(_oneShot && _used) return;
        if(!other || !other.CompareTag(_requiredTag)) return;
        if(!_config) return;

        bool right = transform.InverseTransformPoint(other.transform.position).x > 0f;
        int value = right ? _config.right.value : _config.left.value;
        var type = right ? _config.right.type : _config.left.type;

        var consumer = other.GetComponentInParent<IConsumeDoorChoice>();
        if(consumer == null) return;

        consumer.OnDoorChoice(this, right, value, type);

        OnDoorChosen?.Invoke(value, type);
        OnDoorUsed?.Invoke();

        _used = true;
        if(_doorsCollider) _doorsCollider.enabled = false;

        if(!_oneShot)
        {
            if(_rearmDelay <= 0f) Rearm();
            else
            {
                StopAllCoroutines();
                StartCoroutine(RearmAfter(_rearmDelay));
            }
        }
    }

    private IEnumerator RearmAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Rearm();
    }

    [ContextMenu("Rearm Now")]
    public void Rearm()
    {
        _used = false;
        if(_doorsCollider) _doorsCollider.enabled = true;
    }
}
