using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MenuOpciones : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private Slider sliderVolumen;

    [Header("Pantalla")]
    [SerializeField] private Toggle togglePantallaCompleta;
    [SerializeField] private TMP_Dropdown resolucionesDropDown;

    [Header("Calidad")]
    [SerializeField] private TMP_Dropdown dropdownCalidad;

    private Resolution[] resoluciones;

    private void Start()
    {
        // --- AUDIO ---
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMusica", 0.75f);
        sliderVolumen.value = volumenGuardado;
        CambiarVolumen(volumenGuardado);

        // --- CALIDAD ---
        int calidadGuardada = PlayerPrefs.GetInt("numeroCalidad", 3);
        dropdownCalidad.value = calidadGuardada;
        AjustarCalidad(calidadGuardada);

        // --- PANTALLA ---
        togglePantallaCompleta.isOn = Screen.fullScreen;

        // Cargar resoluciones y aplicar la guardada
        RevisarResoluciones();
    }

    public void CambiarVolumen(float volumen)
    {
        if (ControladorSonido.instance != null)
            ControladorSonido.instance.CambiarVolumen(volumen);

        PlayerPrefs.SetFloat("VolumenMusica", volumen);
    }

    public void AjustarCalidad(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("numeroCalidad", index);
    }

    public void PantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
    }

    public void RevisarResoluciones()
    {
        resoluciones = Screen.resolutions;
        resolucionesDropDown.ClearOptions();

        List<string> opciones = new List<string>();
        int resolucionActual = 0;

        for (int i = 0; i < resoluciones.Length; i++)
        {
            string opcion = $"{resoluciones[i].width} x {resoluciones[i].height}";
            opciones.Add(opcion);

            if (resoluciones[i].width == Screen.currentResolution.width &&
                resoluciones[i].height == Screen.currentResolution.height)
            {
                resolucionActual = i;
            }
        }

        resolucionesDropDown.AddOptions(opciones);

        // Obtener resolución guardada o actual
        int resolucionGuardada = PlayerPrefs.GetInt("numeroResolucion", resolucionActual);
        resolucionGuardada = Mathf.Clamp(resolucionGuardada, 0, resoluciones.Length - 1);

        // Aplicar sin disparar el evento del dropdown
        resolucionesDropDown.SetValueWithoutNotify(resolucionGuardada);
        resolucionesDropDown.RefreshShownValue();

        // Aplicar la resolución al inicio
        CambiarResolucion(resolucionGuardada);
    }
    public void CambiarResolucion(int indiceResolucion)
    {
        if (resoluciones == null || resoluciones.Length == 0) return;

        Resolution resolucion = resoluciones[indiceResolucion];
        Screen.SetResolution(resolucion.width, resolucion.height, Screen.fullScreen);

        PlayerPrefs.SetInt("numeroResolucion", indiceResolucion);
    }



    public void VolverMenu(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}
