using UnityEngine;

public class WeaponOrbitVisual : MonoBehaviour
{
    private Transform owner;
    private float radius = 0.9f;
    private float rotateSpeed = 260f;
    private float offsetAngle = -90f;
    private Vector3 localOffset = new Vector3(0f, 0.35f, 0f);
    private float angle;

    public void Initialize(Transform ownerTransform, float radiusValue, float speed, float startAngle, Vector3 offset)
    {
        owner = ownerTransform;
        radius = radiusValue;
        rotateSpeed = speed;
        offsetAngle = startAngle;
        localOffset = offset;
        angle = startAngle;
    }

    private void Update()
    {
        if (owner == null)
            return;

        angle -= rotateSpeed * Time.deltaTime;
        Vector3 orbitPosition = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f) * radius;
        transform.position = owner.position + orbitPosition + localOffset;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + offsetAngle);
    }
}
