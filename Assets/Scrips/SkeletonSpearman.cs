using UnityEngine;

public class SkeletonSpearman : EnemyBase
{
    private Animator anim;
    [Header("Combate")]
    [SerializeField] private GameObject dañoLanza;
    [SerializeField] private float tiempoEntreAtaques = 1f;
    private float proximoAtaque;
    [SerializeField] private float velocidadCorrer = 4f;

    // Control de pasos
    private float tiempoPasos = 0f;
    private float intervaloCaminar = 0.4f;//Puedes ajustar segun la velocidad de patrulla
    private float intervaloCorrer = 0.2f;//Ajustar según velocidad de persecución


    // Sonidos
    [Header("Sonidos")]
    [SerializeField] protected AudioClip sonidoDaño;
    [SerializeField] protected AudioClip sonidoCaminar;
    [SerializeField] protected AudioClip sonidoCorrer;
    [SerializeField] protected AudioClip sonidoMuerte;
    [SerializeField] private AudioClip sonidoLanza;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        dañoLanza.SetActive(false);
        proximoAtaque = Time.time;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        anim.SetBool("IsRunning", currentState == EnemyState.Chase);
        anim.SetBool("IsMoving", currentState == EnemyState.Patrol);
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

    //PERSECUCION
    protected override void Perseguir()
    {
        if (!canMove || player == null)
        {
            return;
        }
        Vector2 dir = (player.position - transform.position).normalized;
        velocity.x = dir.x * velocidadCorrer;
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

    //ATAQUE
    protected override void Atacar()
    {
        if (isAttacking || player == null)
        {
            return;
        }
        canMove = false;
        rb.linearVelocity = Vector2.zero;

        if (Time.time >= proximoAtaque)
        {
            isAttacking = true;
            proximoAtaque = Time.time + tiempoEntreAtaques;
            anim.SetTrigger("Attack_1_SkeletonSpearman");
            ReproducirSonido(sonidoLanza, 0.9f);
        }
    }

    //EVENTO PARA ACTIVAR O DESACTIVAR COLLIDER LANZA
    public void ActivarDañoLanza()
    {
        dañoLanza.SetActive(true);
    }
    
    public void DesactivarDañoLanza()
    {
        dañoLanza.SetActive(false);
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

        if (!estaMuerto())
        {
            anim.SetTrigger("recibeDanioSpearman");
            ReproducirSonido(sonidoDaño, 0.9f);
        }
        
       
    }

    protected override void Muerte()
    {
        audioSource.Stop();
        ReproducirSonido(sonidoMuerte, 1f, true);
        anim.SetTrigger("Dead_SkeletonSpearman");


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
}
