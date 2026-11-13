
using UnityEngine;
using TMPro;

public class ZonaLlegadaTutorial : MonoBehaviour
{
    [Header("Tarea que se va a completar")]
    public TMP_Text textoTarea;

    private bool tareaCompletada = false;

    private void OnTriggerEnter(Collider other)
    {
        // Comprobar si quien entró es el jugador
        if (other.CompareTag("Player") && !tareaCompletada)
        {
            tareaCompletada = true;

            // Tachar la tarea
            if (textoTarea != null)
            {
                textoTarea.fontStyle |= FontStyles.Strikethrough;
            }

            Debug.Log("¡Tarea completada: Llegar al escritorio!");

            // Desactivar la zona para evitar repetir la detección
            GetComponent<Collider>().enabled = false;
        }
    }
}
