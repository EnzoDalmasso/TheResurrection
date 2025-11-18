using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class AudioSFXSlider : MonoBehaviour
{

    public AudioMixer mixer;
    public Slider slider;

    private void Start()
    {
        //Obtiene el valor guardado en PlayerPrefs
        //Si no existe, usa volumen maximo por defecto
        //Actualiza el slider.
        //Llama a SetVolume() para aplicar el volumen real al AudioMixer
        float savedValue = PlayerPrefs.GetFloat("SFXVolume", 1f);
        slider.value = savedValue;
        SetVolume(savedValue);
    }

    public void SetVolume(float value)
    {
        //Guarda el volumen para la proxima vez que se abra el juego
        PlayerPrefs.SetFloat("SFXVolume", value);

        // Convertimos 0–1 en decibeles
        float volumeDB = Mathf.Log10(Mathf.Clamp(value, 0.001f, 1f)) * 20f;

        mixer.SetFloat("SFXVolume", volumeDB);
    }
}
