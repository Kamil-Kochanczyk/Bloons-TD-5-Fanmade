using UnityEngine;

public class TackShooterTackForwarder : MonoBehaviour
{
    private TackShooterTacks _tackShooterTacksScript;

    private void Awake()
    {
        _tackShooterTacksScript = transform.parent.GetComponent<TackShooterTacks>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _tackShooterTacksScript.HandleTackOnTriggerEnter2D(gameObject, other);
    }
}
