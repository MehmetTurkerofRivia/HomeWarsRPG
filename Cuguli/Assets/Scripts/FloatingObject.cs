using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [SerializeField] private float verticalAmount = 0.05f;
    [SerializeField] private float oscillationsPerSecond = 1f;

    private Vector3 startLocalPosition;
    private Vector3 animationOffset;

    private void OnEnable()
    {
        startLocalPosition = transform.localPosition;
    }

    public void SetStartPosition(Vector3 position)
    {
        startLocalPosition = position;
        transform.localPosition = position;
    }

    public void SetAnimationOffset(Vector3 offset)
    {
        animationOffset = offset;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * oscillationsPerSecond * Mathf.PI * 2f) * verticalAmount;
        transform.localPosition = startLocalPosition + Vector3.up * offset + animationOffset;
    }
}
