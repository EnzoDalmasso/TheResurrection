using UnityEngine;
using UnityEngine.Playables; //Necesario para controlar Timeline


public class ActivarCinematica : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayableDirector cinematica;//Cinematica
    [SerializeField] private GameObject camaraJuego;//CamaraJuego
    [SerializeField] private GameObject camaraCinematica;//Camara Cinematica

    private GameObject player;
    private PlayerController1 playerController;
    private Rigidbody2D rb;

    private GameObject canvasHUD; // referencia al HUD
    private void Start()
    {
        Invoke(nameof(ObtenerJugador), 0.2f);
        //Busca el objeto HUD en la escena
        canvasHUD = GameObject.Find("BarrasEnergia");
    }

 
    //Obtenemos al Player
    private void ObtenerJugador()
    {
        player = FindAnyObjectByType<PlayerController1>()?.gameObject;
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController1>();
            rb = player.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && cinematica != null)
        {
            //Desactivar movimiento sin desactivar el script
            if (playerController != null)
            {
                playerController.SetPuedeMoverse(false);
                playerController.ForzarIdle(true); //Fuerza animacion idle
            }

            //Detenenemos su velocidad para que quede quieto
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }



            // Desactiva HUD (Canvas del juego)
            if (canvasHUD != null)
            {
                canvasHUD.SetActive(false);
            }

            //Activa camara de cinematica y desactiva la de juego
            if (camaraJuego != null)
            {

                camaraJuego.SetActive(false);
                
            }
       
            if (camaraCinematica != null)

            {
                camaraCinematica.SetActive(true);
             

            }


            //Reproducir cinematica
            cinematica.Play();
            cinematica.stopped += CinematicaTerminada;
        }
    }

    private void CinematicaTerminada(PlayableDirector director)
    {
        if (playerController != null)
        {
            playerController.SetPuedeMoverse(true);
            playerController.ForzarIdle(false);//Restablece control normal
        }

        // Reactiva el HUD
        if (canvasHUD != null)
            canvasHUD.SetActive(true);

        //Activa camara de juego y desactiva la de cinematica
        if (camaraJuego != null)
        {
            camaraJuego.SetActive(true);
            if (canvasHUD != null)
            {
                canvasHUD.SetActive(true);
            }

        }

        if (camaraCinematica != null)
        {
            camaraCinematica.SetActive(false);
        }
            

        cinematica.stopped -= CinematicaTerminada;
        gameObject.SetActive(false);
    }
}
