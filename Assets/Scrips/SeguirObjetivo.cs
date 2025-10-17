using Cinemachine;
using UnityEngine;

public class SeguirObjetivo : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineCamera;

    private void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
    }

    // Se usa desde ControladorJuego para asignar el objetivo
    public void ConfigurarObjetivo(Transform objetivo)
    {
        if (cinemachineCamera != null)
        {
            cinemachineCamera.Follow = objetivo;
        }
    }
}