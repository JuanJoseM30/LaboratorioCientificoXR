using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonPruebaEmergencia : MonoBehaviour
{
    [SerializeField] private EmergenciaManager emergenciaManager;

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (emergenciaManager != null)
            {
                emergenciaManager.IniciarEmergencia();
                Debug.Log("Prueba: emergencia iniciada.");
            }
            else
            {
                Debug.LogWarning("No se ha asignado EmergenciaManager.");
            }
        }
    }
}