using Unity.VisualScripting;
using UnityEngine;

public class SkeletonWarrior : EnemyBase
{
    private Animator anim;
    [Header("Combate")]
    [SerializeField] private GameObject dañoEspada;
    [SerializeField] private float tiempoEntreAtaques = 1f;
    private float proximoAtaque;
    [SerializeField] private float velocidadCorrer = 4f;


    // Control de pasos
    private float tiempoPasos = 0f;
    private float intervaloCaminar = 0.3f;//Puedes ajustar segun la velocidad de patrulla
    private float intervaloCorrer = 0.3f;//Ajustar según velocidad de persecución


    private bool corriendo;

    [Header("Defensa")]
    [SerializeField, Range(0f, 1f)] private float probabilidadDefensa =1f; //Chance de bloquear
    private bool enDefensa = false;




    // Sonidos
    [Header("Sonidos")]
    [SerializeField] protected AudioClip sonidoDaño;
    [SerializeField] protected AudioClip sonidoCaminar;
    [SerializeField] protected AudioClip sonidoCorrer;
    [SerializeField] protected AudioClip sonidoMuerte;
    [SerializeField] private AudioClip sonidoEspadazo;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        dañoEspada.SetActive(false);
        proximoAtaque = Time.time;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();



        float velocidadHorizontal = Mathf.Abs(rb.linearVelocity.x);

        //Se considera corriendo si esta persiguiendo
        anim.SetBool("IsRunning", currentState == EnemyState.Chase && velocidadHorizontal > 0.1f);

        //Se considera caminando solo si esta patrullando
        anim.SetBool("IsMoving", currentState == EnemyState.Patrol && velocidadHorizontal > 0.1f);

        anim.SetBool("IsDead", currentState == EnemyState.Dead);


        // SONIDOS DE PASOS CON INTERVALOS
        if (currentState == EnemyState.Patrol || currentState == EnemyState.Chase)
        {
            tiempoPasos -= Time.fixedDeltaTime;

            if (tiempoPasos <= 0f)
            {
                AudioClip clip;
                float volumen;
                float intervalo;

                if (currentState == EnemyState.Chase)
                {
                    clip = sonidoCorrer;
                    volumen = 0.3f;
                    intervalo = intervaloCorrer;
                }
                else
                {
                    clip = sonidoCaminar;
                    volumen = 0.2f;
                    intervalo = intervaloCaminar;
                }
                ReproducirSonido(clip, volumen);
                tiempoPasos = intervalo;
            }
        }
        else
        {
            tiempoPasos = 0f; // Reinicia si no se está moviendo
        }
    }

    protected override void Mover()
    {
        if (!canMove || currentState == EnemyState.Idle) return;

        rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocityY);

    }

    // PATRULLA
    protected override void Patrullar()
    {
        if (!canMove || enDefensa) return;

        velocity.x = dirX * moveSpeed;

        //Si el jugador esta cerca, empieza a perseguir
        if (player != null && distanciaJugador <= radioDeteccion * 1.2f)
        {
            CambiarEstado(EnemyState.Chase);
        }
    }

    // PERSECUCIÓN
    protected override void Perseguir()
    {
        if (enDefensa || !canMove || player == null)
        {
            return;
        }

        Vector2 dir = (player.position - transform.position).normalized;
        velocity.x = dir.x * velocidadCorrer;

        if ((dir.x > 0 && !facingRight) || (dir.x < 0 && facingRight))
        {
            Girar();
        }

        // Cambia a ataque solo cuando esta cerca del rango marcado
        if (distanciaJugador <= radioAtaque * 1.2f)
        {
            CambiarEstado(EnemyState.Attack);
        }
        //Sigue persiguiendo aunque se aleje un poco
        else if (distanciaJugador > radioDeteccion * 2f)
        {
            CambiarEstado(EnemyState.Patrol);
        }

    }

    // ATAQUE
    protected override void Atacar()
    {
        if (isAttacking || player == null) return;

        canMove = false;
        rb.linearVelocity = Vector2.zero;
        velocity = Vector2.zero;

        if (Time.time >= proximoAtaque)
        {
            isAttacking = true;
            proximoAtaque = Time.time + tiempoEntreAtaques;
            anim.SetTrigger("Attack_SkeletonWarrior");
            ReproducirSonido(sonidoEspadazo, 0.9f);
        }
    }

    //ACTIVCAR Y DESACTIAR DAÑO ESPADA
    public void ActivarDañoEspada()
    {
        dañoEspada.SetActive(true);
    }

    public void DesactivarDañoEspada()
    {
        dañoEspada.SetActive(false);
    }

    //FINALIZA EL ATAQUE DEL ESQUELETO
    public void FinalizaAtaque()
    {
        isAttacking = false;
        canMove = true;

        if (!isStunned && !estaMuerto())
        {
            CambiarEstado(EnemyState.Patrol);
        }
            
    }

    // RECIBIR DAÑO
    public override void RecibirDanio(float cantidad)
    {
     

        if (currentState == EnemyState.Dead || isStunned)
        {
            return;
        }

        // Si está actualmente en defensa, no recibe daño
        if (enDefensa)
            return;

        //Comprueba si el jugador esta frente al player
        bool jugadorEnFrente = (facingRight && player.position.x > transform.position.x) || (!facingRight && player.position.x < transform.position.x);

        // Si el jugador está detrás, no puede defenderse
        if (!jugadorEnFrente)
        {
            base.RecibirDanio(cantidad);

            if (!estaMuerto())
            {
                anim.SetTrigger("recibeDanioWarrior");
                ReproducirSonido(sonidoDaño, 0.9f);
            }
            return;
        }

        // Tiramos el dado de defensa (solo si está de frente)
        float chance = Random.value;

        if (chance < probabilidadDefensa)
        {
            ActivarDefensa();
            return; // Bloquea el golpe
        }

        // Si no se defendió, recibe daño normalmente
        base.RecibirDanio(cantidad);

        if (!estaMuerto())
        {
            anim.SetTrigger("recibeDanioWarrior");
            ReproducirSonido(sonidoDaño, 0.9f);
        }

    }

    // MUERTE
    protected override void Muerte()
    {
        audioSource.Stop();
        ReproducirSonido(sonidoMuerte, 1f, true);
        anim.SetTrigger("Dead_SkeletonWarrior");

     

        base.Muerte();
    }

    private bool estaMuerto() => currentState == EnemyState.Dead;
    
    protected void ReproducirSonido(AudioClip clip, float volumen = 1f, bool interrumpir = false)
    {
        if (clip == null || audioSource == null)
            return;

        if (interrumpir)
            audioSource.Stop();

        audioSource.PlayOneShot(clip, volumen);

    }

    private void ActivarDefensa()
    {
        enDefensa = true;
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        velocity = Vector2.zero;
        anim.SetTrigger("Defense_SkeletonWarrior"); // tu animacion de defensa

        //Desactiva el estado de defensa despues de la animacion
        Invoke(nameof(DesactivarDefensa), 1f);
    }

    private void DesactivarDefensa()
    {
        enDefensa = false;
        canMove = true;

        if (estaMuerto() || isStunned)
            return;

        //Si el jugador todavia esta cerca, vuelve a perseguir
        if (player != null && distanciaJugador <= radioDeteccion * 1.2f)
        {
            CambiarEstado(EnemyState.Chase);
        }
        else
        {
            CambiarEstado(EnemyState.Patrol);
        }
    }
}
