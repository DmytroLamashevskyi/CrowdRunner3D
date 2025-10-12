using UnityEngine;
using UnityEngine.Events;

namespace Core.Triggers
{


    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class TriggerSource3D : MonoBehaviour
    {
        [System.Serializable] public class ColliderEvent : UnityEvent<Collider> { }

        [Header("Layer Mask")]
        [SerializeField] private LayerMask _layerMask = ~0;

        public ColliderEvent onEnter = new ColliderEvent();
        public ColliderEvent onExit = new ColliderEvent();
        public ColliderEvent onStay = new ColliderEvent();

        void Reset()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        bool Pass(Collider other) => (_layerMask & (1 << other.gameObject.layer)) != 0;

        void OnTriggerEnter(Collider other) { if(Pass(other)) onEnter.Invoke(other); }
        void OnTriggerExit(Collider other) { if(Pass(other)) onExit.Invoke(other); }
        void OnTriggerStay(Collider other) { if(Pass(other)) onStay.Invoke(other); }
    }
}