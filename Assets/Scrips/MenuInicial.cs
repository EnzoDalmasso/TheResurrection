
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuInicial : MonoBehaviour
{
    
    //MenuCreditos

    public void menuCreditos(string nombreEscena)
    {

        SceneManager.LoadScene(nombreEscena);
    }


    //Tutorial
    public void TutorialJuego()
    {
        // Llama a la transicion
        TransicionEscenasUI.instance.DisolverSalida("Tutorial");
    }


    //Menu Opciones
    public void MenuOpciones(string nombreEscena)
    {

        SceneManager.LoadScene(nombreEscena);
    }


    public void Salir()
    {
      
        Application.Quit();
    }
}
