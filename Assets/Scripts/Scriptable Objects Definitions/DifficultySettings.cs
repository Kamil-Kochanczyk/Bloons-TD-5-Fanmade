using UnityEngine;

/*
 * This file specifies the structure of scriptable objects containing high-level meta info about the game and some initial/default values
 */

[CreateAssetMenu(fileName = "[Easy/Medium/Hard]DifficultySettings", menuName = "Scriptable Objects/DifficultySettings")]
public class DifficultySettings : ScriptableObject
{
    public Difficulty difficulty;
    public ulong money;
    public ushort lives;
    public byte roundsBeforeFreeplay;
    public float costsModifier;
    public float bloonSpeedModifier;
    public RoundWave[] roundWaves;
}
