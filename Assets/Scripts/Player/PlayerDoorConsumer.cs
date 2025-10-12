using Assets.Scripts.Player;
using DoorChunks.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoorConsumer : MonoBehaviour, IConsumeDoorChoice
{
    [Header("References")]
    [SerializeField, Tooltip("Aggregates player state and routes door bonuses.")]
    private PlayerStats _stats;

    public void OnDoorChoice(DoorChunk source, bool rightSide, int value, BonusTypes type)
    {
        if(_stats == null)
        {
            Debug.LogWarning("[PlayerDoorConsumer] PlayerStats reference is missing.", this);
            return;
        }

        _stats.ApplyDoorChoice(source, rightSide, value, type);
    }
}
