
using UnityEngine;

public class ControladorSonido : MonoBehaviour
{
 
    public static ControladorSonido instance;//Instancia Snglenton para acceder de otros scrips
    private AudioSource audioSource;//Referencia al componente de audio

    private void Awake()
    {
        // Si no existe una instancia del controlador
        if (instance == null)
        {
            instance = this; // Guardamos esta como la instancia principal
            DontDestroyOnLoad(gameObject);  //Evita que este objeto se destruya al cambiar de escena
        }
        else
        {
            Destroy(gameObject); //Si ya existe una instancia, destruimos este duplicado
            return;
        }
        audioSource = GetComponent<AudioSource>(); //Obtenemos el componente AudioSource del mismo objeto

        // Aplicar volumen guardado (por defecto 0.75f)
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMusica", 0.50f);
        audioSource.volume = volumenGuardado;
    }


    // Música de fondo
    public void ReproducirMusica(AudioClip musica, bool loop = true)
    {
        audioSource.clip = musica;
        audioSource.loop = loop;
        audioSource.Play();
    }

    // Cambiar volumen
    public void CambiarVolumen(float volumen)
    {
        audioSource.volume = volumen;
        PlayerPrefs.SetFloat("VolumenMusica", volumen);
    }

    
    public void EjecutarSonido(AudioClip sonido, float volumen)
    {
        if (sonido == null || audioSource == null)
            return;
        
        
        audioSource.PlayOneShot(sonido, volumen);//Reproduce el sonido una vez sin interrumpir otros sonidos
        
            
    }

    public void mutearSonido()
    {
        audioSource.Stop();


    }

    public void desmutearSonido()
    {
        audioSource.Play();
    }
}
