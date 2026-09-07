using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private float zoomOutSpeed = 0.7f;
    [SerializeField] private float maxZoomAmount = 1f;
    [SerializeField] private float followSpeed = 12f;
    [SerializeField] private float zoomOutDelay = 0.3f;

    private float defaultOrthographicSize;
    private float defaultFieldOfView;
    private float currentZoomAmount;
    private Camera cameraComponent;
    private Transform followTarget;
    private float lastZoomInTime = float.NegativeInfinity;

    private void Awake()
    {
        zoomOutSpeed = 0.7f;
        maxZoomAmount = 1f;
        cameraComponent = GetComponent<Camera>();
        if (cameraComponent != null)
        {
            defaultOrthographicSize = cameraComponent.orthographicSize;
            defaultFieldOfView = cameraComponent.fieldOfView;
        }
    }

    public void ZoomIn(float amount)
    {
        if (cameraComponent == null)
            return;

        lastZoomInTime = Time.unscaledTime;
        currentZoomAmount = Mathf.Min(
            currentZoomAmount + Mathf.Max(0f, amount),
            Mathf.Max(0f, maxZoomAmount));
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }

    private void LateUpdate()
    {
        if (cameraComponent == null)
            return;

        if (Time.unscaledTime - lastZoomInTime >= zoomOutDelay)
        {
            currentZoomAmount = Mathf.MoveTowards(
                currentZoomAmount,
                0f,
                zoomOutSpeed * Time.unscaledDeltaTime);
        }

        if (cameraComponent.orthographic)
        {
            cameraComponent.orthographicSize = Mathf.Max(0.1f, defaultOrthographicSize - currentZoomAmount);
        }
        else
        {
            cameraComponent.fieldOfView = Mathf.Max(1f, defaultFieldOfView - currentZoomAmount);
        }

        if (followTarget == null)
            return;

        Vector3 targetPosition = new Vector3(followTarget.position.x, followTarget.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.unscaledDeltaTime);
    }
}
