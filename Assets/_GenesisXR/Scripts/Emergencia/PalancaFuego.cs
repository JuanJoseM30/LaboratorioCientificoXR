
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

[RequireComponent(typeof(XRLever))]
public class PalancaFuego : MonoBehaviour
{
    [Header("Referencia a la palanca")]
    [SerializeField] private XRLever palanca;

    [Header("Elementos del mechero")]
    [SerializeField] private ParticleSystem llamas;
    [SerializeField] private Light luzMechero;
    [SerializeField] private AudioSource sonidoLlama;

    [Header("Estado")]
    [SerializeField] private bool mecheroEncendido = false;

    private void Awake()
    {
        // Obtener el XRLever del mismo objeto
        if (palanca == null)
            palanca = GetComponent<XRLever>();
    }

    private void OnEnable()
    {
        palanca.onLeverActivate.AddListener(EncenderMechero);
        palanca.onLeverDeactivate.AddListener(ApagarMechero);
    }

    private void OnDisable()
    {
        palanca.onLeverActivate.RemoveListener(EncenderMechero);
        palanca.onLeverDeactivate.RemoveListener(ApagarMechero);
    }

    private void Start()
    {
        // El mechero comienza apagado
        palanca.value = false;
        ApagarMechero();
    }

    public void EncenderMechero()
    {
        if (mecheroEncendido)
            return;

        mecheroEncendido = true;

        // Activar las llamas principales
        if (llamas != null)
            llamas.Play(true);

        // Activar la luz original
        if (luzMechero != null)
            luzMechero.enabled = true;

        // Reproducir sonido en bucle
        if (sonidoLlama != null && sonidoLlama.clip != null)
            sonidoLlama.Play();

        Debug.Log("Mechero encendido.");
    }

    public void ApagarMechero()
    {
        mecheroEncendido = false;

        // Detener las llamas principales
        if (llamas != null)
            llamas.Stop(
                false,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

        // Apagar la luz
        if (luzMechero != null)
            luzMechero.enabled = false;

        // Detener el sonido
        if (sonidoLlama != null)
            sonidoLlama.Stop();

        Debug.Log("Mechero apagado.");
    }

    public bool EstaEncendido()
    {
        return mecheroEncendido;
    }
}
