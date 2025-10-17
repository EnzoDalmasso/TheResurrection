using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    [SerializeField] private Image imagenVida;

    private void Start()
    {
        
    }


    public void CambiarVida(float valorActual, float valorMaximo)
    {
        imagenVida.fillAmount = valorActual / valorMaximo;
    }
}
