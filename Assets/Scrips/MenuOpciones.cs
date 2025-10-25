using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuOpciones : MonoBehaviour
{
    [SerializeField] private Slider sliderVolumen;

    private void Start()
    {
        // Verificar si el slider está asignado en el Inspector
        if (sliderVolumen == null)
        {
            return; // Salimos para evitar errores
        }

        // Cargar el volumen guardado (por defecto 0.75f)
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMusica", 0.75f);
        sliderVolumen.value = volumenGuardado;
        CambiarVolumen(volumenGuardado);
    }

    public void PantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
    }

    public void CambiarVolumen(float volumen)
    {
        if (ControladorSonido.instance != null)
            ControladorSonido.instance.CambiarVolumen(volumen);
    }



    public void VolverMenu(string nombreEscena)
    {

        SceneManager.LoadScene(nombreEscena);
    }


    public void cambiarCalidad(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
}
