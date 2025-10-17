using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuntoControl : MonoBehaviour
{

    //Si el player colisiona con el ultimo control guarda la posicion
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            ControladorJuego.Instance.UltimoPuntoControl(gameObject);
            
        }
    }
}
