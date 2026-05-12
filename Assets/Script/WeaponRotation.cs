using UnityEngine;

public class WeaponRotation : MonoBehaviour
{
    public Camera cam;

    void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (cam == null) return;

        // Только поворот за мышью
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
}