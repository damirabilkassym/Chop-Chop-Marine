using UnityEngine;
using System.Collections;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance;

    void Awake() => Instance = this;

    public void DoHitStop(float duration, float timeScale)
    {
        StartCoroutine(HitStopRoutine(duration, timeScale));
    }

    IEnumerator HitStopRoutine(float duration, float timeScale)
    {
        Time.timeScale = timeScale;
        
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        
        IEnumerator HitStopRoutine(float duration, float timeScale)
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // Подстройка физики под время

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f; // Возврат к стандарту
        }
    }
}