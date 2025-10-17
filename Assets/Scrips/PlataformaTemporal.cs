using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaTemporal : MonoBehaviour

{
    
    [SerializeField]public float tiempoVisible;//tiempo activa
    [SerializeField] public float tiempoInvisible;//tiempo desactivada

    private float temporizador;//tiempo en el cual inicializamos en 0
    private bool estaVisible = true;//booleano para corroborar si esta visible o no


    private Collider2D colPlataforma;
    private SpriteRenderer sprPlataforma;

    private void Start()
    {
        colPlataforma = GetComponent<Collider2D>();
        sprPlataforma = GetComponent<SpriteRenderer>();
        temporizador = tiempoVisible;
    }

    private void Update()
    {
        // Resta tiempo cada frame
        temporizador -= Time.deltaTime; //Arranca el conteo

        if (temporizador <= 0f)
        {
            if (estaVisible)
            {
                // Oculta la plataforma
                colPlataforma.enabled = false;
                sprPlataforma.enabled = false;
                temporizador = tiempoInvisible;
            }
            else
            {
                // Muestra la plataforma
                colPlataforma.enabled = true;
                sprPlataforma.enabled = true;
                temporizador = tiempoVisible;
            }

            estaVisible = !estaVisible; // cambia estado
        }
    }
}


