
using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonesMenu : MonoBehaviour
{
    PlayerController1 playerController;
    //Reinicia el nivel actual
    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        


    }

    //Te dirige al menuPrincipal
    public void IrAlMenu(string nombreEscena)
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(nombreEscena);

        
    }


    
    public void Salir()
    {
        Application.Quit();
        Debug.Log("QUIT");
    }
}

