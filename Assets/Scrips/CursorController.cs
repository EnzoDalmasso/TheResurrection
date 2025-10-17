using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D cursorTexture;  // El sprite del cursor
    public Vector2 hotspot = new Vector2(0, 0);  // Punto de anclaje del cursor (ajústalo si es necesario)

    void Awake()
    {
        // Mantiene este GameObject entre escenas
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Establece el cursor personalizado
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);

        // Asegura que el cursor esté visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;  // Deja libre el movimiento del cursor
    }
}
