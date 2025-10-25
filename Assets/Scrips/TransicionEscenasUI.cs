using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Este script requiere LeanTween en el proyecto y un CanvasGroup asignado (imagen negra sobre el Canvas)
public class TransicionEscenasUI : MonoBehaviour
{
    public static TransicionEscenasUI instance;

    [Header("Canvas Group para la transición (imagen negra)")]
    public CanvasGroup disolverGroup;

    [Header("Tiempos (segundos)")]
    public float tiempoDisolverEntrada = 0.6f; // tiempo para volver a transparente
    public float tiempoDisolverSalida = 0.6f;  // tiempo para oscurecer antes de cambiar

    private void Awake()
    {
        // Singleton básico
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Estado inicial: pantalla negra bloqueando la UI hasta que se disuelva
        if (disolverGroup != null)
        {
            disolverGroup.alpha = 1f;
            disolverGroup.blocksRaycasts = true;
            disolverGroup.interactable = true;
        }
    }

    private void Start()
    {
        // Al iniciar, hacemos el FadeIn
        DisolverEntrada();

        // Seguridad extra: liberamos raycasts
        if (disolverGroup != null)
        {
            disolverGroup.blocksRaycasts = false;
            disolverGroup.interactable = false;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cuando se carga una escena, hacemos el FadeIn
        DisolverEntrada();

        // Seguridad: liberamos raycasts por si queda bloqueado tras la carga
        if (disolverGroup != null)
        {
            disolverGroup.blocksRaycasts = false;
            disolverGroup.interactable = false;
        }
    }

    // --- MÉTODOS PÚBLICOS ---

    // FadeIn: quitar el panel negro (de 1 -> 0)
    public void DisolverEntrada()
    {
        if (disolverGroup == null) return;

        disolverGroup.blocksRaycasts = true;
        disolverGroup.interactable = true;

        LeanTween.cancel(disolverGroup.gameObject);
        LeanTween.alphaCanvas(disolverGroup, 0f, tiempoDisolverEntrada).setOnComplete(() =>
        {
            disolverGroup.blocksRaycasts = false;
            disolverGroup.interactable = false;
        });
    }

    // FadeOut: oscurece y cambia de escena
    public void DisolverSalida(string nombreEscena)
    {
        if (disolverGroup == null)
        {
            SceneManager.LoadScene(nombreEscena);
            return;
        }

        disolverGroup.alpha = 0f;
        disolverGroup.blocksRaycasts = true;
        disolverGroup.interactable = true;

        LeanTween.cancel(disolverGroup.gameObject);
        LeanTween.alphaCanvas(disolverGroup, 1f, tiempoDisolverSalida).setOnComplete(() =>
        {
            SceneManager.LoadScene(nombreEscena);
        });
    }

    // Transición local entre paneles: oscurece -> ejecuta acción -> aclara
    public void DisolverLocal(Action onMidpoint = null)
    {
        if (disolverGroup == null)
        {
            onMidpoint?.Invoke();
            return;
        }

        disolverGroup.alpha = 0f;
        disolverGroup.blocksRaycasts = true;
        disolverGroup.interactable = true;

        LeanTween.cancel(disolverGroup.gameObject);

        // Oscurecer
        LeanTween.alphaCanvas(disolverGroup, 1f, tiempoDisolverSalida * 0.5f).setOnComplete(() =>
        {
            onMidpoint?.Invoke();

            // Aclarar
            LeanTween.alphaCanvas(disolverGroup, 0f, tiempoDisolverEntrada * 0.5f).setOnComplete(() =>
            {
                disolverGroup.blocksRaycasts = false;
                disolverGroup.interactable = false;
            });
        });
    }
}
