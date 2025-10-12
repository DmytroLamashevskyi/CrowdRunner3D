using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class CrowdSystem : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField, Tooltip("Parent transform that holds all runner instances.")]
    private Transform _runnersParent;

    [FormerlySerializedAs("_runnerPrefub")]
    [SerializeField, Tooltip("Prefab to instantiate for a runner.")]
    private GameObject _runnerPrefab;

    [Header("Layout Settings")]
    [SerializeField, Tooltip("Base spacing for the Vogel spiral (affects density).")]
    private float _radius = 2f;

    [SerializeField, Tooltip("Angle step in degrees. 137.508 gives a 'sunflower' pattern.")]
    private float _angle = 137.508f;

    [SerializeField, Tooltip("If true, re-layout every frame (debug). Otherwise only when needed.")]
    private bool _layoutOnUpdate = false;

    [Header("Limits")]
    [SerializeField, Tooltip("Max allowed world-space radius of the crowd. Spiral compresses when exceeded. Set to: roadWidth/2 - margin.")]
    private float _maxWorldRadius = 4f;

    [SerializeField, Tooltip("Optional hard cap for runner count (0 = unlimited).")]
    private int _maxRunners = 0;

    [Header("Events")]
    [Tooltip("Invoked when the runners count changes.")]
    public UnityEvent<int> OnCountChanged;

    private bool _layoutDirty = true;
    private float _angleRad;

    public int Count => _runnersParent ? _runnersParent.childCount : 0;

    private void Awake()
    {
        _angleRad = _angle * Mathf.Deg2Rad;
    }

    private void OnEnable()
    {
        _layoutDirty = true;
        NotifyCount();
    }

    private void OnValidate()
    {
        _angleRad = _angle * Mathf.Deg2Rad;
        _layoutDirty = true;
        _maxWorldRadius = Mathf.Max(0f, _maxWorldRadius);
        _maxRunners = Mathf.Max(0, _maxRunners);
    }

    private void Update()
    {
        if(_layoutOnUpdate)
        {
            PlaceRunners();
        }
        else if(_layoutDirty)
        {
            PlaceRunners();
            _layoutDirty = false;
        }
    }

    // Auto-called when children added/removed if this component is on the same GO as _runnersParent
    private void OnTransformChildrenChanged()
    {
        _layoutDirty = true;
        NotifyCount();
    }

    public void MarkLayoutDirty() => _layoutDirty = true;

    private void PlaceRunners()
    {
        if(!_runnersParent) return;

        int n = _runnersParent.childCount;
        float spacing = GetEffectiveSpacing(); // compressed spacing when needed

        for(int i = 0; i < n; i++)
        {
            _runnersParent.GetChild(i).localPosition = GetRunnerPosition(i, spacing);
        }
    }

    private Vector3 GetRunnerPosition(int index, float spacing)
    {
        // Vogel spiral (sunflower): r = c*sqrt(i), theta = i*angle
        float k = Mathf.Sqrt(index);
        float t = index * _angleRad;
        float x = spacing * k * Mathf.Cos(t);
        float z = spacing * k * Mathf.Sin(t);
        return new Vector3(x, 0f, z);
    }

    /// <summary>Effective base spacing with adaptive compression to keep radius <= _maxWorldRadius.</summary>
    private float GetEffectiveSpacing()
    {
        if(_maxWorldRadius <= 0f) return _radius;        // no limit
        int n = Mathf.Max(Count, 1);
        float desiredRadius = _radius * Mathf.Sqrt(n);    // radius without compression
        if(desiredRadius <= _maxWorldRadius) return _radius;

        float scale = _maxWorldRadius / desiredRadius;    // 0..1
        return _radius * scale;
    }

    /// <summary>Actual current bounding radius, considering compression.</summary>
    public float GetBoundingRadius()
    {
        int n = Count;
        if(n <= 0) return 0f;
        float spacing = GetEffectiveSpacing();
        return spacing * Mathf.Sqrt(n);
    }

    // Backward compatibility
    public void ApplyBonys(BonusTypes bonusType, int bonusAmaunt) => ApplyBonus(bonusType, bonusAmaunt);

    public void ApplyBonus(BonusTypes bonusType, int bonusAmount)
    {
        if(!_runnersParent) return;

        switch(bonusType)
        {
            case BonusTypes.Addition:
                if(bonusAmount > 0) AddRunners(bonusAmount);
                break;

            case BonusTypes.Difference:
                if(bonusAmount > 0) RemoveRunners(bonusAmount);
                break;

            case BonusTypes.Multiplication:
                if(bonusAmount <= 1) break;
                AddRunners((bonusAmount - 1) * Count);
                break;

            case BonusTypes.Division:
                if(bonusAmount <= 1) break;
                int target = Count / bonusAmount;
                RemoveRunners(Mathf.Max(0, Count - target));
                break;
        }
    }

    private void RemoveRunners(int amount)
    {
        if(!_runnersParent || amount <= 0) return;

        int n = Count;
        amount = Mathf.Min(amount, n);

        for(int i = n - 1; i >= n - amount; i--)
        {
            var t = _runnersParent.GetChild(i);
            t.SetParent(null, false);
            Destroy(t.gameObject);
        }

        _layoutDirty = true;
        NotifyCount();
    }

    private void AddRunners(int amount)
    {
        if(!_runnersParent || !_runnerPrefab || amount <= 0) return;

        // hard cap, if enabled
        if(_maxRunners > 0)
        {
            int free = _maxRunners - Count;
            if(free <= 0) return;
            amount = Mathf.Min(amount, free);
        }

        for(int i = 0; i < amount; i++)
        {
            Instantiate(_runnerPrefab, _runnersParent);
        }

        _layoutDirty = true;
        NotifyCount();
    }

    private void NotifyCount() => OnCountChanged?.Invoke(Count);

    // Optional public API:
    public void SetMaxWorldRadius(float value) { _maxWorldRadius = Mathf.Max(0f, value); MarkLayoutDirty(); }
    public void SetMaxRunners(int max) { _maxRunners = Mathf.Max(0, max); }
}
