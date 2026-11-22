using UnityEngine;
using UnityEngine.Events;

// Caja invisible: cuando el jugador (tag Player) entra, lanza un evento UNA vez.
// Opcional: solo funciona cuando la tarea actual es la indicada.
// Necesita un Collider con "Is Trigger" activado.
public class ZonaLlegada : MonoBehaviour
{
    [Header("Solo funciona cuando toca esta tarea (opcional)")]
    public ControladorTareas controladorTareas;
    [Tooltip("0 = funciona siempre")]
    public int tareaRequerida = 0;

    [Header("Qué pasa cuando el jugador llega")]
    public UnityEvent alLlegar;

    private bool yaLlego = false;

    private void OnTriggerEnter(Collider other) { Revisar(other); }

    // Por si el jugador ya estaba dentro cuando empezó la tarea
    private void OnTriggerStay(Collider other) { Revisar(other); }

    private void Revisar(Collider other)
    {
        if (yaLlego) return;
        if (!other.CompareTag("Player")) return;

        if (controladorTareas != null && tareaRequerida > 0
            && controladorTareas.TareaActual != tareaRequerida)
            return; // todavía no toca

        yaLlego = true;
        Debug.Log("[ZonaLlegada] El jugador llegó a " + gameObject.name);
        alLlegar.Invoke();
    }
}