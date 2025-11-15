using UnityEngine;

public class PlacaPresPlayer : MonoBehaviour
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

    //Detecta si el Player presiona
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BolaMagica") || other.CompareTag("Player"))
        {
            if (botonPresionadoSonido != null)
                ControladorSonido.instance.EjecutarSonido(botonPresionadoSonido, 1f);

            activado = true;
            pressPlaca.SetBool("Press", true);
            
            //Reseteamos para permitir reproducir el sonido de levantamiento
            sonidoPuertaReproducido = false;
            GetComponent<Collider2D>().enabled = false;
        }
    }

   
    private void Update()
    {
        if (puerta == null) return;

        Vector2 destino;

        if (activado)
        {
            destino = posicionPuertaAbierta;

            
            // Sonido al comenzar a abrir la puerta (solo una vez)
            if (!sonidoPuertaReproducido && activado &&
                Vector2.Distance(puerta.transform.position, posicionPuertaCerrada) < 0.1f)
            {
                if (levantamientoPueraSonido != null)
                    ControladorSonido.instance.EjecutarSonido(levantamientoPueraSonido, 1f);

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
        puerta.transform.position = Vector2.MoveTowards((Vector2)puerta.transform.position, destino, velocidadApertura * Time.deltaTime);
    }


}
