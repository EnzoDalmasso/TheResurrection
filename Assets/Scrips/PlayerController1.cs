
using System;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerController1 : MonoBehaviour
{
    //VARIABLES DE MOVIMIENTO
    [Header("Movimiento")]
    public float velocidadWalk = 5f;// Velocidad normal
    public float sprintSpeed = 8f;// Velocidad al correr
    public float fuerzaSalto = 8f;// Fuerza del salto
    public float tiempoSprintMaximo = 3f;// Duración maxima del sprint
    private bool estaCayendo = false;

    private float tiempoSprintRestante;//Tiempo restante de sprint
    private bool estaSprintando = false;//Si el jugador está corriendo
    private bool bloqueoSprint = false;// Si el sprint está bloqueado

    [Header("Deteccion Caja Lateral")]
    [SerializeField] private Transform[] sensoresCaja; // izquierda y derecha
    [SerializeField] private float distanciaSensorCaja = 0.1f;
    [SerializeField] private LayerMask capaCaja;


    //CONTROL DE PASOS
    private float tiempoPasos = 0f;
    private float intervaloCaminar = 0.5f;
    private float intervaloCorrer = 0.3f;

    //EMPUJE QUE GENERA EL ENEMIGO CUANDO ME GOLPEA
    [Header("Empuje Enemigo")]
    [SerializeField] public float fuerzaEmpuje = 5f;// Fuerza de empuje
    [SerializeField] public float duracionEmpuje = 0.2f;//Cuanto dura el empuje
    private bool siendoEmpujado = false; //Si esta en estado de empuje
    private Vector2 direccionEmpuje;//Direccion actual del empuje
    private float tiempoEmpuje;//Temporizador


    //FISICA Y MOVIMIENTO
    private Rigidbody2D rb;// Referencia al Rigidbody2D
    private bool enSuelo = false;//Indica si está tocando el suelo
    private bool puedeMoverse = true;//Control general del movimiento


    [Header("Deteccion Paredes")]
    [SerializeField] private LayerMask capaParedes;
    [SerializeField] private float distanciaDeteccionPared = 0.2f;
    private bool tocandoPared = false;
    [SerializeField] private Transform sensorPared;

    [Header("Deteccion de techo")]
    [SerializeField] private Transform[] sensorTecho;
    [SerializeField] private float distanciaSensor = 0.1f;
    [SerializeField] private LayerMask capaTecho;


    //TIEMPO COYOTE (PERMITE SALTAR POCO DESPUES DE DEJAR EL SUELO)
    [SerializeField] private float tiempoCoyoteTime = 0.1f;
    private float tiempoCoyote = 0f;

    //VIDA, MANA Y ESTADO
    [Header("Stats")]
    [SerializeField] private float VidaMaxima = 100f;
    private float VidaActual;
    [SerializeField] private int manaMaximo = 3;
    private int manaActual = 0;
    public bool estaVivo = true;

    //DISPARO
    [Header("Ataques")]
    [SerializeField] private Transform controladorDisparo;
    [SerializeField] private GameObject BolaBasica;
    [SerializeField] private GameObject BolaUltra;
    [SerializeField] private float tiempoEntreDisparos = 1f;
    [SerializeField] private float distanciaMinimaDisparo = 1f; // Distancia para que no pueda disparar cerca del player
    [SerializeField] private Vector2 offsetDistanciaMinima = Vector2.zero;

    private float proximoTiempoDisparo;
    private bool estaAtacando = false;

    // Sonidos
    [Header("Sonidos")]
    private AudioSource audioSource;
    [SerializeField] protected AudioClip sonidoCaminar;
    [SerializeField] protected AudioClip sonidoCorrer;
    [SerializeField] protected AudioClip sonidoDaño;
    [SerializeField] protected AudioClip sonidoMuerte;
    [SerializeField] private AudioClip sonidoDisparoBasico;
    [SerializeField] private AudioClip sonidoDisparoUltra;
    [SerializeField] private AudioClip sonidoSalto;


    //REFERENCIAS
    [Header("Referencias")]
    private CapsuleCollider2D capsuleCollider;
    private Animator animPlayer;
    private SpriteRenderer spriteRenderer;
    private bool orientacionDer = true;

    [Header("UI")]
    [SerializeField] private BarraVida barraVida;
    [SerializeField] private BarraSprint barraSprint;
    [SerializeField] private BarraPoder barraPoder;

    private bool yaSalto = false;
    //EVENTOS
    public event EventHandler MuerteJugador; //Se dispara cuando el jugador muere

    //START
    void Start()
    {


        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animPlayer = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        tiempoSprintRestante = tiempoSprintMaximo;
        VidaActual = VidaMaxima;
        manaActual = 0;
        barraPoder.ActualizarBarra(manaActual, manaMaximo);
        proximoTiempoDisparo = Time.time;

        animPlayer.SetBool("estaVivo", true);
        animPlayer.ResetTrigger("Dead");
        animPlayer.Rebind();
        animPlayer.Update(0f);
    }

    //UPDATE — ATAQUES Y ANIMACIÓN
    void Update()
    {
        if (!puedeMoverse || !estaVivo) return;

        //Si esta en algun menu no puede disparar
        if (!JuegoMenuManager.puedeDisparar)
            return;


        RotarControladorDisparoHaciaMouse();//Rotar el controlador de disparo hacia el mouse

        //Ataque basico
        if (Time.time >= proximoTiempoDisparo && !estaAtacando && Input.GetMouseButtonDown(0) && enSuelo)
        {
            Vector2 puntoMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // CANCELA SI EL MOUSE ESTÁ DENTRO DEL COLLIDER DEL PLAYER
            if (capsuleCollider.OverlapPoint(puntoMouse))
                return;

            Vector2 centroZonaBloqueo = (Vector2)transform.position + offsetDistanciaMinima;

            if (Vector2.Distance(centroZonaBloqueo, puntoMouse) < distanciaMinimaDisparo)
                return;

            OrientarHaciaMouse();

            estaAtacando = true;
            puedeMoverse = false; 
            rb.linearVelocity = Vector2.zero; 
            ReproducirSonido(sonidoDisparoBasico, 1f);

            if (estaSprintando)
                estaSprintando = false;

            animPlayer.SetTrigger("AttackBasic");
            proximoTiempoDisparo = Time.time + tiempoEntreDisparos;
        }

        //Ataque cargado, solo si el mana esta completo y no esta atacando
        if (manaActual == manaMaximo && Input.GetMouseButtonDown(1) && !estaAtacando && enSuelo)
        {
            Vector3 posicionMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float distancia = Vector2.Distance(controladorDisparo.position, posicionMouse);

            Vector2 puntoMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (capsuleCollider.OverlapPoint(puntoMouse))
                return;

            Vector2 centroZonaBloqueo = (Vector2)transform.position + offsetDistanciaMinima;

            if (Vector2.Distance(centroZonaBloqueo, puntoMouse) < distanciaMinimaDisparo)
                return;
            OrientarHaciaMouse();

            estaAtacando = true;
            puedeMoverse = false; 
            rb.linearVelocity = Vector2.zero;

            if (estaSprintando)
                estaSprintando = false;

            ReproducirSonido(sonidoDisparoUltra, 1f);
            animPlayer.SetTrigger("AttackUltra");
        }

   


        //Actualizamos parámetros para el Animator
        ActualizarParametrosAnimator();
    }


    // FIXEDUPDATE — MOVIMIENTO FISICO
    void FixedUpdate()
    {
        if (!puedeMoverse || !estaVivo) return;

        DetectarPared();
        

        // Si esta sprintando y choca con una pared, cancela el sprint y corta sonido
        if (estaSprintando && tocandoPared)
        {
            estaSprintando = false;
            audioSource.Stop(); // Corta cualquier sonido de pasos/correr activo
        }


        float inputX = siendoEmpujado ? 0 : Input.GetAxisRaw("Horizontal");
        bool botonSalto = Input.GetButton("Jump");

        //SISTEMA DE SPRINT
        bool presionandoSprint = Input.GetKey(KeyCode.LeftShift);

        // Si esta sprintando actualmente
        if (estaSprintando)
        {
            tiempoSprintRestante -= Time.fixedDeltaTime;

            //Si se suelta Shift o se acaba el tiempo
            if (!presionandoSprint || tiempoSprintRestante <= 0)
            {
                estaSprintando = false;
                animPlayer.SetBool("estaSprintando", false);

                if (tiempoSprintRestante <= 0)
                {
                    tiempoSprintRestante = 0;
                    bloqueoSprint = true; //Se bloquea hasta llenarse completamente
                }
            }
        }
        //Si no esta sprintando
        else
        {
            //Si esta bloqueado, solo recarga hasta llenarse
            if (bloqueoSprint)
            {
                //Ahora recarga igual aunque mantenga Shift
                if (tiempoSprintRestante < tiempoSprintMaximo)
                {
                    tiempoSprintRestante += Time.fixedDeltaTime;

                    //Cuando se llena completamente, se desbloquea
                    if (tiempoSprintRestante >= tiempoSprintMaximo)
                    {
                        tiempoSprintRestante = tiempoSprintMaximo;
                        bloqueoSprint = false;
                    }
                }
            }
            //Si no esta bloqueado, recarga normalmente mientras no presione Shift
            else if (!presionandoSprint && tiempoSprintRestante < tiempoSprintMaximo)
            {
                tiempoSprintRestante += Time.fixedDeltaTime;
                if (tiempoSprintRestante > tiempoSprintMaximo)
                {
                    tiempoSprintRestante = tiempoSprintMaximo;
                }
                   
            }

            //Solo permite sprintar cuando la barra esta completamente cargada
            if (presionandoSprint && !bloqueoSprint && tiempoSprintRestante >= tiempoSprintMaximo && Mathf.Abs(inputX) > 0.1f)
            {
                estaSprintando = true;
            }
        }
        //Actualizamos la barra visual
        barraSprint.CambiarSprint(tiempoSprintRestante, tiempoSprintMaximo);

        //MOVIMIENTO HORIZONTAL
        float velocidadMovimiento = estaSprintando ? sprintSpeed : velocidadWalk;

        Vector2 vel = rb.linearVelocity; //usamos velocity real del Rigidbody2D


        if (Mathf.Abs(inputX) > 0.1f)
        {
            // Si hay input, aplicamos velocidad normalmente
            vel.x = inputX * velocidadMovimiento;
        }
        else
        {
            // Si no hay input, desaceleramos suavemente en lugar de cortar a 0
            vel.x = Mathf.Lerp(vel.x, 0, 5f * Time.fixedDeltaTime);
        }

        //SALTO PERSONAJE

        //Salto con tiempo coyote
        if (enSuelo)
        {
            tiempoCoyote = tiempoCoyoteTime;
            yaSalto = false;
        }
        else
        {
            tiempoCoyote -= Time.fixedDeltaTime;
        }
           

        //se bloquea si esta tocando una caja lateral y se mueve
        bool bloqueadoPorCaja = TocaCajaLateral() && (Mathf.Abs(rb.linearVelocity.x) > 0.04f || estaSprintando);

        if (!siendoEmpujado && botonSalto && tiempoCoyote > 0f && !TocaTecho() && !bloqueadoPorCaja)
        {
            vel.y = fuerzaSalto;
            tiempoCoyote = 0f;
            ReproducirSonido(sonidoSalto, 1f);
            rb.linearVelocity = vel; // aplicar salto

            yaSalto=true;
        }

        //APLICAR VELOCIDAD

        rb.linearVelocity = new Vector2(vel.x, rb.linearVelocity.y);

        // Si toca un techo el personaje corta la subida
        if (TocaTecho() && rb.linearVelocity.y > 0)
        {
            //Cortamos la subida del jugador y le aplicamos una fuerza hacia abajo
            //Evitamos que se pegue en el techo
            //Comenza a caer el inmediatamente
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -0.5f);
        }

        //DETECTAR SUELO Y CAIDA
        //Detectar si esta en el suelo
        enSuelo = Mathf.Abs(rb.linearVelocity.y) < 0.01f;

        // Detectar caida
        estaCayendo = !enSuelo && rb.linearVelocity.y < -0.1f;

        //SONIDOS DE PASOS
        if (estaVivo && puedeMoverse && enSuelo && Mathf.Abs(rb.linearVelocity.x) > 0.1f && !tocandoPared)
        {
            tiempoPasos -= Time.fixedDeltaTime;

            if (tiempoPasos <= 0f)
            {
                AudioClip clip = estaSprintando ? sonidoCorrer : sonidoCaminar;
                float volumen = estaSprintando ? 0.8f : 0.6f;

                ReproducirSonido(clip, volumen);
                tiempoPasos = estaSprintando ? intervaloCorrer : intervaloCaminar;
            }
        }
        else
        {
            // Reiniciamos el temporizador si no se mueve o no está en suelo
            tiempoPasos = 0f;
        }
        //ORIENTACION DEL SPRITE
        //Voltear sprite segun la direccion
        OrientacionPlayer(inputX);
    }
   
    private void OnDrawGizmosSelected()
    {
        if (capsuleCollider != null)
        { // Si no hay collider todavía, igual dibujamos
            Gizmos.color = Color.red;

            // USAR EL OFFSET EN EL DIBUJO
            Vector2 centroZonaBloqueo = (Application.isPlaying)
                ? (Vector2)transform.position + offsetDistanciaMinima
                : (Vector2)transform.position + offsetDistanciaMinima;

            Gizmos.DrawWireSphere(centroZonaBloqueo, distanciaMinimaDisparo);
        }

        // COLOR 2: Radio mínimo de disparo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaMinimaDisparo);
    }
    //BOOLEANO QUE VERIFICA SI ESTA QUIETO EL PERSONAJE
    private bool EstaQuieto()
    {
        return Mathf.Abs(rb.linearVelocity.x) < 0.01f && Mathf.Abs(rb.linearVelocity.y) < 0.01f;
    }

    //ACTUALIZAR PARAMETROS DEL ANIMATOR
    void ActualizarParametrosAnimator()
    {
        //Estos valores se leen desde el Animator Controller
        //No reproducimos animaciones manualmente
        animPlayer.SetFloat("VelocidadX", Mathf.Abs(rb.linearVelocity.x));//Para pasar de idle a run
        animPlayer.SetFloat("VelocidadY", rb.linearVelocity.y);
        animPlayer.SetBool("enSuelo", enSuelo);// Controla salto/caida
        animPlayer.SetBool("estaSprintando", estaSprintando);//Corre vs camina
        animPlayer.SetBool("estaAtacando", estaAtacando);//El Animator controla la animacion
        animPlayer.SetBool("estaVivo", estaVivo);// Si esta muerto, muestra anim de muerte
        animPlayer.SetBool("estaCayendo", estaCayendo);

    }

    //ORIENTACION Y ROTACION DEL DISPARO
    //se encarga de voltear al personaje cuando se mueve con las teclas (A/D o flechas).
    void OrientacionPlayer(float input)
    {
        if ((orientacionDer && input < 0) || (!orientacionDer && input > 0))
        {
            orientacionDer = !orientacionDer;
            Vector3 escala = transform.localScale;
            escala.x *= -1;
            transform.localScale = escala;
        }
    }

    //COLISIONES
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Arrow"))
        {
            Arrow flecha = collision.collider.GetComponent<Arrow>();
            if (flecha != null)
            {
                RecibirDaño(flecha.danio);
               
                RecibirGolpe(flecha.transform.position); //Aplicamos empuje
                //puedeMoverse = true;
                Destroy(flecha.gameObject);
            }
        }
        if (collision.collider.CompareTag("LanzaSpearman"))
        {
            RecibirDaño(15);
            RecibirGolpe(collision.transform.position);
            //puedeMoverse = true;
        }
        if (collision.collider.CompareTag("EspadaSkeleton"))
        {
            RecibirDaño(10);
            RecibirGolpe(collision.transform.position);
            //puedeMoverse = true;
        }
      
    }
    //COLISIONES CON CAJAS
    private void OnCollisionStay2D(Collision2D collision)
    {
        //Verifica si la caja esta bajo el player 
        if (collision.gameObject.layer == LayerMask.NameToLayer("Caja"))
        {
            // Si el player esta tocando una caja y tiene impulso hacia arriba, lo corta
            if (rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            }
        }
    }

    private bool TocaCajaLateral()
    {
        foreach (Transform sensor in sensoresCaja)
        {
            if (sensor == null) continue;

            RaycastHit2D hit = Physics2D.Raycast(sensor.position, sensor.right * Mathf.Sign(sensor.localPosition.x), distanciaSensorCaja, capaCaja);

            if (hit.collider != null)
            {
                return true; // si alguno de los sensores toca una caja
            }
        }
        return false;
    }


    //DETECCION DE PARED
    private void DetectarPared()
    {
        Vector2 origen = sensorPared != null ? (Vector2)sensorPared.position : (Vector2)transform.position;
        Vector2 direccion = orientacionDer ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(origen, direccion, distanciaDeteccionPared, capaParedes);
        tocandoPared = hit.collider != null;

        Debug.DrawRay(origen, direccion * distanciaDeteccionPared, tocandoPared ? Color.red : Color.green);
    }

    //DETECCION DE TECHO
    private bool TocaTecho()
    {
        foreach (Transform sensor in sensorTecho)
        {
            if (sensor == null) continue;

            RaycastHit2D hit = Physics2D.Raycast(sensor.position, Vector2.up, distanciaSensor, capaTecho);

            if (hit.collider != null)
            {
                return true; // si uno toca, ya alcanza
            }
        }
        return false;
    }


    public void RecibirGolpe(Vector2 posicionEnemigo)
    {
        if (!estaVivo) return;

        //Calcula direccion desde el enemigo hacia el jugador
        direccionEmpuje = ((Vector2)transform.position - posicionEnemigo).normalized;

        //Si están casi en la misma posición, decide empuje lateral
        if (direccionEmpuje.magnitude < 0.1f)
        {
            direccionEmpuje = transform.position.x > posicionEnemigo.x ? Vector2.right : Vector2.left;
        }

        //Le damos un pequeño impulso hacia arriba
        direccionEmpuje.y = Mathf.Clamp(direccionEmpuje.y + 0.3f, 0.1f, 1f);

        // Reinicia movimiento actual y aplica fuerza física
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direccionEmpuje * fuerzaEmpuje, ForceMode2D.Impulse);

        // Control de estado
        siendoEmpujado = true;
        puedeMoverse = false;
        tiempoEmpuje = duracionEmpuje;

        // Restablecer control luego del empuje
        Invoke(nameof(FinEmpuje), duracionEmpuje);
    }

    private void FinEmpuje()
    {
        siendoEmpujado = false;
        puedeMoverse = true;
    }


    //Disparo basico
    public void DisparoBola1()
    {


        //Convertimos la posicion del mouse en coordenadas del mundo
        Vector3 posicionMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Calcula la direccion
        Vector2 direccion = (posicionMouse - controladorDisparo.position).normalized;

        //Instanciamos la bola
        GameObject bola = Instantiate(BolaBasica, controladorDisparo.position, Quaternion.identity);

        //Llamamos el scrip de la bola
        BolaMagicaBasica bolaBasica = bola.GetComponent<BolaMagicaBasica>();

        // Si existe la bola llamamos a la direccion
        if (bolaBasica != null)
        {
            bolaBasica.SetDireccion(direccion);
        }

    }


    //Disparo Ultra
    public void DisparoBola2()
    {
        if (manaActual < 3) return;//Si no tiene 3 de mana no dispara


        manaActual -= 3;
        barraPoder.ActualizarBarra(manaActual, manaMaximo);//Actualiza la barra de mana

        //Asegura que el player gire hacia donde esta el mouse
        Vector3 posicionMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Calcula la direccion
        Vector2 direccion = (posicionMouse - controladorDisparo.position).normalized;

        //Instanciamos la bola
        GameObject bola = Instantiate(BolaUltra, controladorDisparo.position, Quaternion.identity);

        //Llamamos el scrip de la bola
        BolaMagicaUltra bolaUltra = bola.GetComponent<BolaMagicaUltra>();

        // Si existe la bola llamamos a la direccion
        if (bolaUltra != null)
        {
            bolaUltra.SetDireccion(direccion);
        }

    }

   

    //Este metodo puede ser llamado desde el evento de animacion cuando termina el ataque
    public void TerminarAtaque()
    {
        estaAtacando = false;// ahora puede volver a disparar después del cooldown
        puedeMoverse = true;
    }


    //Se encarga de voltearlo automaticamente hacia donde esta el mouse al disparar
    void OrientarHaciaMouse()
    {
        Vector3 posicionMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool mouseADerecha = posicionMouse.x > transform.position.x;

        // Si el personaje mira a la derecha y el mouse está a la izquierda (o al revés), rotamos
        if ((orientacionDer && !mouseADerecha) || (!orientacionDer && mouseADerecha))
        {
            orientacionDer = !orientacionDer;
            Vector3 escala = transform.localScale;
            escala.x *= -1;
            transform.localScale = escala;
        }
    }

    //ROTA EL CONTROL DE DISPARO PARA QUE SIGA EL CURSOR DEL MOUSE EN TIEMPO REAL
    //CUANDO SE DISPARA LA BOLA SALE A LA DIRECCI´´ON CORRECTA
    void RotarControladorDisparoHaciaMouse()
    {
        // Si no hay controlador o cámara, no hacemos nada
        if (controladorDisparo == null || Camera.main == null)
        {
            return;
        }
            

        Vector3 posMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = posMouse - controladorDisparo.position;
        float angulo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        controladorDisparo.rotation = Quaternion.Euler(0, 0, angulo);
    }

    // VIDA Y MANA
    // RECIBIR DAÑO
    public void RecibirDaño(float cantidad)
    {
        //Si ya esta muerto, ignoramos
        if (!estaVivo) return;

        //Resta Bida
        VidaActual -= cantidad;
        barraVida.CambiarVida(VidaActual, VidaMaxima);
        ReproducirSonido(sonidoDaño);

        //Reproduce animacion de daño
        animPlayer.SetTrigger("Hurt");

        //Si la vida es 0, muere
        if (VidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        audioSource.Stop();
        ReproducirSonido(sonidoMuerte,1f,true);
        estaVivo = false;
        puedeMoverse = false;
        rb.linearVelocity = Vector2.zero; //Detiene movimiento

        FondoMovimiento fondo = FindFirstObjectByType<FondoMovimiento>();
        if (fondo != null)
            fondo.DesactivarSeguimientoJugador();

        animPlayer.SetBool("estaVivo", false);
        animPlayer.SetTrigger("Dead");

    }

    public void AnimacionMuerteTerminada()
    {
        MuerteJugador?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject, 1f);//Se destruye 

    }

    public void AgregarMana(int cantidad)
    {
        manaActual = Mathf.Clamp(manaActual + cantidad, 0, manaMaximo);
        barraPoder.ActualizarBarra(manaActual, manaMaximo);
    }

    public void AgregarVida(int cantidad)
    {
        VidaActual = Mathf.Clamp(VidaActual + cantidad, 0, VidaMaxima);
        barraVida.CambiarVida(VidaActual, VidaMaxima);
    }


    //SONIDO

    protected void ReproducirSonido(AudioClip clip, float volumen = 1f, bool interrumpir = false)
    {
        if (clip == null || audioSource == null)
            return;

        if (interrumpir)
            audioSource.Stop();

        audioSource.clip = clip;
        audioSource.Play();
    }

    public void SetPuedeMoverse(bool valor)
    {
        puedeMoverse = valor;
        if (!valor && rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Para frenar completamente
            
        }
            
    }

    public void ForzarIdle(bool valor)
    {
        if (animPlayer == null) return;

        if (valor)
        {
            animPlayer.SetFloat("VelocidadX", 0);
            animPlayer.SetFloat("VelocidadY", 0);
            animPlayer.SetBool("enSuelo", true);
            animPlayer.SetBool("estaSprintando", false);
            animPlayer.SetBool("estaAtacando", false);
        }
        else
        {
            // Permite que el Update vuelva a controlar las animaciones normalmente
            ActualizarParametrosAnimator();
        }
        
    }
}