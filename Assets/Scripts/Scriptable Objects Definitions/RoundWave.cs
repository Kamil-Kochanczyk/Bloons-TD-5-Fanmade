using UnityEngine;

/*
 * This file specifies the structure of scriptable objects containing meta info about round waves
 * A round wave is basically a container for all bloon waves unleashed during one particular round
 */

[CreateAssetMenu(fileName = "Round[Number]Wave", menuName = "Scriptable Objects/RoundWave")]
public class RoundWave : ScriptableObject
{
    public BloonWave[] bloonWaves;
}
