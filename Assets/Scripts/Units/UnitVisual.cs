using System.Collections;
using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    private Vector3 originalLocalPos;
    private Coroutine impactRoutine;
    private float queuedForce;
    private float currentDirection;
    private float bufferedForce;
    private float bufferedDirection;
    private Coroutine bufferRoutine;

    /// <summary>
    /// Call this from your combat system
    /// </summary>
    public void PlayImpact(bool isTargetEnemy, float normalizedDamage, bool isCrit)
    {
        float intensity = CalculateIntensity(normalizedDamage, isCrit);
        float direction = isTargetEnemy ? 1f : -1f;

        bufferedForce += intensity;
        bufferedDirection = direction;

        if (bufferRoutine == null)
            bufferRoutine = StartCoroutine(ImpactBuffer());
    }

    private IEnumerator ImpactBuffer()
    {
        // Wait a short time to gather multiple hits
        yield return new WaitForSeconds(0.05f);

        float finalForce = bufferedForce;
        float finalDirection = bufferedDirection;

        bufferedForce = 0f;
        bufferRoutine = null;

        // Now play ONE impact
        if (impactRoutine != null)
            StopCoroutine(impactRoutine);

        impactRoutine = StartCoroutine(ImpactRoutine(finalDirection, finalForce));
    }

    private IEnumerator ImpactRoutine(float direction, float intensity)
    {
        Vector3 start = transform.localPosition;
        Vector3 hit = start + new Vector3(direction * intensity, 0f, 0f);

        // Quick push
        transform.localPosition = hit;

        // Slight pause (gives weight)
        yield return new WaitForSeconds(0.08f);

        float t = 0f;
        float duration = 0.25f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            // Ease-out for smoother return
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            transform.localPosition = Vector3.Lerp(hit, start, easedT);
            yield return null;
        }

        transform.localPosition = start;
        impactRoutine = null;
    }

    private float CalculateIntensity(float normalizedDamage, bool isCrit)
    {
        var baseIntensity = 1f;

        float intensity = normalizedDamage * baseIntensity;

        if (isCrit)
            intensity *= 1.5f;

        return Mathf.Clamp(intensity, 0.05f, 0.3f);
    }
}