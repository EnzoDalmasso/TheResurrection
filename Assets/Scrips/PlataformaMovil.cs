using System.Diagnostics.Contracts;
using UnityEngine;

public class PlataformaMovil : MonoBehaviour
{
    [SerializeField] private Transform[] puntosMocivimiento;

    [SerializeField] private float velocidadMovimiento;

    private int siguientePlataforma;
    private bool ordenPlataformas;
   
    private void FixedUpdate()
    {
       if(ordenPlataformas && siguientePlataforma+1 >=puntosMocivimiento.Length)
        {
            ordenPlataformas = false;
        }
        if(!ordenPlataformas && siguientePlataforma<=0)
        {
            ordenPlataformas=true;
        }

        if (Vector2.Distance(transform.position, puntosMocivimiento[siguientePlataforma].position)< 0.1f)
        {
            if(ordenPlataformas)
            {
                siguientePlataforma += 1;
            }
            else
            {
                siguientePlataforma -= 1;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, puntosMocivimiento[siguientePlataforma].position, velocidadMovimiento * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            ContactPoint2D contacto = collision.GetContact(0);
            if(contacto.normal.y < -0.5f)
            {
                collision.transform.SetParent(this.transform);
                Debug.Log("Arriba");
            }
            
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
