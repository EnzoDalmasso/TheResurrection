using UnityEngine;
using UnityEngine.Audio;

public class MusicaManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    private static MusicaManager instancia;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Evita duplicados si volvés a cargar la escena inicial
            return;
        }

        // Aplicar volumen guardado
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMusica", 0.75f);
        audioMixer.SetFloat("VolumenMusica", Mathf.Log10(volumenGuardado) * 20);
    }
}
