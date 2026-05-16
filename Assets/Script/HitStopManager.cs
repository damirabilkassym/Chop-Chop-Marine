using UnityEngine;
using System.Collections;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance;

    private bool isHitStopping = false;

    void Awake()
    {
        // Настройка синглтона
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DoHitStop(float duration, float timeScale)
    {
        // Если хит-стоп уже идёт, не наслаиваем новый, чтобы не сломать тайминги
        if (isHitStopping) return;

        StartCoroutine(HitStopRoutine(duration, timeScale));
    }

    IEnumerator HitStopRoutine(float duration, float timeScale)
    {
        isHitStopping = true;

        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Подстройка физики, чтобы не было микрофризов

        // Используем WaitForSecondsRealtime, так как обычный WaitForSeconds замрёт вместе с Time.timeScale
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f; // Возвращаем физику к стандарту Unity

        isHitStopping = false;
    }
}