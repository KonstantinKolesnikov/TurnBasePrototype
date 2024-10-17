using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;
    public GameObject impactEffect;
    public GameObject lineEffect;
    private void Awake()
    {
        Instance = this;
    }

    public Coroutine StartImpactEffect(Vector3 position, float seconds)
    {
        return StartCoroutine(ImpactEffect(position, seconds));
    }
    public IEnumerator ImpactEffect(Vector3 position, float seconds)
    {
        GameObject obj = Instantiate(impactEffect, position, Quaternion.identity);
        yield return new WaitForSeconds(seconds);
        Destroy(obj);
    }

    public Coroutine StartLineEffect(Vector3 position, Vector3 positionTo, float seconds)
    {
        return StartCoroutine(LineEffect(position, positionTo, seconds));
    }

    public IEnumerator LineEffect(Vector3 position, Vector3 positionTo, float seconds)
    {
        GameObject obj = Instantiate(lineEffect, position, Quaternion.identity);
        LineRenderer lineRenderer = obj.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, position);
        lineRenderer.SetPosition(1, positionTo);
        yield return new WaitForSeconds(seconds);
        Destroy (obj);
    }
}
