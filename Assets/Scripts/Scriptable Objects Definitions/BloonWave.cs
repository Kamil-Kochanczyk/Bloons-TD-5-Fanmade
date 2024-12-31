using UnityEngine;

/*
 * This file specifies the structure of scriptable objects containing meta info about bloon waves
 * A bloon wave is a series of bloons of some type type unleashed one after another in some time intervals
 * During the game there can be one or more bloon waves unleashed in each round
 * All bloon waves unleashed during one particular round form what is called a round wave
 */

[CreateAssetMenu(fileName = "BloonWave[Number]", menuName = "Scriptable Objects/BloonWave")]
public class BloonWave : ScriptableObject
{
    public BloonType bloonType;
    public ushort bloonsCount;
    public float nextBloonWaitTime;
    public float nextWaveWaitTime;
}
