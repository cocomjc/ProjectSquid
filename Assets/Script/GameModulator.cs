using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameModulator", menuName = "ScriptableObjects/GameModulator", order = 1)]
public class GameModulator : ScriptableObject
{
    public float swimPropulsionPower;
    public float swimSpeedBeforeNextMove;
    public float grabMoveSpeed;
    public float grabDistanceWhileMove;
    public float grabRotationSpeed; 
}
