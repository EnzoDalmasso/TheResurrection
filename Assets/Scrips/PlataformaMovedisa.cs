using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaMovedisa : MonoBehaviour
{
    private Vector3 posicionInicial;//Posicion inicial de la plataforma
    private Rigidbody2D rb;//Rigid de la plataforma
    private Collider2D col;//Collider de la plataforma
    private SpriteRenderer sr;//Sprite de la plataforma

    [SerializeField] private float tiempoTemblor = 0.5f;//tiempo en el cual tiembla la plataforma antes de caer
    [SerializeField] private float tiempoCaida = 0.5f;//tiempo que tarda antes de soltar la caida
    [SerializeField] private float tiempoRespawn = 3f;//tiempo que tarde en reaparecer la plataforma
    [SerializeField] private float fuerzaTemblor = 0.1f;//Intensidad de movimiento hacia los lados que genera la plataforma

    private void Awake()
    {
        posicionInicial = transform.position;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.bodyType = RigidbodyType2D.Kinematic; ; //al inicio no cae la plataforma
    }

    //Si el player cae sobre la plataforma inicia una corrutina
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Caer());
        }
    }


    private IEnumerator Caer()
    {
        // Peque�o temblor
        float tiempoTranscurrido = 0f;//tiempo transcurrido
        Vector3 posInicio = transform.position; //Guarda la posicion de inicio para volver al centro luego del temblor.

        while (tiempoTranscurrido < tiempoTemblor)
        {
            //Movemos la plataforma de un lado para el otro
            //Multiplicamos por fuerzatemblor para controlar la intensidad
            //Espera el frame antes de continuar
            float x = Mathf.Sin(tiempoTranscurrido * 50f) * fuerzaTemblor; 
            transform.position = posInicio + new Vector3(x, 0, 0);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        transform.position = posInicio; //volvemos a la posicion de inicio
        yield return new WaitForSeconds(tiempoCaida);

        //Se cae
        rb.bodyType = RigidbodyType2D.Dynamic;
        col.enabled = false;

        //Esperamos que caiga un rato y luego desaparezca
        yield return new WaitForSeconds(1f);
        sr.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Esperamos respawn
        yield return new WaitForSeconds(tiempoRespawn);

        // Reseteamos
        transform.position = posicionInicial;
        col.enabled = true;
        sr.enabled = true;
    }
}
