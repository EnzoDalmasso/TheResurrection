 
using UnityEngine;

public class Corazon : MonoBehaviour
{

    private int cantidaVida = 25; // Cada corazon suma 25 de vida
    //Sonido agarre de corazon
    [SerializeField] private AudioClip corazonSonido;




    //Si el player colisiona con el mana va aumentando la barra de poder y se destruye
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController1 playerController = other.GetComponent<PlayerController1>();
            if (playerController != null)
            {

                if (corazonSonido != null)
                {
                    ControladorSonido.instance.EjecutarSonido(corazonSonido, 1f);
                     playerController.AgregarVida(cantidaVida);
                }
               

            }

            Destroy(gameObject); // Destruye el objeto de maná al recogerlo
        }
    }

}
