using UnityEngine;
using System.Collections;

public class WeaponRotation : MonoBehaviour
{
    public Camera cam;
    public float swingSpeed = 20f;
    public float swingAngle = 90f;

    private bool isAttacking = false;

    void Update()
    {
        
        if (!isAttacking)
        {
            RotateTowardsMouse();
        }

        
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(SwingSword());
        }

        
        if (Input.GetMouseButtonDown(1) && !isAttacking)
        {
            StartCoroutine(ParryAction());
        }

        void RotateTowardsMouse()
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 lookDir = mousePos - transform.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

            
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

            
            Transform swordTransform = transform.GetChild(0);
            Vector3 newScale = swordTransform.localScale;

            
            if (mousePos.x < transform.position.x)
            {
                
                newScale.x = -Mathf.Abs(newScale.x);
            }
            else
            {
                
                newScale.x = Mathf.Abs(newScale.x);
            }

            swordTransform.localScale = newScale;
        }
    }

    void RotateTowardsMouse()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    IEnumerator SwingSword()
    {
        isAttacking = true;

        Quaternion startRot = transform.localRotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, -swingAngle);

        float t = 0;
       
        while (t < 1f)
        {
            t += Time.deltaTime * swingSpeed;
            transform.localRotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        
        yield return new WaitForSeconds(0.1f);

        
        transform.localRotation = startRot;

        isAttacking = false;
    }

    IEnumerator ParryAction()
    {
        isAttacking = true; 

        
        Quaternion parryRot = transform.localRotation * Quaternion.Euler(0, 0, 45f);
        transform.localRotation = parryRot;

        
        yield return new WaitForSeconds(0.2f); 

        isAttacking = false;
    }
}