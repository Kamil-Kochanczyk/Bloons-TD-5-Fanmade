using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * TODO: RoundWaveManager description
 */

public class RoundWaveManager : MonoBehaviour
{
    private RoundWave _roundWave = null;
    private bool _isRoundWaveUnleashed = false;
    
    public bool IsRoundWaveUnleashed => _isRoundWaveUnleashed;

    public static RoundWaveManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PrepareRoundWave(RoundWave roundWave)
    {
        _roundWave = roundWave;
        BloonsPoolsManager.Instance.InitializeNewBloonsPools(roundWave);
    }

    public void PrepareRandomRoundWave(ushort round)
    {
        // x + Mathf.FloorToInt(round / y) <=> result will be increased every y rounds and will be guaranteed to be at least x

        int minWaves = 2 + Mathf.FloorToInt(round / 5.0f);
        int maxWaves = minWaves + 2;
        int waveCount = Random.Range(minWaves, maxWaves + 1);

        BloonType maxBloonType = (BloonType)(System.Enum.GetValues(typeof(BloonType)).Length - 1);

        int minBloons = 5 + Mathf.FloorToInt(round / 2.0f);
        int maxBloons = minBloons + 10;

        List<BloonWave> bloonWaves = new List<BloonWave>();

        for (int i = 0; i < waveCount; i++)
        {
            BloonWave bloonWave = ScriptableObject.CreateInstance<BloonWave>();
            bloonWave.bloonType = (BloonType)Random.Range(0, (int)maxBloonType + 1);
            bloonWave.bloonsCount = (ushort)Random.Range(minBloons, maxBloons + 1);
            bloonWave.nextBloonWaitTime = Random.Range(0.2f, 0.5f);
            bloonWave.nextWaveWaitTime = Random.Range(0.5f, 3.0f);
            bloonWaves.Add(bloonWave);
        }

        RoundWave randomRoundWave = ScriptableObject.CreateInstance<RoundWave>();
        randomRoundWave.bloonWaves = bloonWaves.ToArray();

        PrepareRoundWave(randomRoundWave);
    }

    public void UnleashRoundWave()
    {
        StartCoroutine(RoundWaveUnleasher());
    }

    private IEnumerator RoundWaveUnleasher()
    {
        _isRoundWaveUnleashed = false;

        foreach (BloonWave bloonWave in _roundWave.bloonWaves)
        {
            for (int i = 0; i < bloonWave.bloonsCount; i++)
            {
                GameObject bloon = BloonsPoolsManager.Instance.GetBloon(bloonWave.bloonType);
                PathManager.Instance.AddBloonToPath();
                bloon.GetComponent<AlongThePathMover>().StartMovingFromSpawnPoint();
                yield return new WaitForSeconds(bloonWave.nextBloonWaitTime);
            }
            yield return new WaitForSeconds(bloonWave.nextWaveWaitTime);
        }

        _isRoundWaveUnleashed = true;
    }

    public void ClearRoundWave()
    {
        _roundWave = null;
        BloonsPoolsManager.Instance.ClearAllPools();
    }
}
