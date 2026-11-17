
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
        // Detectar cuando el jugador baja la palanca
        palanca.onLeverDeactivate.AddListener(AlDesactivarPalanca);
    }

    private void OnDisable()
    {
        palanca.onLeverDeactivate.RemoveListener(AlDesactivarPalanca);
    }

    public void PrepararEmergencia()
    {
        emergenciaActiva = true;
        ventilacionIntentada = false;

        // IMPORTANTE:
        // No modificamos palanca.value.
        // El mechero debe permanecer encendido.

        Debug.Log("Emergencia preparada. El mechero continúa encendido.");
    }

    private void AlDesactivarPalanca()
    {
        if (!emergenciaActiva || ventilacionIntentada)
            return;

        ventilacionIntentada = true;

        Debug.LogWarning("¡FALLO! La ventilación no responde.");

        AlFallarVentilacion.Invoke();
    }

    public void FinalizarEmergencia()
    {
        emergenciaActiva = false;
        ventilacionIntentada = false;

        Debug.Log("Palanca de emergencia desactivada.");
    }

    public bool VentilacionIntentada()
    {
        return ventilacionIntentada;
    }
}
