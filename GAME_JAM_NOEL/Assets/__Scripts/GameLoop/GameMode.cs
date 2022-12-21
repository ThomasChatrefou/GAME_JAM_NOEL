using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameMode", menuName = "ScriptableObjects/GameLoop/GameMode")]
public class GameMode : ScriptableObject
{
    public float timeBetweenRounds;
    public GameRound[] rounds;
}
