using UnityEngine;

public class SkeletonArcher : EnemyBase
{
    private Animator anim;
    [Header("Combate")]
    [SerializeField] private GameObject prefabFlecha;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float tiempoEntreDisparos = 1.5f;
    private float proximoDisparo;
    [SerializeField] private float fuerzaDisparo = 10f;

    // Control de pasos
    private float tiempoPasos = 0f;
    private float intervaloCaminar = 0.3f; // Ajusta segun la animacion


    // Sonidos
    [Header("Sonidos")]
    [SerializeField] protected AudioClip sonidoDaño;
    [SerializeField] protected AudioClip sonidoCaminar;
    [SerializeField] protected AudioClip sonidoMuerte;
    [SerializeField] private AudioClip sonidoDisparo;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        proximoDisparo = Time.time;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        bool moviendose = currentState == EnemyState.Patrol || currentState == EnemyState.Chase;
        anim.SetBool("IsMoving", moviendose);

        anim.SetBool("IsDead", currentState == EnemyState.Dead);

        // SONIDOS DE PASOS CON INTERVALOS (solo caminar)
        if (anim.GetBool("IsMoving"))
        {
            tiempoPasos -= Time.fixedDeltaTime;

            if (tiempoPasos <= 0f)
            {
                ReproducirSonido(sonidoCaminar, 0.2f);
                tiempoPasos = intervaloCaminar;
            }
        }
        else
        {
            tiempoPasos = 0f;
        }

    }

    protected override void Perseguir()
    {
        if (!canMove || player == null)
        {
            return;
        }

        // Mantiene distancia de disparo
        Vector2 dir = (player.position - transform.position);
        Girar(dir.x);

        if (distanciaJugador <= radioAtaque * 1.2f)
        { 
            CambiarEstado(EnemyState.Attack); 
        }
            
        else
        {
            CambiarEstado(EnemyState.Chase);
        }
           
    }

    protected override void Atacar()
    {
        if (isAttacking || player == null)
        {
            return;
        }
        canMove = false;
        rb.linearVelocity = Vector2.zero;

        if (Time.time >= proximoDisparo)
        {
            isAttacking = true;
            proximoDisparo = Time.time + tiempoEntreDisparos;
            anim.SetTrigger("Attack_SkeletonArcher");
        }
    }

    // Evento de animación: dispara flecha
    public void DispararFlecha()
    {
        if (prefabFlecha == null || puntoDisparo == null || player == null)
        {
            return;
        }

        ReproducirSonido(sonidoDisparo,0.9f);

        // Instancia la flecha
        GameObject flecha = Instantiate(prefabFlecha, puntoDisparo.position, Quaternion.identity);

        // Calcula direccion hacia el jugador
        Vector2 dir = (player.position - transform.position).normalized;

        // Le pasa la direccion al script de la flecha
        Arrow arrowScript = flecha.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.SetDireccion(dir);
        }
            

        
    }

    public void FinalizaAtaque()
    {
        isAttacking = false;
        canMove = true;

        if (!isStunned && !estaMuerto())
        {
            CambiarEstado(EnemyState.Patrol);
        }
            
    }

    public override void RecibirDanio(float cantidad)
    {
        if (currentState == EnemyState.Dead || isStunned)
        {
            return;
        }
        base.RecibirDanio(cantidad);

        //Si despues de aplicar daño sigue vivo reproduce el sonido de daño y animación.
        if (!estaMuerto())
        {
            anim.SetTrigger("recibeDanioArcher");
            ReproducirSonido(sonidoDaño, 0.9f);
        }
        
      
    }

    protected override void Muerte()
    {
        audioSource.Stop();
        ReproducirSonido(sonidoMuerte, 1f, true);
        anim.SetTrigger("Dead_SkeletonArcher");

        base.Muerte();
    }

    private bool estaMuerto() => currentState == EnemyState.Dead;

    protected void ReproducirSonido(AudioClip clip, float volumen = 1f, bool interrumpir = false)
    {
        if (clip == null || audioSource == null) return;

        if (interrumpir)
            audioSource.Stop();

        audioSource.PlayOneShot(clip, volumen);
    }
}

  
   
