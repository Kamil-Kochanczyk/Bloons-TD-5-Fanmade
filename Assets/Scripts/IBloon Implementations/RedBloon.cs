using UnityEngine;

public class RedBloon : MonoBehaviour, IBloon
{
    public BloonType BloonType => BloonType.Red;
    public BloonType ChildType => BloonType.Red;
    public byte ChildrenCount => 0;
    public float DefaultSpeed => 1.0f;
    public ushort HealthPoints => _healthPoints;
    public ushort LivesWorth => 1;
    public bool IsCamo => false;
    public bool IsRegrow => false;
    public bool IsSlowedDown => _isSlowedDown;
    public bool IsFrozen => _isFrozen;
    public bool IsDizzy => _isDizzy;

    private ushort _healthPoints = 1;
    private bool _isSlowedDown = false;
    private bool _isFrozen = false;
    private bool _isDizzy = false;

    public bool TryPop(GameObject poppingItem)
    {
        bool tryPop = false;

        IItem item = poppingItem.GetComponent<IItem>();
        AlongThePathMover alongThePathMover = GetComponent<AlongThePathMover>();

        _healthPoints -= item.Damage;

        if (_healthPoints <= 0 )
        {
            _healthPoints = 1;
            _isSlowedDown = false;
            _isFrozen = false;
            _isDizzy = false;
            BloonsPoolsManager.Instance.ReturnBloon(gameObject);
            PathManager.Instance.RemoveBloonFromPath();
            tryPop = true;
        }

        if (item.BloonSlowingFactor != 1.0f)
        {
            alongThePathMover.SlowDown(item.BloonSlowingFactor, item.BloonSlowingDuration);
        }

        if (item.CanFreezeBloons)
        {
            alongThePathMover.Stop(item.FreezeBloonsDuration);
        }

        return tryPop;
    }
}
