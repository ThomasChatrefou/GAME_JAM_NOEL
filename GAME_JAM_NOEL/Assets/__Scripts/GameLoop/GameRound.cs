using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Round", menuName = "ScriptableObjects/GameLoop/Round")]
public class GameRound : ScriptableObject
{
    public float Duration;
    public List<SpawnParams> Spawn;
}

[Serializable]
public struct SpawnParams
{
    [Tooltip("Type of Enemy to spawn")]
    public GameObject EnemyPrefab;
    [Tooltip("Zones in which Enemies will spawn")]
    public SpawnZone[] Zones;
    [Tooltip("Total amount of spawned Enemies over the EnemySpawnParam")]
    public int Amount;
    [Tooltip("Time between two Enemy spawns in seconds")]
    public float Period;
    [Tooltip("time in % of the GameRound when the Spawn will Activate")]
    [Range(0f, 1f)] public float ActivationTiming;
    [Tooltip("time in % of the GameRound when the Spawn will Stop\nMust be > timeBeforeActivate")]
    [Range(0f, 1f)] public float StopTiming;

    public Vector3 RandomPosInRectTopLeftToBottomRight(Vector2 topLeft, Vector2 bottomRight)
    {
        List<Vector3> pos = new List<Vector3>();

        foreach (SpawnZone zone in Zones)
            pos.Add(zone.GetRandomPosition(topLeft, bottomRight));

        return pos[Random.Range(0, Zones.Length)];
    }
}