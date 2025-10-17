using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuVictoriaLvl : MonoBehaviour
{
    public GameObject menuVictoria;
    private GameObject canvasHUD;//referencia al HUD del player


    private void Start()
    {

        // Busca el objeto HUD en la escena
        canvasHUD = GameObject.Find("BarrasEnergia");
    }


    //Si el personaje Entra dentro de la collision se activa el menu victoria

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            menuVictoria.SetActive(true);
            Time.timeScale = 0f;


            if (canvasHUD != null)
            {
                canvasHUD.SetActive(false);
            }

            collision.GetComponent<PlayerController1>().enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;



        }
    }
    //Menu  Inicial
    public void MenuInicial(string nombreEscena)
    {

        SceneManager.LoadScene(nombreEscena);
    }


}
