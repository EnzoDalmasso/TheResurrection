using UnityEngine;

public class MenuVictoria : MonoBehaviour
{
    private bool activado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            JuegoMenuManager menu = FindFirstObjectByType<JuegoMenuManager>();
            if (menu != null)
            {
                menu.MostrarVictoria();
            }
        }
    }
}
