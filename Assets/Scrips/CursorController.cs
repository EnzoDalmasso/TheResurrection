using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorController : MonoBehaviour
{
    [Header("Texturas del cursor")]
    public Texture2D cursorMenu;     // Cursor en menús (ej: blanco)
    public Texture2D cursorJuego;    // Cursor en gameplay normal (ej: negro)
    public Texture2D cursorEnemy;    // Cursor al apuntar a enemigo (ej: rojo)

    [Header("Ajustes del cursor")]
    public Vector2 hotspot = Vector2.zero;

    // Control interno
    private string escenaActual;
    private bool enMenu = false;
    private bool enPausaOVictoria = false;
    private bool enJuego = false;
    private Texture2D cursorActual;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        ActualizarTipoDeCursor();
    }

    void Update()
    {
        if (enJuego && !enPausaOVictoria)
            DetectarApuntadoEnemigo();
    }

    // Se ejecuta cada vez que se carga una escena
    void OnSceneLoaded(Scene escena, LoadSceneMode modo)
    {
        escenaActual = escena.name;
        ActualizarTipoDeCursor();
    }

    void ActualizarTipoDeCursor()
    {
        // Detectar si la escena es de menú o de juego
        if (escenaActual.Contains("Menu")) // por ejemplo: "MenuInicial"
        {
            enMenu = true;
            enJuego = false;
            enPausaOVictoria = false;
            SetCursor(cursorMenu);
        }
        else
        {
            enMenu = false;
            enJuego = true;
            enPausaOVictoria = false;
            SetCursor(cursorJuego);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Detecta si el mouse apunta a un enemigo
    void DetectarApuntadoEnemigo()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider != null && hit.collider.CompareTag("Enemigo"))
        {
            if (cursorActual != cursorEnemy)
                SetCursor(cursorEnemy);
        }
        else
        {
            if (cursorActual != cursorJuego)
                SetCursor(cursorJuego);
        }
    }

    // --- Métodos públicos para cambiar el cursor desde otros scripts ---
    public void MostrarCursorMenu()
    {
        enPausaOVictoria = true;
        SetCursor(cursorMenu);
    }

    public void MostrarCursorJuego()
    {
        enPausaOVictoria = false;
        SetCursor(cursorJuego);
    }

    private void SetCursor(Texture2D cursor)
    {
        if (cursor == null) return;
        cursorActual = cursor;
        Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
    }
}
