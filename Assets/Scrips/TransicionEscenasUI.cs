using UnityEngine;
using UnityEngine.SceneManagement;

public class TransicionEscenasUI : MonoBehaviour
{
    public static TransicionEscenasUI instance;//Singlenton de la transcion


    public CanvasGroup disolverGroup;//Canvas para la transición
    public float tiempoDisolverEntrada;//Tiempo que dura la pantalla en negro
    public float tiempoDisolverSalida;//Tiempo que dura en quitar la pantalla en negro


    private void Awake()
    {
        //Si no ha instancia se crea, caso contrario se destruye
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);

        }
    }

    void Start()
    {
        DisolverEntrada();
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
        DisolverEntrada(); // Se ejecuta al cargar una nueva escena
    }

    //Esto hace que el disolverGroup (el panel negro del UI) se desvanezca gradualmente hasta una transparencia total (alpha = 0) en el tiempo tiempoDisolverEntrada.
    private void DisolverEntrada()
    {
        LeanTween.alphaCanvas(disolverGroup, 0f, tiempoDisolverEntrada).setOnComplete(() =>
        {
            disolverGroup.blocksRaycasts = false;//El panel deja de blockear clicks
            disolverGroup.interactable = false;//No se puede interactuar
        });
    }

    //Funcion opuesta al Disolver
    public void DisolverSalida(string nombreEscena)
    {
        disolverGroup.alpha = 1f; //aseguramos que empiece en negro
        disolverGroup.blocksRaycasts = true;
        disolverGroup.interactable = true;

        LeanTween.alphaCanvas(disolverGroup, 1f, tiempoDisolverSalida).setOnComplete(() =>
        {

            SceneManager.LoadScene(nombreEscena);
        });
    }
}
