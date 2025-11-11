using UnityEngine;

public class FondoMovimiento : MonoBehaviour
{
    [SerializeField] private Vector2 velMovimiento;//Velocidad de movimiento
    private Vector2 offset;//Sirve para almacenar el desplazamiento del fondo
    private Material material;//Material para el sprite
    private Rigidbody2D jugadorRB;
    private bool seguirJugador = true;

    private void Awake()
    {
        // Busca SpriteRenderer en este objeto o sus hijos
        var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            material = spriteRenderer.material;
        }
           
        else
        {
           
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                jugadorRB = player.GetComponent<Rigidbody2D>();
            }
                
            else
            {
                return;
            } 
        }
           
    }

    private void Update()
    {
        // Si el jugador murio y reaparecio, volvemos a buscarlo
        if (jugadorRB == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                jugadorRB = player.GetComponent<Rigidbody2D>();
                
            }
        }

        if (!seguirJugador || jugadorRB == null || material == null) return;

        offset = (jugadorRB.linearVelocity.x * 0.1f) * velMovimiento * Time.deltaTime;
        material.mainTextureOffset += offset;
    }

    public void DesactivarSeguimientoJugador()
    {
        seguirJugador = false;
    }
}
