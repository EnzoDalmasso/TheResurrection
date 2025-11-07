using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float velocidadArrow = 10f;// velocidad de la flecha
    [SerializeField] public int danio = 1;//Daño que causa
    private Vector2 direccion; //Direccion de movimiento


    // Metodo para darle direccion desde el enemigo
    public void SetDireccion(Vector2 dir)
    {
        direccion = dir.normalized;

        //Rota la flecha para que parezca que sale con angulo
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }
    private void Start()
    {
       
        Collider2D miCollider = GetComponent<Collider2D>();
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemigo");

        foreach (GameObject enemigo in enemigos)
        {
            Collider2D colEnemigo = enemigo.GetComponent<Collider2D>();
            if (colEnemigo != null && miCollider != null)
            {
                Physics2D.IgnoreCollision(miCollider, colEnemigo);
            }
        }
    }
    void Update()
    {
        //Movimiento recto en el mundo
        transform.Translate(direccion * velocidadArrow * Time.deltaTime, Space.World);

        //Se destruye después de 3 segundos para evitar acumulacion
        Destroy(gameObject, 3f);
    }

   
    private void OnTriggerEnter2D(Collider2D other)
    {
      

        if (other.CompareTag("Player"))
        {
          
            Destroy(gameObject);
        }
    }
}
