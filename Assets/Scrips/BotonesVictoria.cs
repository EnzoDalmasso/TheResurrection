 using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonesVictoria : MonoBehaviour
{

    public void NuevaPartida(string nombreEscena)
    {

        //Borrar progreso de checkpoints
        PlayerPrefs.DeleteKey("puntosIndex");
        PlayerPrefs.DeleteKey("sessionStarted");
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }

    //Menu Inicial

    public void MenuInicial(string nombreEscena)
    {
        Time.timeScale = 1f;//Restaurar tiempo por si estaba en pausa
        SceneManager.LoadScene(nombreEscena);
        
    }


    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void Salir()
    {

        Application.Quit();
    }
    
}


