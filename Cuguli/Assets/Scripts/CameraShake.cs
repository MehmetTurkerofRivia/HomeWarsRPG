using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 baseLocalPosition;
    private float remainingTime;
    private float duration;
    private float strength;

    private void Awake()
    {
        baseLocalPosition = transform.localPosition;
    }

    public void Shake(float shakeDuration, float shakeStrength)
    {
        duration = Mathf.Max(duration, shakeDuration);
        remainingTime = Mathf.Max(remainingTime, shakeDuration);
        strength = Mathf.Max(strength, shakeStrength);
    }

    private void LateUpdate()
    {
        if (remainingTime <= 0f)
        {
            transform.localPosition = baseLocalPosition;
            return;
        }

        remainingTime -= Time.unscaledDeltaTime;
        float falloff = duration > 0f ? remainingTime / duration : 0f;
        transform.localPosition = baseLocalPosition + (Vector3)(Random.insideUnitCircle * strength * falloff);
    }
}
