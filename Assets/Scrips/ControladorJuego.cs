using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorJuego : MonoBehaviour
{

    // Patron singleton
    public static ControladorJuego Instance;

    [SerializeField] private GameObject[] puntosDeControl;//Puntos de control en la escena
    [SerializeField] private GameObject jugador;//Prefab del jugador
    public GameObject[] PuntosDeControl => puntosDeControl;

    //Sonido checkpoint
    [SerializeField] private AudioClip checkPointSonido;

    private int indexPuntosControl; //Ultimo checkpoint alcanzado

  

    private void Awake()
    {
        
        
        Instance = this;

        //Cuando se inicia por primera vez arranca del Inicio
        if (!PlayerPrefs.HasKey("sessionStarted"))
        {
            PlayerPrefs.SetInt("puntosIndex", 0);
            PlayerPrefs.SetInt("sessionStarted", 1);
            PlayerPrefs.Save();
        }

        //Recuperamos el checkpoint guardado si llegamos a perder
        indexPuntosControl = PlayerPrefs.GetInt("puntosIndex", 0);

        //Instanciamos al jugador en ese punto
        GameObject nuevoJugador = Instantiate(jugador,puntosDeControl[indexPuntosControl].transform.position,Quaternion.identity);

        //Asignamos el jugador al CinemachineVirtualCamera
        var cam = FindAnyObjectByType<CinemachineVirtualCamera>();
        if (cam != null)
        {
            cam.Follow = nuevoJugador.transform;
        }

        var menu = FindAnyObjectByType<JuegoMenuManager>();
        if (menu != null)
        {
            PlayerController1 playerScript = nuevoJugador.GetComponent<PlayerController1>();
            playerScript.MuerteJugador += menu.OnMuerteJugador;
        }


    }

    //Este metodo llama cada punto de control
    public void UltimoPuntoControl(GameObject puntoControl)
    {
        

        for (int i = 0; i < puntosDeControl.Length; i++)
        {
            if (puntosDeControl[i] == puntoControl && i > indexPuntosControl)
            {
                indexPuntosControl = i;
                PlayerPrefs.SetInt("puntosIndex", indexPuntosControl);
                PlayerPrefs.Save();
                //Reproducir sonido de checkpoint
                if (checkPointSonido != null)
                {
                    ControladorSonido.instance.EjecutarSonido(checkPointSonido, 1f);
                }
            }
        }
    }

    private void Update()
    {
        //Recarga el ultimo checkpoint al presionar la R
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
