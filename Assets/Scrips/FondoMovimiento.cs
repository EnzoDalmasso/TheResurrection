using UnityEngine;

public class FondoMovimiento : MonoBehaviour
{
    [SerializeField] private Vector2 velMovimiento;//Velocidad de movimiento
    private Vector2 offset;//Sirve para almacenar el desplazamiento del fondo
    private Material material;//Material para el sprite
    private Rigidbody2D jugadorRB;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;//Obtenemos el material
        jugadorRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        offset = (jugadorRB.linearVelocity.x * 0.1f)*velMovimiento * Time.deltaTime;// calcula el desplazamiento del offset y lo multiplica
        material.mainTextureOffset += offset; // modifica el maintexture del material
    }
}
