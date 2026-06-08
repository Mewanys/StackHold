using UnityEngine;

public class BuildingBlock : MonoBehaviour
{
    //private BuildingManager manager;
    //private Transform validBase; // Ссылка на фундамент текущей стройки
    //private bool hasLanded = false;

    //public void Initialize(BuildingManager mgr, Transform foundation)
    //{
    //    manager = mgr;
    //    validBase = foundation;
    //}

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (hasLanded) return;

    //    // Проверяем, во что врезались. 
    //    // Это должно быть либо "Base" (фундамент), либо другой блок "BuildingBlock"
    //    // Важно: у фундамента должен быть тег "Base" или компонент BuildingBlock (если вы так решите),
    //    // но лучше проверять через Transform parent.

    //    bool hitBase = collision.transform == validBase;
    //    bool hitBlock = collision.gameObject.GetComponent<BuildingBlock>() != null;

    //    if (hitBase || hitBlock)
    //    {
    //        // Успешное попадание
    //        hasLanded = true;
    //        manager.OnBlockLanded(this, collision.transform);
    //    }
    //    else
    //    {
    //        // Врезались в землю (Terrain) или что-то левое -> Промах
    //        // Можно сразу уничтожать, либо дать упасть ниже
    //    }
    //}

    //void Update()
    //{
    //    if (hasLanded) return;

    //    // Если блок упал ниже фундамента на 5 единиц — считаем промахом и удаляем
    //    if (validBase != null && transform.position.y < validBase.position.y - 5f)
    //    {
    //        manager.OnBlockMissed();
    //        Destroy(gameObject);
    //    }
    //}
}