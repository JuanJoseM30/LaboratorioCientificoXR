using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Va en la PalancaFuego (el mechero de Cata).
// Si el jugador enciende el mechero cuando toca la tarea de calentar,
// espera unos segundos y avisa de que la mezcla ya se calentó.
public class CalentarMezcla : MonoBehaviour
{
    [Header("Control de tareas de la Etapa 2")]
    public ControladorTareas controladorTareas;
    public int tareaDeCalentar = 3;

    [Header("Tiempo calentando (segundos)")]
    public float segundosCalentando = 5f;

    [Header("Qué pasa cuando termina de calentar")]
    public UnityEvent alTerminarDeCalentar;

    private bool yaCalentado = false;

    // Conectar al "On Lever Activate" de la palanca del mechero
    public void Empezar()
    {
        if (yaCalentado) return;

        if (controladorTareas != null && controladorTareas.TareaActual != tareaDeCalentar)
        {
            Debug.Log("[CalentarMezcla] Todavía no toca calentar");
            return;
        }

        yaCalentado = true;
        StartCoroutine(Calentar());
    }

    private IEnumerator Calentar()
    {
        Debug.Log("[CalentarMezcla] Calentando la mezcla...");
        yield return new WaitForSeconds(segundosCalentando);
        Debug.Log("[CalentarMezcla] ¡Mezcla calentada!");
        alTerminarDeCalentar.Invoke();
    }
}