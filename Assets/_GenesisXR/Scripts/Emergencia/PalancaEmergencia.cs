
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Content.Interaction;

[RequireComponent(typeof(XRLever))]
public class PalancaEmergencia : MonoBehaviour
{
    [Header("Referencia a la palanca")]
    [SerializeField] private XRLever palanca;

    [Header("Estado de la emergencia")]
    [SerializeField] private bool emergenciaActiva = false;

    [Header("Evento de fallo")]
    public UnityEvent AlFallarVentilacion = new UnityEvent();

    private bool ventilacionIntentada = false;

    private void Awake()
    {
        if (palanca == null)
            palanca = GetComponent<XRLever>();
    }

    private void OnEnable()
    {
        palanca.onLeverActivate.AddListener(AlActivarPalanca);
    }

    private void OnDisable()
    {
        palanca.onLeverActivate.RemoveListener(AlActivarPalanca);
    }

    // Se llamará cuando comience la emergencia
    public void PrepararEmergencia()
    {
        emergenciaActiva = true;
        ventilacionIntentada = false;

        // Devolver la palanca a su posición inicial
        palanca.value = false;

        Debug.Log("Emergencia preparada. Palanca reiniciada.");
    }

    private void AlActivarPalanca()
    {
        if (!emergenciaActiva)
            return;

        if (ventilacionIntentada)
            return;

        ventilacionIntentada = true;

        Debug.Log("¡FALLO! La ventilación no responde.");

        AlFallarVentilacion.Invoke();
    }

    // Permite consultar si ya se intentó activar la ventilación
    public bool VentilacionIntentada()
    {
        return ventilacionIntentada;
    }
}
