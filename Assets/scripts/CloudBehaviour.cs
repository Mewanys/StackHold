using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudBehaviour : MonoBehaviour
{
    [Header("Life")]
    public float lifeTime = 20f;
    public float fadeTime = 4f;

    [Header("Movement")]
    public float moveSpeed = 0.6f;
    public float internalDrift = 0.25f;
    public float internalRotate = 10f;

    Vector3 moveDir;
    List<Renderer> rends = new List<Renderer>();
    List<Transform> layers = new List<Transform>();
    List<Vector3> basePos = new List<Vector3>();

    void Start()
    {
        moveDir = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ).normalized;

        foreach (Transform t in transform)
        {
            layers.Add(t);
            basePos.Add(t.localPosition);

            Renderer r = t.GetComponent<Renderer>();
            if (r != null)
            {
                rends.Add(r);
                Color c = r.material.color;
                c.a = 0;
                r.material.color = c;
            }
        }

        StartCoroutine(LifeRoutine());
    }

    void Update()
    {
        // движение облака
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        // внутреннее движение слоёв
        for (int i = 0; i < layers.Count; i++)
        {
            Vector3 offset = new Vector3(
                Mathf.Sin(Time.time * 0.6f + i),
                Mathf.Cos(Time.time * 0.4f + i) * 0.3f,
                Mathf.Cos(Time.time * 0.7f + i)
            ) * internalDrift;

            layers[i].localPosition = basePos[i] + offset;

            layers[i].Rotate(Vector3.up,
                internalRotate * Time.deltaTime * (i % 2 == 0 ? 1 : -1));
        }
    }

    IEnumerator LifeRoutine()
    {
        yield return StartCoroutine(Fade(0, 1));
        yield return new WaitForSeconds(lifeTime);
        yield return StartCoroutine(Fade(1, 0));
        Destroy(gameObject);
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / fadeTime);

            foreach (Renderer r in rends)
            {
                Color c = r.material.color;
                c.a = a;
                r.material.color = c;
            }

            yield return null;
        }
    }
}
