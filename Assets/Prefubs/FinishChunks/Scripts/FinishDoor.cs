using Core.Triggers;
using UnityEngine;
using UnityEngine.Events;

public class FinishDoor : MonoBehaviour
{
    [System.Serializable] public class ColliderEvent : UnityEvent<Collider> { }

    [Header("Trigger Source")]
    [SerializeField]private TriggerSource3D _trigger;

    [Header("Trigger Events")]
    public ColliderEvent OnEntered = new ColliderEvent();
    public ColliderEvent OnExited = new ColliderEvent();
    public ColliderEvent OnStayed = new ColliderEvent();

    void Reset() { if(!_trigger) _trigger = GetComponentInChildren<TriggerSource3D>(true); }
    void Awake() { if(!_trigger) _trigger = GetComponentInChildren<TriggerSource3D>(true); }

    void OnEnable()
    {
        if(_trigger == null) return;
        _trigger.onEnter.AddListener(ForwardEnter);
        _trigger.onExit.AddListener(ForwardExit);
        _trigger.onStay.AddListener(ForwardStay);
    }

    void OnDisable()
    {
        if(_trigger == null) return;
        _trigger.onEnter.RemoveListener(ForwardEnter);
        _trigger.onExit.RemoveListener(ForwardExit);
        _trigger.onStay.RemoveListener(ForwardStay);
    }

    void ForwardEnter(Collider who) => OnEntered.Invoke(who);
    void ForwardExit(Collider who) => OnExited.Invoke(who);
    void ForwardStay(Collider who) => OnStayed.Invoke(who);
}