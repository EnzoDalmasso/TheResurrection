
using UnityEngine;

public class Trampas : MonoBehaviour
{

    [SerializeField] private int danioTrampa = 100;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))//Verifica si la trampa colisiono con el player
        {


            //Busca en el scrip del jugador para poder pasarle el daño que genera la trampa
            PlayerController1 player = collision.GetComponent<PlayerController1>();

            if (player != null)
            {
                // Calculamos una dirección de impacto simple (hacia arriba por ejemplo)
                Vector2 direccionImpacto = (player.transform.position - transform.position).normalized;

                player.RecibirDaño(danioTrampa);
                
            }

        }
    }
}



