using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuPausa : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    private GameObject canvasHUD; // referencia al HUD


    private bool juegoPausado=false;

    private void Start()
    {
        // Busca el objeto HUD en la escena
        canvasHUD = GameObject.Find("BarrasEnergia");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausa();
            }
                
        }
    }

    public void Pausa()
    {
        
        juegoPausado = true;
        Time.timeScale = 0f;

        botonPausa.SetActive(false);
        menuPausa.SetActive(true);

        if (canvasHUD != null)
        {
            canvasHUD.SetActive(false);
        }
            
    }

    public void Reanudar()
    {
    
        juegoPausado = false;
        Time.timeScale = 1f;

        botonPausa.SetActive(true);
        menuPausa.SetActive(false);

        if (canvasHUD != null)
        {
            canvasHUD.SetActive(true);
        }

    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

   

    public void menuInicial(string nombreEscena)
    {
        Time.timeScale = 1f; //Restaurar tiempo por si estaba en pausa
        SceneManager.LoadScene(nombreEscena);
        
    }
}
