using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TackShooterTacks : MonoBehaviour, IItem
{
    public ItemType ItemType => ItemType.TackShooterTacks;
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

    GameObject[] _tacks;
    Vector2[] _tacksInitialPositions;
    private const float _flyingSpeed = 2.0f;
    private const float _maxFlyingTime = 1.0f;
    private float _tackFlyingTime;
    private bool _areTacksFlying;

    private void Awake()
    {
        _tacks = new GameObject[transform.childCount];

        for (int i = 0; i < _tacks.Length; i++)
        {
            _tacks[i] = transform.GetChild(i).gameObject;
        }

        _tacksInitialPositions = new Vector2[_tacks.Length];

        _tackFlyingTime = 0.0f;
        _areTacksFlying = false;
    }

    private void Update()
    {
        if (_areTacksFlying)
        {
            _tackFlyingTime += Time.deltaTime;

            if ( _tackFlyingTime > _maxFlyingTime)
            {
                ReturnToPool();
            }
        }
    }

    public void HandleTackOnTriggerEnter2D(GameObject tack, Collider2D other)
    {
        if (other.gameObject.layer == 7)
        {
            IBloon iBloon = other.GetComponent<IBloon>();
            bool isPopSuccessful = iBloon.TryPop(gameObject);

            if (isPopSuccessful)
            {
                Owner.GetComponent<ITower>().IncreaseKilledBloons(1);
            }

            tack.GetComponent<Collider2D>().enabled = false;
            tack.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void ReturnToPool()
    {
        _owner = null;
        _bloonTarget = null;

        _areTacksFlying = false;
        _tackFlyingTime = 0.0f;

        ItemsPoolsManager.Instance.ReturnItem(gameObject);

        for (int i = 0; i < _tacks.Length; i++)
        {
            _tacks[i].GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            _tacks[i].GetComponent<Collider2D>().enabled = true;
            _tacks[i].GetComponent<SpriteRenderer>().enabled = true;
            _tacks[i].transform.position = _tacksInitialPositions[i];
        }
    }

    public void PerformAction()
    {
        for (int i = 0; i < _tacksInitialPositions.Length; i++)
        {
            _tacksInitialPositions[i] = _tacks[i].transform.position;
        }

        _tacks[0].transform.GetComponent<Rigidbody2D>().linearVelocity = _flyingSpeed * Vector2.up;
        _tacks[1].transform.GetComponent<Rigidbody2D>().linearVelocity = _flyingSpeed * Helper.Rotate(Vector2.up, -30.0f).normalized;
        _tacks[2].transform.GetComponent<Rigidbody2D>().linearVelocity = _flyingSpeed * Helper.Rotate(Vector2.up, -60.0f).normalized;
        _tacks[3].transform.GetComponent<Rigidbody2D>().linearVelocity = _flyingSpeed * Vector2.right;
        _tacks[4].transform.GetComponent<Rigidbody2D>().linearVelocity = _flyingSpeed * Helper.Rotate(Vector2.right, -30.0f).normalized;
        _tacks[5].transform.GetComponent<Rigidbody2D>().linearVelocity = _flyingSpeed * Helper.Rotate(Vector2.right, -60.0f).normalized;
        _tacks[6].transform.GetComponent<Rigidbody2D>().linearVelocity = _flyingSpeed * Vector2.down;
        _tacks[7].transform.GetComponent<Rigidbody2D>().linearVelocity = _flyingSpeed * Helper.Rotate(Vector2.down, -30.0f).normalized;
        _tacks[8].transform.GetComponent<Rigidbody2D>().linearVelocity = _flyingSpeed * Helper.Rotate(Vector2.down, -60.0f).normalized;
        _tacks[9].transform.GetComponent<Rigidbody2D>().linearVelocity = _flyingSpeed * Vector2.left;
        _tacks[10].transform.GetComponent<Rigidbody2D>().linearVelocity = _flyingSpeed * Helper.Rotate(Vector2.left, -30.0f).normalized;
        _tacks[11].transform.GetComponent<Rigidbody2D>().linearVelocity = _flyingSpeed * Helper.Rotate(Vector2.left, -60.0f).normalized;

        _areTacksFlying = true;
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
