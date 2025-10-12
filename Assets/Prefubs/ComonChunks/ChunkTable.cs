using UnityEngine;

public enum ChunkTag { Generic, Empty, Long, Hard }

/// <summary>
/// A weighted pool of chunk prefabs used by the ChunkManager to build levels.
/// Each entry defines: prefab reference, relative spawn weight, grouping tag,
/// and repetition rules.
/// </summary>
[CreateAssetMenu(menuName = "Game/Chunks/Weighted Table", fileName = "ChunkTable_")]
public class ChunkTable : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        [Header("Prefab")]
        [Tooltip("Chunk prefab to spawn. Must implement GetLength() > 0 for correct spacing.")]
        public Chunk prefab;

        [Header("Weight (relative probability)")]
        [Min(0f)]
        [Tooltip("Relative chance. 0 = never picked. Final probability = weight / sum(all active weights).")]
        public float weight = 1f;

        [Header("Grouping & repetition")]
        [Tooltip("Category tag used to limit repeating the same group (e.g. avoid too many Empty/Long in a row).")]
        public ChunkTag tag = ChunkTag.Generic;

        [Tooltip("Max allowed same TAG in a row. 0 = unlimited. Example: 1 means you cannot have two 'Empty' chunks back-to-back.")]
        public int maxConsecutiveTag = 0;

        [Tooltip("If false, this very same prefab cannot appear twice in a row.")]
        public bool allowImmediateRepeat = true;
    }

    [Tooltip("List of all candidate chunks used by the ChunkManager.")]
    public Entry[] entries;
}