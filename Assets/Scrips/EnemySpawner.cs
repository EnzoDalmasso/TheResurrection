using UnityEngine;

public class EnemyTriggerSpawner : MonoBehaviour
{
    [Header("Prefabs de enemigos")]
    public GameObject[] enemyPrefabs;//Lista de prefabs de enemigos

    [Header("Posicion del spawn")]
    public Transform puntoSpawn;//Putno spawn del enemigo

    [Header("Configuracion del generador")]
    public int cantidadEnemigos = 1;//Cantidad de enemigos generar

    [Header("Desplazamiento horizontal de spawn")]
    public float rangoDesplazamientoX = 1f;//Cuanto puede variar la posición en el eje X

    private bool yaSpawneo = false;//Boleano para avisar que se spawneo 1 ves

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !yaSpawneo)
        {
            //Generamos los enemigos solo si no hay enemigos generados actualmente
            for (int i = 0; i < cantidadEnemigos; i++)
            {
                GenerarEnemigo();
            }
            yaSpawneo = true;
        }
        
    }

    private void GenerarEnemigo()
    {
        if (enemyPrefabs.Length == 0) return;

        //Selecciona un enemigo aleatorio
        int index = Random.Range(0, enemyPrefabs.Length);

        //Genera un desplazamiento aleatorio solo en el eje X
        float desplazamientoX = Random.Range(-rangoDesplazamientoX, rangoDesplazamientoX);

        //Nueva posicion para generar
        Vector3 posicionDesplazada = new Vector3(puntoSpawn.position.x + desplazamientoX, puntoSpawn.position.y, puntoSpawn.position.z);

        //Genera el enemigo en la nueva posicion
        Instantiate(enemyPrefabs[index], posicionDesplazada, Quaternion.identity);
    }
}