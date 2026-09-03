using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 14f;
    [SerializeField] private float scaleShrinkAmount = 0.1f;
    [SerializeField] private float scaleChangeSpeed = 12f;
    [SerializeField] private float scalePulseSpeed = 16f;

    private Rigidbody2D rb;
    private PlayerInventory inventory;
    private Vector3 originalScale;
    private float scalePulseTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inventory = GetComponent<PlayerInventory>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        HandleMovement();
        UpdateMovementScale();
    }

    private void UpdateMovementScale()
    {
        bool isMoving = rb.linearVelocity.sqrMagnitude > 0.01f;
        float targetY = originalScale.y;

        if (isMoving)
        {
            scalePulseTime += Time.deltaTime * scalePulseSpeed;
            float pulse = (Mathf.Sin(scalePulseTime) + 1f) * 0.5f;
            targetY = originalScale.y - (scaleShrinkAmount * pulse);
        }
        else
        {
            scalePulseTime = 0f;
        }

        targetY = Mathf.Max(0.01f, targetY);
        Vector3 targetScale = new Vector3(originalScale.x, targetY, originalScale.z);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleChangeSpeed * Time.deltaTime);
    }

    private void HandleMovement()
    {
        Vector2 move = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                move.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                move.y -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                move.x += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                move.x -= 1f;
        }

        move = move.normalized;

        Vector2 currentVelocity = rb.linearVelocity;
        Vector2 targetVelocity = move * moveSpeed;

        if (move == Vector2.zero)
        {
            currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }

        rb.linearVelocity = currentVelocity;
    }

}