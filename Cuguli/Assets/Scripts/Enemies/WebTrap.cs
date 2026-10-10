using UnityEngine;

public class WebTrap : MonoBehaviour
{
    [SerializeField, Range(0.05f, 1f)] private float speedMultiplier = 0.45f;
    [SerializeField] private float slowDuration = 1.5f;
    [SerializeField] private float lifetime = 8f;

    private void Awake()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ApplySlow(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ApplySlow(collision.collider);
    }

    private void ApplySlow(Collider2D other)
    {
        PlayerMovement movement = other.GetComponentInParent<PlayerMovement>();
        if (movement != null)
            movement.ApplySlow(speedMultiplier, slowDuration);
    }

    private void OnValidate()
    {
        slowDuration = Mathf.Max(0f, slowDuration);
        lifetime = Mathf.Max(0.1f, lifetime);
    }
}
