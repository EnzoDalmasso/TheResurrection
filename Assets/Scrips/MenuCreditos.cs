using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCreditos : MonoBehaviour
{
    public void volveralMenu(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}
