using UnityEngine;

public class PlacaPresion : MonoBehaviour
{
    [SerializeField] private GameObject puerta;//Referencia a la puerta
    [SerializeField] private float alturaApertura;//Altura de apertura puerta en Y
    [SerializeField] private float velocidadApertura = 2f;//Velocidad de apertura

    //Sonido puerta y boton
    [SerializeField] private AudioClip botonPresionadoSonido;
    [SerializeField] private AudioClip levantamientoPueraSonido;

    private Animator pressPlaca;

    private Vector2 posicionPuertaCerrada;//Posicion inicial de la puerta
    private Vector2 posicionPuertaAbierta; //Posicion de la puerta abierta

    private bool activado = false;//Indica si la placa esta presionada o no
    private bool sonidoPuertaReproducido = false; //Evita que el sonido se repita

    private void Start()
    {
        pressPlaca = GetComponent<Animator>();

        if (puerta != null)
        {
            // Guardamos la posicion inicial (cerrada)
            posicionPuertaCerrada = puerta.transform.position;

            // Calculamos la posición abierta solo en Y
            posicionPuertaAbierta = new Vector2(posicionPuertaCerrada.x, posicionPuertaCerrada.y + alturaApertura);
        }
    }

    //Detecta si la "Caja" es presionada
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Caja"))
        {
            if (botonPresionadoSonido != null)
                ControladorSonido.instance.EjecutarSonido(botonPresionadoSonido, 1f);

            activado = true;
            pressPlaca.SetBool("Press", true);

            //Reseteamos para permitir reproducir el sonido de levantamiento
            sonidoPuertaReproducido = false;
        }
    }

    //Detecta si la "Caja" no es presionada
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Caja") )
        {
            if (botonPresionadoSonido != null)
                ControladorSonido.instance.EjecutarSonido(botonPresionadoSonido, 1f);

            activado = false;
            pressPlaca.SetBool("Press", false);

            //Permitimos que vuelva a sonar cuando la puerta se abra de nuevo
            sonidoPuertaReproducido = false;
        }
    }


    private void Update()
    {
        if (puerta == null) return;

        Vector2 destino;

        if (activado)
        {
            destino = posicionPuertaAbierta;

            //Sonido cuando la puerta empieza a levantarse
            if (!sonidoPuertaReproducido && puerta.transform.position.y < posicionPuertaAbierta.y)
            {
                if (levantamientoPueraSonido != null)
                {
                    ControladorSonido.instance.EjecutarSonido(levantamientoPueraSonido, 1f);
                }
                   

                sonidoPuertaReproducido = true;
            }
        }
        else
        {
            destino = posicionPuertaCerrada;

            //Resetea para que suene de nuevo la proxima vez que se abra
            sonidoPuertaReproducido = false;
        }

        // Movemos suavemente la puerta solo en Y
        puerta.transform.position = Vector2.MoveTowards((Vector2)puerta.transform.position,destino,velocidadApertura * Time.deltaTime);
    }

}
