using UnityEngine;
using UnityEngine.SceneManagement;

public class JuegoMenuManager : MonoBehaviour
{

    [Header("Paneles del juego")]
    public GameObject panelPausa;
    public GameObject panelControles;
    public GameObject panelVictoria;
    public GameObject panelDerrota;

    private bool juegoPausado = false;

    [SerializeField] private AudioSource musicaFondo;

    private GameObject canvasHUD; // referencia al HUD
    public static bool puedeDisparar = true;//No puede disparar en el menu el player
    private void Start()
    {
        OcultarTodosLosPaneles();
        //Busca el objeto HUD en la escena
        canvasHUD = GameObject.Find("BarrasEnergia");
    }


    //VERIFICACION SI EL PLAYER ESTA VIVO O MUERTO
    private void OnEnable()
    {
        PlayerController1 player = FindFirstObjectByType<PlayerController1>();
        if (player != null)
            player.MuerteJugador += OnMuerteJugador;
    }

    
    private void OnDisable()
    {
        PlayerController1 player = FindFirstObjectByType<PlayerController1>();
        if (player != null)
            player.MuerteJugador -= OnMuerteJugador;
    }

    //AL PRECIONAR ESCAPE ACTIVA O DESACTIVA LA PAUSA
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!panelPausa.activeSelf && !panelControles.activeSelf &&
                !panelVictoria.activeSelf && !panelDerrota.activeSelf)
            {
                PausarJuego();
            }
            else if (panelPausa.activeSelf)
            {
                ReanudarJuego();
            }
        }
    }

    //OCULTA LOS PANELES
    private void OcultarTodosLosPaneles()
    {
        panelPausa.SetActive(false);
        panelControles.SetActive(false);
        panelVictoria.SetActive(false);
        panelDerrota.SetActive(false);

        Time.timeScale = 1f;
        juegoPausado = false;

        // Restaurar permiso de disparo al iniciar/ocultar menús para que
        // PlayerController1.Update() siga actualizando el Animator.
        JuegoMenuManager.puedeDisparar = true;

    }

    // PAUSA
    public void PausarJuego()
    {
        panelPausa.SetActive(true);
        Time.timeScale = 0f;
        juegoPausado = true;
        JuegoMenuManager.puedeDisparar = false;

        if (canvasHUD != null)
        {
            canvasHUD.SetActive(false);
        }

        if (ControladorSonido.instance != null)
        {
            ControladorSonido.instance.mutearSonido();
        }

        //Cambio cursor al de menu
        CursorController cambioCursor = FindFirstObjectByType<CursorController>();
        if (cambioCursor != null)
        {
            cambioCursor.MostrarCursorMenu();
        }

    }

    //REANUDA EL JUEGO
    public void ReanudarJuego()
    {
        panelPausa.SetActive(false);
        panelControles.SetActive(false);
        Time.timeScale = 1f;
        juegoPausado = false;
        JuegoMenuManager.puedeDisparar = true;

        if (canvasHUD != null)
        {
            canvasHUD.SetActive(true);
        }
        if (ControladorSonido.instance != null)
        {
            ControladorSonido.instance.desmutearSonido();
        }
        CursorController cambioCursor = FindFirstObjectByType<CursorController>();
        if (cambioCursor != null)
        {
            cambioCursor.MostrarCursorJuego();
        }

    }

    //PANEL DE CONTROLES
    public void MostrarControles()
    {
        panelPausa.SetActive(false);
        panelControles.SetActive(true);
        JuegoMenuManager.puedeDisparar = false;

        CursorController cambioCursor = FindFirstObjectByType<CursorController>();
        if (cambioCursor != null)
        {
            cambioCursor.MostrarCursorMenu();
        }

    }

    
    public void VolverDesdeControles()
    {
        panelControles.SetActive(false);
        panelPausa.SetActive(true);
        JuegoMenuManager.puedeDisparar = false;
    }

    //MUESTRA PANEL DE VICTORIA
    public void MostrarVictoria()
    {
        OcultarTodosLosPaneles();
        panelVictoria.SetActive(true);
        Time.timeScale = 0f;
        JuegoMenuManager.puedeDisparar = false;

        if (canvasHUD != null)
        {
            canvasHUD.SetActive(false);
        }

        if (musicaFondo != null)
        {
            musicaFondo.Pause(); //Pausa la musica de fondo
        }

        //Mostramos cursor de menu
        CursorController cambioCursor = FindFirstObjectByType<CursorController>();
        if (cambioCursor != null)
        { cambioCursor.MostrarCursorMenu(); }
    }

    //MUESTRA PANEL DE DERROTA
    public void MostrarDerrota()
    {
        OcultarTodosLosPaneles();
        panelDerrota.SetActive(true);
        Time.timeScale = 0f;
        JuegoMenuManager.puedeDisparar = false;

        if (canvasHUD != null)
        {
            canvasHUD.SetActive(false);
        }

        if (musicaFondo != null)
        {
            musicaFondo.Pause(); //Pausa la musica de fondo
        }
        CursorController cambioCursor = FindFirstObjectByType<CursorController>();
        if (cambioCursor != null)
        { cambioCursor.MostrarCursorMenu(); }
    }

    //RECIBE EVENTO DE MUERTE DEL PLAYER
    public void OnMuerteJugador(object sender, System.EventArgs e)
    {
        MostrarDerrota();
    }

    //Siguiente Nivel
    public void SiguienteNivel(string nombreSiguienteEscena)
    {
        //Reactivar el tiempo por si estaba pausado
        Time.timeScale = 1f;

        //Borrar progreso de checkpoints
        PlayerPrefs.DeleteKey("puntosIndex");
        PlayerPrefs.DeleteKey("sessionStarted");
        PlayerPrefs.Save();

        //Cargar siguiente nivel
        SceneManager.LoadScene(nombreSiguienteEscena);
    }

    
    //REINICIAMOS NIVEL
    public void ReiniciarNivel()
    {
  
        Time.timeScale = 1f;

        if (ControladorSonido.instance != null)
        {
            ControladorSonido.instance.desmutearSonido();
        }

        // Elimina cualquier referencia vieja del jugador
        PlayerController1 player = FindFirstObjectByType<PlayerController1>();
        if (player != null)
        {
            player.estaVivo = true;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //VOLVEMOS AL MENU INICIAL
    public void VolverAlMenuInicial(string nombreEscenaMenu)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    public void SalirJuego()
    {
        Application.Quit();
    }
}