
using UnityEngine;
using UnityEngine.UI;

public class BarraSprint : MonoBehaviour
{

    [SerializeField] private Image imagenSprint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CambiarSprint(float valorActual, float valorMaximo)
    {
        imagenSprint.fillAmount = valorActual / valorMaximo;
    }


}
