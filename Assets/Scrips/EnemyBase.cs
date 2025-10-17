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

    //Movimiento fisico
    [SerializeField] protected float moveSpeed = 2f;
    protected bool facingRight = true;
    protected Vector2 velocity;

    [Header("Gravedad Personalizada")]
    [SerializeField] protected float gravedad = -25f;
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
    [SerializeField] protected LayerMask capaSuelo;
    [SerializeField] protected Transform checkSuelo;
    [SerializeField] protected Transform checkPared;
    [SerializeField] protected float distanciaSuelo = 0.2f;
    [SerializeField] protected float distanciaPared = 0.2f;

    //Aturdimiento
    [SerializeField] protected float duracionAturdimientoBase = 1.5f;
    protected float tiempoAturdimiento;

    //Otros
    protected float distanciaJugador;
    protected int direccionPatrulla = 1;
    protected float temporizadorCambioDireccion = 0f;
    [SerializeField] protected float tiempoCambioDireccion = 5f;


    // START
    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        // Asegura que el Rigidbody este configurado correctamente
        rb.gravityScale = 0f;//Iguala este valor al del Player para misma gravedad
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

        if (player)
        {
            distanciaJugador = Vector2.Distance(transform.position, player.position);
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
        // Si detecta al jugador, cambia de estado
        if (distanciaJugador <= radioDeteccion)
        {
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

        if (HayParedAdelante() || !EstaEnSuelo())
        {
            direccionPatrulla *= -1;
        }
           
        velocity.x = direccionPatrulla * moveSpeed;
        Girar(velocity.x);

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
        Girar(dir.x);

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
        if (!canMove) return;

        // Aplicamos gravedad "manual"
        if (!EstaEnSuelo())
        {
            velocidadVertical += gravedad * Time.fixedDeltaTime;
        }
        else if (velocidadVertical < 0f)
        {
            velocidadVertical = 0f; // Detiene caida al tocar suelo
        }

        // Movimiento horizontal + vertical
        Vector2 vel = new Vector2(velocity.x, velocidadVertical);
        rb.linearVelocity = vel;

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
        Destroy(gameObject, 2f);
    }



    //  UTILIDADES
    protected void CambiarEstado(EnemyState nuevo)
    {
        if (currentState == nuevo) return;
        currentState = nuevo;
    }

    protected bool EstaEnSuelo()
    {
        RaycastHit2D hit = Physics2D.Raycast(checkSuelo.position, Vector2.down, distanciaSuelo, capaSuelo);
        enSuelo = hit.collider != null;
        return enSuelo;
    }

    protected bool HayParedAdelante()
    {
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        return Physics2D.Raycast(checkPared.position, dir, distanciaPared, capaSuelo);
    }

    protected void Girar(float direccionX)
    {
        if (direccionX > 0 && !facingRight)
        {
            if(EstaEnSuelo())
            {
                facingRight = true;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
           
        }
        else if (direccionX < 0 && facingRight)
        {
            if(EstaEnSuelo())
            {
                facingRight = false;
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
         
        }
    }


}

