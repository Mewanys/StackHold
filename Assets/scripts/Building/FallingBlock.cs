using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dropSpeed = 8f;

    private bool dropping = false;

    void Update()
    {
        if (!dropping)
        {
            transform.position += Vector3.right * Mathf.Sin(Time.time * moveSpeed) * Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0))
        {
            dropping = true;
        }

        if (dropping)
        {
            transform.position += Vector3.down * dropSpeed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BuildZone"))
        {
            float accuracy = CalculateAccuracy(other.transform.position.x);

            if (accuracy > 0.2f)
            {
                BuildingManager.Instance.RegisterBlock(accuracy);
                BlockSpawner.Instance.SpawnBlock();
                Destroy(gameObject);
            }
            else
            {
                Miss();
            }
        }
    }

    float CalculateAccuracy(float centerX)
    {
        float dist = Mathf.Abs(transform.position.x - centerX);
        float maxDist = 2f;

        float accuracy = Mathf.Clamp01(1 - dist / maxDist);
        return accuracy;
    }

    void Miss()
    {
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;

        Destroy(gameObject, 3f);
    }
}