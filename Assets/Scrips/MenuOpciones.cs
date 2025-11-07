using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
public class MenuOpciones : MonoBehaviour
{
    //REFERENCIAS DEL MENU
    [Header("Referencias")]
    [SerializeField] private Slider sliderVolumen;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    //VARIABLES
    [Header("Variables Internas")]
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private float currentRefreshRate;
    private int currentResolutionIndex = 0;
    private void Start()
    {
        // Verificar si el slider estA asignado en el Inspector
        if (sliderVolumen == null)
        {
            return; //Salimos para evitar errores
        }

        // Cargar el volumen guardado (por defecto 0.75f)
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMusica", 0.75f);
        sliderVolumen.value = volumenGuardado;//Ajusta la posicion del slider
        CambiarVolumen(volumenGuardado);//Aplica el volumen inicial

        //RESOLUCIOES
        resolutions = Screen.resolutions;//Obtiene todas las resoluciones que soporta el monitor
        filteredResolutions = new List<Resolution>();//Crea una lista vacia para almacenar las filtradas

        resolutionDropdown.ClearOptions();//Guarda la tasa de refresco actual
        currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;

        //Filtrar resoluciones que tengan la misma tasa de refresco que la actual
        for (int i = 0; i < resolutions.Length; i++)
        {
            if ((float)resolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        //Ordenaa las resoluciones filtradas por ancho y alto (de mayor a menor)
        filteredResolutions.Sort((a, b) => {
            if (a.width != b.width)
                return b.width.CompareTo(a.width);//Primero el ancho
            else
                return b.height.CompareTo(a.height);//Luego el alto
        });

        //Crea una lista de texto para mostrar en el dropdown
        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            //Formato:1920x1080 60 Hz
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " + filteredResolutions[i].refreshRateRatio.value.ToString("0.##") + " Hz"; // Ondalık basamak sınırlandı
            options.Add(resolutionOption);

            //Comprueba si esta resolucion coincide con la resolucion actual de la pantalla
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height && (float)filteredResolutions[i].refreshRateRatio.value == currentRefreshRate) // double'dan float'a dönüştürüldü
            {
                currentResolutionIndex = i;//Guarda el indice actual
            }
        }

        resolutionDropdown.AddOptions(options);//Cargar opciones en el dropdown

        //Selecciona por defecto la primera resolucion (o la actual)
        resolutionDropdown.value = currentResolutionIndex = 0;
        resolutionDropdown.RefreshShownValue();

        //Aplica la resolucion inicial
        SetResolution(currentResolutionIndex);

    }

    //ACTIVA O DESACTIVA LA PANTALLA COMPLETA
    public void PantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
    }

    //CAMBIA EL VOLUMEN GENERAL DEL JUEGO
    public void CambiarVolumen(float volumen)
    {
        //Llama al controlador del sonido del juego si existe
        if (ControladorSonido.instance != null)
            ControladorSonido.instance.CambiarVolumen(volumen);
    }


    // Vuelve al menu principal
    public void VolverMenu(string nombreEscena)
    {

        SceneManager.LoadScene(nombreEscena);
    }

    //Cambia la resolucion de pantalla al indice seleccionado
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
    }


    //Cambia la calidad grafica del juego segun el indice del dropdown
    public void cambiarCalidad(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
}