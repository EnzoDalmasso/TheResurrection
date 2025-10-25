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

    private void Start()
    {
        OcultarTodosLosPaneles();
    }

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

    private void OcultarTodosLosPaneles()
    {
        panelPausa.SetActive(false);
        panelControles.SetActive(false);
        panelVictoria.SetActive(false);
        panelDerrota.SetActive(false);

        Time.timeScale = 1f;
        juegoPausado = false;
    }

    // ===================
    // PAUSA
    // ===================
    public void PausarJuego()
    {
        panelPausa.SetActive(true);
        Time.timeScale = 0f;
        juegoPausado = true;
    }

    public void ReanudarJuego()
    {
        panelPausa.SetActive(false);
        panelControles.SetActive(false);
        Time.timeScale = 1f;
        juegoPausado = false;
    }

    // ===================
    // CONTROLES
    // ===================
    public void MostrarControles()
    {
        panelPausa.SetActive(false);
        panelControles.SetActive(true);
    }

    public void VolverDesdeControles()
    {
        panelControles.SetActive(false);
        panelPausa.SetActive(true);
    }

    // ===================
    // VICTORIA / DERROTA
    // ===================
    public void MostrarVictoria()
    {
        OcultarTodosLosPaneles();
        panelVictoria.SetActive(true);
        Time.timeScale = 0f;
    }

    public void MostrarDerrota()
    {
        OcultarTodosLosPaneles();
        panelDerrota.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnMuerteJugador(object sender, System.EventArgs e)
    {
        MostrarDerrota();
    }

    // ===================
    // BOTONES
    // ===================
    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

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