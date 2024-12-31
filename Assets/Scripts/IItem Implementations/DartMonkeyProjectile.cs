using UnityEngine;

public class DartMonkeyProjectile : MonoBehaviour, IItem
{
    public ItemType ItemType => ItemType.DartMonkeyProjectile;
    public GameObject Owner => _owner;
    public GameObject BloonTarget => _bloonTarget;
    public ushort Damage => 1;
    public bool IsExplosive => false;
    public float BloonSlowingFactor => 1.0f;
    public float BloonSlowingDuration => 0.0f;
    public bool CanFreezeBloons => false;
    public float FreezeBloonsDuration => 0.0f;

    private GameObject _owner = null;
    private GameObject _bloonTarget = null;

    private const float _flyingSpeed = 2.5f;
    private const float _maxFlyingTime = 1.5f;
    private float _flyingTime;
    private bool _isFlying;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _flyingTime = 0.0f;
        _isFlying = false;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_isFlying)
        {
            _flyingTime += Time.deltaTime;

            if (_flyingTime > _maxFlyingTime)
            {
                ReturnToPool();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 7)
        {
            IBloon iBloon = other.GetComponent<IBloon>();
            bool isPopSuccessful = iBloon.TryPop(gameObject);

            if (isPopSuccessful)
            {
                Owner.GetComponent<ITower>().IncreaseKilledBloons(1);
            }

            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        _owner = null;
        _bloonTarget = null;
        _isFlying = false;
        _flyingTime = 0.0f;
        _rb.linearVelocity = Vector2.zero;
        ItemsPoolsManager.Instance.ReturnItem(gameObject);
    }

    public void PerformAction()
    {
        Helper.LookAtTarget(gameObject, BloonTarget, 0.0f);
        _rb.linearVelocity = _flyingSpeed * (Vector2)(BloonTarget.transform.position - transform.position).normalized;
        _isFlying = true;
    }

    public void SetNewOwner(GameObject newOwner)
    {
        _owner = newOwner;
    }

    public void SetNewTarget(GameObject newTarget)
    {
        _bloonTarget = newTarget;
    }
}
