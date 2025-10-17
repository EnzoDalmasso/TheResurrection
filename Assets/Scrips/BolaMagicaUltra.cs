using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BolaMagicaUltra : MonoBehaviour
{

    [SerializeField] private float velocidad;//Velocidad de la bola magica
    [SerializeField] public int danioBola;//Daño de la bola

    private Vector2 direccion = Vector2.right; //Direccion de movimiento



    // Update is called once per frame
    private void Update()
    {

        transform.Translate(Vector2.right * velocidad * Time.deltaTime, Space.Self);//Movimiento de la bola

        Destroy(gameObject, 1f);//Despues del segundo se destruye la bola



    }

    public void SetDireccion(Vector2 dir)
    {
        direccion = dir.normalized;

        // Rotar visualmente la bala hacia la dirección
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angulo);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Suelo"))//Si colisiona con el suelo se destruye
        {

            Destroy(gameObject);
        }

        if (collision.CompareTag("Enemigo")) //Su colisiona con el enemigo le envia al enemigo el daño que genera la bola y aplica un pequeño aturdimiento
        {
            EnemyBase enemigo = collision.GetComponent<EnemyBase>();

            if (enemigo != null)
            {
                enemigo.RecibirDanio(danioBola);
                //enemigo.AplicarAturdimiento(0.5f);
            }

            Destroy(gameObject);

        }

    }

}


