using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Player
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("External Systems")]
        [SerializeField, Tooltip("CrowdSystem that manages runner instances.")]
        private CrowdSystem _crowd;

        [Header("Runtime Stats (Read-Only)")]
        [SerializeField, ReadOnly, Tooltip("How many doors have been used so far.")]
        private int _doorsUsed;

        [SerializeField, ReadOnly, Tooltip("Last received door value.")]
        private int _lastValue;

        [SerializeField, ReadOnly, Tooltip("Last received bonus type.")]
        private BonusTypes _lastType;

        [Header("Events")]
        [Tooltip("Raised after a door choice is successfully applied.")]
        public UnityEvent OnAfterDoorChoice;

        public int DoorsUsed => _doorsUsed;
        public int RunnerCount => _crowd ? _crowd.Count : 0;

        /// <summary>Apply a door choice: routes arithmetic bonuses to the CrowdSystem.</summary>
        public void ApplyDoorChoice(DoorChunk source, bool rightSide, int value, BonusTypes type)
        {
            switch(type)
            {
                case BonusTypes.Addition:
                case BonusTypes.Difference:
                case BonusTypes.Multiplication:
                case BonusTypes.Division:
                    if(_crowd) _crowd.ApplyBonus(type, value);
                    else Debug.LogWarning("[PlayerStats] CrowdSystem is missing; cannot apply crowd bonus.", this);
                    break;

                default:
                    // Extend here for other types later (Health, Damage, Speed, etc.)
                    Debug.Log($"[PlayerStats] Unhandled bonus type: {type} (value={value}).", this);
                    break;
            }

            _doorsUsed++;
            _lastValue = value;
            _lastType = type;

            OnAfterDoorChoice?.Invoke();
        }

        // Optional helper if you want to set it from code
        public void SetCrowd(CrowdSystem crowd) => _crowd = crowd;
    }
}
