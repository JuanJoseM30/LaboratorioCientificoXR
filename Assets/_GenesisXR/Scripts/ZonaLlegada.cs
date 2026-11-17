using UnityEngine;
using UnityEngine.Events;

// Caja invisible: cuando el jugador (tag Player) entra, lanza un evento UNA vez.
// Necesita un Collider con "Is Trigger" activado.
public class ZonaLlegada : MonoBehaviour
{
    [Header("Qué pasa cuando el jugador llega")]
    public UnityEvent alLlegar;

    private bool yaLlego = false;

    private void OnTriggerEnter(Collider other)
    {
        if (yaLlego) return;
        if (!other.CompareTag("Player")) return;

        yaLlego = true;
        Debug.Log("[ZonaLlegada] El jugador llegó a " + gameObject.name);
        alLlegar.Invoke();
    }
}
