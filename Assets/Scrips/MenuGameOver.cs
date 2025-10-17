
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class MenuGameOver : MonoBehaviour
{
    [SerializeField] private GameObject menuGameOver;
    private PlayerController1 player;
    private GameObject canvasHUD;//referencia al HUD del player

    private void Start()
    {
        player= GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController1>();
        player.MuerteJugador += ActivarMenu;

        // Busca el objeto HUD en la escena
        canvasHUD = GameObject.Find("BarrasEnergia");
    }

    private void ActivarMenu(object sender, EventArgs e)
    {
        ControladorSonido.instance.mutearSonido();

        ControladorSonido.instance.mutearSonido();

        // Buscar todos los enemigos y mutearlos
        EnemyBase[] enemigos = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        foreach (EnemyBase enemigo in enemigos)
        {
            
            //enemigo.mutearSonido();
        }
        menuGameOver.SetActive(true);

        if (canvasHUD != null)
        {
            canvasHUD.SetActive(false);
        }
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ControladorSonido.instance.desmutearSonido();

        EnemyBase[] enemigos = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        foreach (EnemyBase enemigo in enemigos)
        {
            //enemigo.desmutearSonido();
        }
        if (canvasHUD != null)
        {
            canvasHUD.SetActive(true);
        }
    }

    public void MenuInicial(string nombre)
    {
        Time.timeScale = 1f;//Restaurar tiempo por si estaba en pausa
        SceneManager.LoadScene(nombre);
    }

    public void Salir()
    {

        Debug.Log("Salir");
    }

}
