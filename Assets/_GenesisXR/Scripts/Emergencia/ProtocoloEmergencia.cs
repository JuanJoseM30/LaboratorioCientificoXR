
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Content.Interaction;

public class ProtocoloEmergencia : MonoBehaviour
{
    [Header("Botones del protocolo")]
    [SerializeField] private XRPushButton botonVerde;
    [SerializeField] private XRPushButton botonNegro;
    [SerializeField] private XRPushButton botonRojo;

    [Header("Eventos")]
    [SerializeField] private UnityEvent alFallarSecuencia;
    [SerializeField] private UnityEvent alCompletarProtocolo;
    [SerializeField] private UnityEvent alAvanzarPaso;

    // 0 = Verde, 1 = Negro, 2 = Rojo, 3 = Completado
    private int pasoActual = 0;

    public int PasoActual => pasoActual;
    public bool ProtocoloCompletado => pasoActual == 3;

    private void OnEnable()
    {
        if (botonVerde != null)
            botonVerde.onPress.AddListener(PulsarVerde);

        if (botonNegro != null)
            botonNegro.onPress.AddListener(PulsarNegro);

        if (botonRojo != null)
            botonRojo.onPress.AddListener(PulsarRojo);
    }

    private void OnDisable()
    {
        if (botonVerde != null)
            botonVerde.onPress.RemoveListener(PulsarVerde);

        if (botonNegro != null)
            botonNegro.onPress.RemoveListener(PulsarNegro);

        if (botonRojo != null)
            botonRojo.onPress.RemoveListener(PulsarRojo);
    }

    private void PulsarVerde()
    {
        if (pasoActual == 0)
        {
            pasoActual = 1;
            Debug.Log("PROTOCOLO: Válvula de alivio activada.");
            alAvanzarPaso?.Invoke();
        }
        else if (pasoActual == 1)
        {
            return; // ya estaba pulsado: se ignora el rebote
        }
        else if (pasoActual != 3)
        {
            FallarSecuencia();
        }
    }

    private void PulsarNegro()
    {
        if (pasoActual == 1)
        {
            pasoActual = 2;
            Debug.Log("PROTOCOLO: Reactivos aislados.");
            alAvanzarPaso?.Invoke();
        }
        else if (pasoActual == 2)
        {
            return; // ya estaba pulsado: se ignora el rebote
        }
        else if (pasoActual != 3)
        {
            FallarSecuencia();
        }
    }

    private void PulsarRojo()
    {
        if (pasoActual == 2)
        {
            pasoActual = 3;

            Debug.Log("PROTOCOLO COMPLETADO: Extracci�n restablecida.");

            alAvanzarPaso?.Invoke();
            alCompletarProtocolo?.Invoke();
        }
        else if (pasoActual != 3)
        {
            FallarSecuencia();
        }
    }

    private void FallarSecuencia()
    {
        Debug.LogWarning("PROTOCOLO INCORRECTO: Reiniciando secuencia.");

        pasoActual = 0;

        alFallarSecuencia?.Invoke();
    }

    public void ReiniciarProtocolo()
    {
        pasoActual = 0;
        Debug.Log("Protocolo reiniciado.");
    }
}
