using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Round", menuName = "ScriptableObjects/GameLoop/Round")]
public class GameRound : ScriptableObject
{
    public float Duration;
    public List<Spawn> Spawn;
}

[Serializable]
public struct Spawn
{
    public EnemySpawn EnemySpawn;
    public float timeBeforeActivate;
}