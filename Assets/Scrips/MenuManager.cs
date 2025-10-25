using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles del menu")]
    public GameObject panelMenuPrincipal;
    public GameObject panelControles;
    public GameObject panelOpciones;
    public GameObject panelCreditos;

    [Header("Transición")]
    public TransicionEscenasUI transicion; //asignar en Inspector

    private void Start()
    {
        MostrarMenuPrincipal();
    }

    //MENUS
    public void MostrarMenuPrincipal()
    {
        panelMenuPrincipal.SetActive(true);
        panelControles.SetActive(false);
        panelOpciones.SetActive(false);
        panelCreditos.SetActive(false);
    }

    public void MostrarControles()
    {
        if (transicion != null)
        {
            transicion.DisolverLocal(() =>
            {
                panelMenuPrincipal.SetActive(false);
                panelControles.SetActive(true);
            });
        }
        else
        {
            panelMenuPrincipal.SetActive(false);
            panelControles.SetActive(true);
        }
    }

    public void MostrarOpciones()
    {
        if (transicion != null)
        {
            transicion.DisolverLocal(() =>
            {
                panelMenuPrincipal.SetActive(false);
                panelOpciones.SetActive(true);
            });
        }
        else
        {
            panelMenuPrincipal.SetActive(false);
            panelOpciones.SetActive(true);
        }
    }

    public void MostrarCreditos()
    {
        if (transicion != null)
        {
            transicion.DisolverLocal(() =>
            {
                panelMenuPrincipal.SetActive(false);
                panelCreditos.SetActive(true);
            });
        }
        else
        {
            panelMenuPrincipal.SetActive(false);
            panelCreditos.SetActive(true);
        }
    }

    public void VolverAlMenu()
    {
        if (transicion == null)
            transicion = TransicionEscenasUI.instance;

        

        if (transicion != null)
        {
            
            transicion.DisolverLocal(() =>
            {
                panelControles.SetActive(false);
                panelOpciones.SetActive(false);
                panelCreditos.SetActive(false);
                panelMenuPrincipal.SetActive(true);
            });
        }
        else
        {
            Debug.Log("Transicion es NULL");
            MostrarMenuPrincipal();
        }
    }

    //JUGAR
    public void Jugar(string nombreEscena)
    {
        if (transicion != null)
            transicion.DisolverSalida(nombreEscena);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscena);
    }

    //SALIR
    public void SalirJuego()
    {
        if (transicion != null)
            transicion.DisolverLocal(() => { Application.Quit(); });
        else
            Application.Quit();
    }
}
