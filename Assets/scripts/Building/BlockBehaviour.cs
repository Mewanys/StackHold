using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BlockBehaviour : MonoBehaviour
{
    // Событие: блок коснулся опоры (платформы или другого блока)
    public Action<BlockBehaviour> OnBlockLanded;
    // Событие: блок упал в KillZone (промахнулся ИЛИ соскользнул)
    public Action<BlockBehaviour> OnBlockFell;

    private Rigidbody rb;
    private bool hasProcessedLanded = false; // Флаг для предотвращения многократного вызова Landed
    private bool isLanded = false; // Флаг: блок коснулся опоры

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Изначально блок висит на кране
    }

    public void Drop()
    {
        rb.isKinematic = false;
        transform.SetParent(null); // Отцепляем от крана
    }

    public void Freeze()
    {
        rb.isKinematic = true; // Для фиксации блоков
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasProcessedLanded) return;

        // Если коснулись предыдущего блока или стартовой платформы
        if (collision.gameObject.CompareTag("Block") || collision.gameObject.CompareTag("BasePlatform"))
        {
            hasProcessedLanded = true;
            isLanded = true;
            this.gameObject.tag = "Block"; // Назначаем тег, чтобы следующие блоки могли на него падать

            // Сообщаем менеджеру, что блок коснулся опоры
            OnBlockLanded?.Invoke(this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Если блок попал в KillZone (упал вниз)
        if (other.CompareTag("KillZone"))
        {
            // Сообщаем менеджеру, что блок упал (менеджер сам решит, был ли это промах или соскальзывание)
            OnBlockFell?.Invoke(this);

            // Уничтожаем блок немедленно, чтобы он не мешал
            Destroy(gameObject);
        }
    }
}