using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuntoControl : MonoBehaviour
{

    [Header("Sprites del checkpoint")]
    [SerializeField] private Sprite spriteInactivo;
    [SerializeField] private Sprite spriteActivo;

    private SpriteRenderer sr;
    private bool activado = false;
    private int miIndex;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Determinar el índice de este punto en el ControladorJuego
        for (int i = 0; i < ControladorJuego.Instance.PuntosDeControl.Length; i++)
        {
            if (ControladorJuego.Instance.PuntosDeControl[i] == gameObject)
            {
                miIndex = i;
                break;
            }
        }

        int ultimoIndex = PlayerPrefs.GetInt("puntosIndex", 0);
        if (miIndex <= ultimoIndex)
        {
            ActivarVisualmente();
        }
        else
        {
            DesactivarVisualmente();
        }
    }
    private void ActivarVisualmente()
    {
        activado = true;
        if (sr != null && spriteActivo != null)
            sr.sprite = spriteActivo;
    }

    private void DesactivarVisualmente()
    {
        activado = false;
        if (sr != null && spriteInactivo != null)
            sr.sprite = spriteInactivo;
    }
    //Si el player colisiona con el ultimo control guarda la posicion
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            ControladorJuego.Instance.UltimoPuntoControl(gameObject);
            ActivarVisualmente();
        }
    }
}
