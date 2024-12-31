using UnityEngine;
using UnityEngine.InputSystem;

/*
 * TODO: LevelManager description
 */

public class LevelManager : MonoBehaviour
{
    [SerializeField] private InputAction _spacePress;

    [SerializeField] private DifficultySettings _difficultySettings;

    public Difficulty Difficulty => _difficultySettings.difficulty;
    public ulong Money { get; private set; }
    public ushort Lives { get; private set; }
    public byte RoundsBeforeFreeplay => _difficultySettings.roundsBeforeFreeplay;
    public float CostsModifier => _difficultySettings.costsModifier;
    public float BloonSpeedModifier => _difficultySettings.bloonSpeedModifier;
    public RoundWave[] RoundWaves => _difficultySettings.roundWaves;

    public ushort Round { get; private set; } = 1;
    public bool IsRoundOngoing { get; private set; } = false;

    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Money = _difficultySettings.money;
        Lives = _difficultySettings.lives;
    }

    private void OnEnable()
    {
        _spacePress.performed += OnSpacePress;
        _spacePress.Enable();
    }

    private void OnDisable()
    {
        _spacePress.Disable();
        _spacePress.performed -= OnSpacePress;
    }

    private void OnSpacePress(InputAction.CallbackContext context)
    {
        if (!IsRoundOngoing)
        {
            StartRound();
        }
    }

    public void StartRound()
    {
        IsRoundOngoing = true;

        if (Round <= RoundsBeforeFreeplay)
        {
            RoundWaveManager.Instance.PrepareRoundWave(RoundWaves[Round - 1]);
        }
        else
        {
            RoundWaveManager.Instance.PrepareRandomRoundWave(Round);
        }

        RoundWaveManager.Instance.UnleashRoundWave();
    }

    public void EndRound()
    {
        IsRoundOngoing = false;
        Round++;
        RoundWaveManager.Instance.ClearRoundWave();
    }

    public void AddLives(ushort livesToAdd)
    {
        Lives += livesToAdd;
    }

    public void LoseLives(ushort livesToLose)
    {
        Lives = (ushort)((Lives > livesToLose) ? (Lives - livesToLose) : 0);
    }

    public void AddMoney(ulong moneyToAdd)
    {
        Money += moneyToAdd;
    }

    public void LoseMoney(ulong moneyToLose)
    {
        Money = (ulong)((Money > moneyToLose) ? (Money - moneyToLose) : 0);
    }
}
