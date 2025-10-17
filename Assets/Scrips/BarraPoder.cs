using UnityEngine;
using UnityEngine.UI;

public class BarraPoder : MonoBehaviour
{
    
    [SerializeField] private Image fillPoder;

    

    //Actualiza la barra de poder

    public void ActualizarBarra(float mana, float manaMax)
    {
        fillPoder.fillAmount = mana/ manaMax;
    }
}
