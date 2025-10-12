using System.Collections.Generic;
using UnityEngine;

public enum GenerationMode { FixedLength, Endless }

public class ChunkManager : MonoBehaviour
{
    [Header("Mode")]
    [SerializeField] private GenerationMode _mode = GenerationMode.FixedLength;

    [Header("Prefabs")]
    [SerializeField] private Chunk _startChunk;
    [SerializeField] private Chunk _finishChunk;
    [SerializeField] private ChunkTable _table;

    [Header("Fixed Length")]
    [Min(0f)][SerializeField] private float _targetLength = 100f;

    [Header("Endless")]
    [SerializeField] private Transform _follow;
    [Min(0f)][SerializeField] private float _keepAheadDistance = 80f;
    [Min(0f)][SerializeField] private float _despawnBehindDistance = 40f;

    [Header("Random")]
    [SerializeField] private bool _useSeed = false;
    [SerializeField] private int _seed = 12345;

    private readonly List<(Chunk inst, float startZ, float endZ)> _spawned = new();
    private float _cursorZ;
    private int _lastEntryIndex = -1;
    private ChunkTag _lastTag = ChunkTag.Generic;
    private int _consecutiveTagCount = 0;

    void Start()
    {
        if(_useSeed) Random.InitState(_seed);
        Rebuild();
    }

    void Update()
    {
        if(_mode != GenerationMode.Endless || _follow == null) return;

        float playerZ = transform.InverseTransformPoint(_follow.position).z;

        int safety = 0; const int SAFETY_LIMIT = 2048;
        while((_cursorZ - playerZ) < _keepAheadDistance)
        {
            if(!TrySpawnFromTable())
            {
                Debug.LogWarning("[ChunkManager] Нет кандидатов для спавна (Endless). Останавливаю догенерацию.", this);
                break;
            }
            if(++safety > SAFETY_LIMIT) { Debug.LogError("[ChunkManager] Safety break (Endless).", this); break; }
        }

        // Чистим хвост
        while(_spawned.Count > 0 && _spawned[0].endZ < playerZ - _despawnBehindDistance)
        {
            if(_spawned[0].inst) Destroy(_spawned[0].inst.gameObject);
            _spawned.RemoveAt(0);
        }
    }

    [ContextMenu("Rebuild")]
    public void Rebuild()
    {
        foreach(var s in _spawned) if(s.inst) DestroyImmediate(s.inst.gameObject);
        _spawned.Clear();
        _cursorZ = 0f; _lastEntryIndex = -1; _lastTag = ChunkTag.Generic; _consecutiveTagCount = 0;

        if(_table == null || _table.entries == null || _table.entries.Length == 0)
        {
            Debug.LogWarning("[ChunkManager] Пустая таблица чанков.", this);
        }

        if(_startChunk) SpawnExact(_startChunk);

        if(_mode == GenerationMode.FixedLength)
        {
            float finLen = _finishChunk ? Mathf.Max(0.01f, _finishChunk.GetLength()) : 0f;

            int safety = 0; const int SAFETY_LIMIT = 10000;
            while((_cursorZ + finLen) < _targetLength)
            {
                if(!TrySpawnFromTable())
                {
                    Debug.LogWarning("[ChunkManager] Нет кандидатов для спавна (FixedLength). Заканчиваю ранний.", this);
                    break;
                }
                if(++safety > SAFETY_LIMIT) { Debug.LogError("[ChunkManager] Safety break (FixedLength).", this); break; }
            }

            if(_finishChunk) SpawnExact(_finishChunk);
        }
        else
        {
            float startFrom = _follow ? transform.InverseTransformPoint(_follow.position).z : 0f;
            int safety = 0; const int SAFETY_LIMIT = 2048;
            while((_cursorZ - startFrom) < _keepAheadDistance)
            {
                if(!TrySpawnFromTable())
                {
                    Debug.LogWarning("[ChunkManager] Нет кандидатов для спавна при прогреве (Endless).", this);
                    break;
                }
                if(++safety > SAFETY_LIMIT) { Debug.LogError("[ChunkManager] Safety break (prewarm).", this); break; }
            }
        }
    }

    private bool TrySpawnFromTable()
    {
        if(_table == null || _table.entries == null || _table.entries.Length == 0)
            return false;

        var candidates = new List<int>();
        float totalW = 0f;

        bool singleWithNoRepeat = _table.entries.Length == 1 && _table.entries[0].allowImmediateRepeat == false;

        for(int i = 0; i < _table.entries.Length; i++)
        {
            var e = _table.entries[i];
            if(e.prefab == null || e.weight <= 0f) continue;

            bool forbidImmediate = !e.allowImmediateRepeat && !singleWithNoRepeat;

            if(forbidImmediate && i == _lastEntryIndex) continue;

            if(e.maxConsecutiveTag > 0 && e.tag == _lastTag && _consecutiveTagCount >= e.maxConsecutiveTag)
                continue;

            candidates.Add(i);
            totalW += e.weight;
        }

        if(candidates.Count == 0)
        {
            for(int i = 0; i < _table.entries.Length; i++)
            {
                var e = _table.entries[i];
                if(e.prefab == null || e.weight <= 0f) continue;

                bool forbidImmediate = !e.allowImmediateRepeat && !singleWithNoRepeat;
                if(forbidImmediate && i == _lastEntryIndex) continue;

                candidates.Add(i);
                totalW += e.weight;
            }
        }

        if(candidates.Count == 0) return false;

        if(singleWithNoRepeat && _lastEntryIndex == 0)
            Debug.LogWarning("[ChunkManager] В таблице один элемент с запретом повтора. Разрешаю повтор, чтобы не зависнуть.", this);

        float r = Random.value * totalW;
        float acc = 0f;
        int chosen = candidates[0];
        foreach(int i in candidates)
        {
            acc += _table.entries[i].weight;
            if(r <= acc) { chosen = i; break; }
        }

        var entry = _table.entries[chosen];
        SpawnExact(entry.prefab);

        _consecutiveTagCount = (entry.tag == _lastTag) ? _consecutiveTagCount + 1 : 1;
        _lastTag = entry.tag;
        _lastEntryIndex = chosen;

        return true;
    }

    private void SpawnExact(Chunk prefab)
    {
        float len = Mathf.Max(0.01f, prefab.GetLength());
        float start = _cursorZ;
        float end = start + len;

        var localPos = new Vector3(0f, 0f, start + len * 0.5f);
        var inst = Instantiate(prefab, transform.TransformPoint(localPos), transform.rotation, transform);

        _spawned.Add((inst, start, end));
        _cursorZ = end;
    }
}
