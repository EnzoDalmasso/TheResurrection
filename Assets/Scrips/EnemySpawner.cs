using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyTriggerSpawner : MonoBehaviour
{
    [Header("Prefabs de enemigos")]
    public GameObject[] enemyPrefabs; //Lista de prefabs de enemigos

    [Header("Posición de spawn")]
    public Transform puntoSpawn; //Donde aparece el enemigo

    private GameObject enemigoActual; //Referencia al enemigo generado

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemigoActual == null) //Solo genera si no hay enemigo
            {
                GenerarEnemigo();
            }
        }
    }

    private void GenerarEnemigo()
    {
        if (enemyPrefabs.Length == 0) return;

        int index = Random.Range(0, enemyPrefabs.Length);

        enemigoActual = Instantiate(enemyPrefabs[index], puntoSpawn.position, Quaternion.identity);
    }
}