using UnityEngine;

public class Mana : MonoBehaviour
{
    [SerializeField]private int cantidadMana = 1; // Cada mana suma 1 "carga" de maná
    //Sonido agarre de mana
    [SerializeField] private AudioClip manaSonido;


    //Si el player colisiona con el mana va aumentando la barra de poder y se destruye
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController1 playerController = other.GetComponent<PlayerController1>();
            if (playerController != null)
            {
                
                if (manaSonido != null)
                {
                    
                    ControladorSonido.instance.EjecutarSonido(manaSonido, 1f);
                    playerController.AgregarMana(cantidadMana);
                    
                }
            }
            
            Destroy(gameObject); // Destruye el objeto de maná al recogerlo
        }
    }
}
