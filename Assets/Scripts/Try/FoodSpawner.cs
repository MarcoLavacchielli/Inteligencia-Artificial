using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject prefabToSpawn;

    [SerializeField]
    Bounds spawnBounds;

    [SerializeField]
    float spawnInterval = 7f;  //tiempo de espera, esta en 7 pero mepa que es medio cortina negro, fijate de subirlo o de achicar el escenario

    private void Start()
    {
        StartCoroutine(SpawnPrefabRoutine());
    }

    IEnumerator SpawnPrefabRoutine()
    {
        while (true)
        {
            SpawnPrefab();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnPrefab()
    {
        // Obtén una posición aleatoria dentro de los límites especificados
        Vector3 randomPosition = GetRandomPosition();

        // Instancia el prefab en la posición generada
        Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);
    }

    Vector3 GetRandomPosition()
    {
        // Genera una posición aleatoria dentro de los límites especificados
        float randomX = Random.Range(spawnBounds.min.x, spawnBounds.max.x);
        float randomY = Random.Range(spawnBounds.min.y, spawnBounds.max.y);
        float randomZ = Random.Range(spawnBounds.min.z, spawnBounds.max.z);

        return new Vector3(randomX, randomY, randomZ);
    }
}
