using Newtonsoft.Json.Bson;
using System;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Stunned,
    Dead
}

public class EnemyBase : MonoBehaviour
{
    //Referencias
    protected Transform player;
    protected AudioSource audioSource;
    protected Rigidbody2D rb;
    protected CapsuleCollider2D col;
    
    //Movimiento fisico
    [SerializeField] protected float moveSpeed = 2f;
    protected bool facingRight = true;
    protected Vector2 velocity;
    protected float dirX = 1;
   

    [Header("Gravedad Personalizada")]
    [SerializeField] protected float gravedad = 9.8f;
    protected bool enSuelo = false;
    protected float velocidadVertical = 0f;

    //Vida
    [SerializeField] protected float vidaMaxima = 100f;
    protected float vidaActual;

    //Maquina de estados
    protected EnemyState currentState = EnemyState.Idle;
    protected bool canMove = true;
    protected bool isAttacking;
    protected bool isStunned;

    //Deteccion
    [SerializeField] protected float radioDeteccion = 6f;
    [SerializeField] protected float radioAtaque = 1.5f;
    [SerializeField] LayerMask Suelo;
    [SerializeField] private LayerMask Pared;


    //Aturdimiento
    [SerializeField] protected float duracionAturdimientoBase = 1.5f;
    protected float tiempoAturdimiento;

    //Otros
    protected float distanciaJugador;
    protected int direccionPatrulla = 1;
    protected float temporizadorCambioDireccion = 0f;
    [SerializeField] protected float tiempoCambioDireccion = 5f;
    [SerializeField] private float tiempoEntreGolpes = 0.5f;
private float tiempoUltimoGolpe = 0f;

    // START
    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        col=GetComponent<CapsuleCollider2D>();
        // Asegura que el Rigidbody este configurado correctamente
        rb.gravityScale = 9.8f;//Iguala este valor al del Player para misma gravedad
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform; //Asignamos el transform del Player

            PlayerController1 playerController = playerObj.GetComponent<PlayerController1>();
            if (playerController != null)
            {
                playerController.MuerteJugador += OnPlayerDeath;
            }
                
        }

        if (player != null)
            distanciaJugador = Vector2.Distance(transform.position, player.position);

        vidaActual = vidaMaxima;
        CambiarEstado(EnemyState.Patrol);
    }



    // UPDATE DE FISICA
    protected virtual void FixedUpdate()
    {
        if (currentState == EnemyState.Dead)
        {
            return;
        }

        // Primero chequea el suelo
        CheckSuelo();

        if (player != null)
        {
            distanciaJugador = Vector2.Distance(transform.position, player.position);
        }

        // Solo hacer detecciones si está en suelo
        if (enSuelo)
        {
            Detecciones();
        }
        

           

        switch (currentState)
        {
            case EnemyState.Idle:
                {
                    Idle();
                    break;
                }
            case EnemyState.Patrol:
                {
                    Patrullar();
                    break;
                }
            case EnemyState.Chase:
                {
                    Perseguir();
                    break;
                }
            case EnemyState.Attack:
                {
                    Atacar();
                    break;
                }
            case EnemyState.Stunned:
                {
                    ManejarAturdimiento();
                    break;

                }

        }

        if (!isStunned && currentState != EnemyState.Dead)
        {
            Mover();
        }
        
    }

    //Se llama cuando el jugador muere
    protected virtual void OnPlayerDeath(object sender, EventArgs e)
    {
        //Desvinculamos la referencia al jugador
        player = null;

        //Cambiamos el comportamiento
        canMove = false;
        velocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        //Los enemigos vuelven a Idle
        CambiarEstado(EnemyState.Idle);
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.enabled = false; //Detiene todas las animaciones
        }

        AudioSource audio=GetComponent<AudioSource>();
        if(audio!=null)
        {
        audio.enabled = false;
        }

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemigo") )
        {
            //Cambia de direccion
            Girar();

            //Reinicia el temporizador del patrol para evitar bucles
            temporizadorCambioDireccion = 0f;
        }

        // Si choca con una puerta gira
        if (collision.collider.name == "Puerta")
        {
            Girar();
            temporizadorCambioDireccion = 0f;
        }
    }
    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // COOL DOWN: evita pegar 60 veces por segundo
            if (Time.time < tiempoUltimoGolpe + tiempoEntreGolpes)
                return;

            tiempoUltimoGolpe = Time.time; // Resetea el cooldown

            PlayerController1 player = collision.GetComponent<PlayerController1>();

            if (player != null)
            {
                // Daño
                player.RecibirDaño(10f);

                // Empuje usando la POSICIÓN del enemigo
                player.RecibirGolpe(transform.position);
            }
        }
    }
    protected virtual void OnDestroy()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");

        //Por seguridad, desuscribir si el Player es destruido
        if (player != null)
        {
            PlayerController1 playerController = player.GetComponent<PlayerController1>();
            if (playerController != null)
            {
                playerController.MuerteJugador -= OnPlayerDeath;
            }
               
        }
    }

    //METODOS DE ESTADO
    protected virtual void Idle()
    {
        //Bloquea todo movimiento
        velocity = Vector2.zero;
        velocidadVertical = 0f;
        rb.linearVelocity = Vector2.zero;
        canMove = false;

        // Si detecta al jugador, cambia de estado
        if (distanciaJugador <= radioDeteccion)
        {

            canMove = true; // Vuelve a moverse si detecta jugador
            CambiarEstado(EnemyState.Chase);
        }
            
    }

    protected virtual void Patrullar()
    {
        if (!canMove)
        {
            return;
        }


        temporizadorCambioDireccion += Time.deltaTime;
        if (temporizadorCambioDireccion >= tiempoCambioDireccion)
        {
            direccionPatrulla *= -1;
            temporizadorCambioDireccion = 0f;
        }
  
        //Si el jugador esta cerca lo persigue
        if (distanciaJugador <= radioDeteccion)
        {
            CambiarEstado(EnemyState.Chase);
        }
        

    }

    protected virtual void Perseguir()
    {
        if (!canMove || player == null) 
        {
            return;
        }


        
        Vector2 dir = (player.position - transform.position).normalized;
        velocity.x = dir.x * moveSpeed * 1.5f; //Aumentamos la velocidad para que parezca que corra
        Girar();

        if (distanciaJugador <= radioAtaque)
        {
            CambiarEstado(EnemyState.Attack);
        }
        else if (distanciaJugador > radioDeteccion * 1.5f)
        {
            CambiarEstado(EnemyState.Patrol);
        }
            
    }

    
    protected virtual void Atacar()
    {
        if (isAttacking || player == null)
        {
            return;
        }
        velocity.x = 0f;
        canMove = false;
        isAttacking = true;
       
    }

    protected virtual void ManejarAturdimiento()
    {
        tiempoAturdimiento -= Time.deltaTime;
        if (tiempoAturdimiento <= 0f)
        {
            isStunned = false;
            canMove = true;
            CambiarEstado(EnemyState.Patrol);
        }
    }


    //  MOVIMIENTO CON RIGIDBODY2D
    protected virtual void Mover()
    {
        if (!canMove || currentState == EnemyState.Idle) return;

        rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocityY);
  

    }


    //  COMBATE Y DAÑO
    public virtual void RecibirDanio(float cantidad)
    {
        if (currentState == EnemyState.Dead || isStunned)
        {
            return;
        }

        vidaActual -= cantidad;
        AplicarAturdimiento(duracionAturdimientoBase);

        if (vidaActual <= 0f)
        {
            CambiarEstado(EnemyState.Dead);
            Muerte();
        }
        else
        {
            CambiarEstado(EnemyState.Stunned);
        }
    }

    protected virtual void AplicarAturdimiento(float duracion)
    {
        tiempoAturdimiento = duracion;
        isStunned = true;
        canMove = false;
        velocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }

    protected virtual void Muerte()
    {
        canMove = false;
        isAttacking = false;
        velocity.x = 0f;

        //Apagar el collider de daño
        Transform danio = transform.Find("DanioPlayer");
        if (danio != null)
        {
            Collider2D colDanio = danio.GetComponent<Collider2D>();
            if (colDanio != null)
                colDanio.enabled = false;
        }

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        Destroy(gameObject, 2f);
    }



    //  UTILIDADES
    protected void CambiarEstado(EnemyState nuevo)
    {
        if (currentState == nuevo) return;
        currentState = nuevo;
    }


    private void CheckSuelo()
    {
        //Utilizamos el centro del collider, no la del transform
        Vector2 centroCol = (Vector2)col.bounds.center;

        //Calculamos los puntos izquierda y derecha del collider
        Vector2 sueloIz = new Vector2(centroCol.x - (col.bounds.extents.x * 0.9f), centroCol.y - col.bounds.extents.y);
        Vector2 sueloDer = new Vector2(centroCol.x + (col.bounds.extents.x * 0.9f), centroCol.y - col.bounds.extents.y);

        //Raycast
        float distanciaRayo = 0.03f;
        RaycastHit2D rayoSueloIzq = Physics2D.Raycast(sueloIz, Vector2.down, distanciaRayo, Suelo);
        RaycastHit2D rayoSueloDer = Physics2D.Raycast(sueloDer, Vector2.down, distanciaRayo, Suelo);

        // Debug
        Debug.DrawRay(sueloIz, Vector2.down * distanciaRayo, Color.cyan);
        Debug.DrawRay(sueloDer, Vector2.down * distanciaRayo, Color.cyan);

        enSuelo = (rayoSueloIzq || rayoSueloDer);
    }


    protected virtual void Girar()
    {
        rb.linearVelocity = Vector2.zero;
        facingRight = !facingRight;//Actualiza la direccion real
        transform.localScale = new Vector2(transform.localScale.x*-1,transform.localScale.y);
        dirX *= -1;
    }

    private void Detecciones()
    {

        Vector2 centroCol = (Vector2)col.bounds.center;
        Vector2 posRayoDebajo = new Vector2(centroCol.x + (col.bounds.extents.x * dirX),centroCol.y - col.bounds.extents.y);

        float distanciaRayoX = 0.05f;
       
        RaycastHit2D rayoDebajo = Physics2D.Raycast(posRayoDebajo, Vector2.down, distanciaRayoX, Suelo);
        Debug.DrawRay(posRayoDebajo, Vector2.down * distanciaRayoX, Color.red);

        if (!rayoDebajo)
        {
            Girar();
            return;
        }


        //Raycast hacia adelante (para detectar paredes)
        Vector2 posRayoFrente = new Vector2(centroCol.x + (col.bounds.extents.x * dirX),centroCol.y);

        float distanciaRayoFrente = 0.05f;
        RaycastHit2D rayoFrente = Physics2D.Raycast(posRayoFrente, Vector2.right * dirX, distanciaRayoFrente, Pared);
        Debug.DrawRay(posRayoFrente, Vector2.right * dirX * distanciaRayoFrente, Color.red);

        // Si hay pared adelante gira
        if (rayoFrente)
        {
            Girar();
        }
    }
}

