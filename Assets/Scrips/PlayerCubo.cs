using UnityEngine;

public class PlayerCubo : MonoBehaviour
{
    public float moveSpeed = 5f;      // Velocidad de movimiento
    public float jumpVelocity = 12f;  // Velocidad inicial del salto
    public float gravity = 25f;       // Gravedad manual

    private Vector2 velocity;         // Velocidad actual
    private bool isGrounded = true;   // Si está en el suelo
    private float groundY = 0f;       // Y del "suelo plano"

    void Start()
    {
        // Guardamos la posición Y inicial como "suelo"
        groundY = transform.position.y;
        transform.position = new Vector2(transform.position.x, groundY);
    }

    void Update()
    {
        HandleInput();
        ApplyGravity();
        Move();
        CheckGrounded();
        
    }

    void HandleInput()
    {
        // Movimiento horizontal (A/D o flechas)
        float inputX = Input.GetAxisRaw("Horizontal");
        velocity.x = inputX * moveSpeed;

        // Salto
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = jumpVelocity;
            isGrounded = false;
        }
    }

    void ApplyGravity()
    {
        // Si no está en el suelo, aplicar gravedad
        if (!isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
    }

    void Move()
    {
        // Mover el jugador con la velocidad calculada
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    void CheckGrounded()
    {
        // Si cayó por debajo del suelo, lo "aterrizamos"
        if (transform.position.y <= groundY)
        {
            transform.position = new Vector2(transform.position.x, groundY);
            velocity.y = 0;
            isGrounded = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlataformaMovil"))
        {
            transform.parent = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlataformaMovil")
        {
            transform.parent = null;
        }
    }

}

