
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuInicial : MonoBehaviour
{


    private void Start()
    {
        Debug.Log("MenuInicial activo");
    }
    //MenuCreditos

    public void menuCreditos(string nombreEscena)
    {

        SceneManager.LoadScene(nombreEscena);
    }


    //Tutorial
    public void Juego()
    {
        Debug.Log("Jugar");
        // Llama a la transicion
        TransicionEscenasUI.instance.DisolverSalida("Juego");
    }


    //Menu Opciones
    public void MenuOpciones(string nombreEscena)
    {

        SceneManager.LoadScene(nombreEscena);
    }

    public void MenuControles(string nombreEscena)
    {  
       SceneManager.LoadScene(nombreEscena);
    }
    public void Salir()
    {
      
        Application.Quit();
    }
}
