
using UnityEngine;
using TMPro;

public class ZonaLlegadaTutorial : MonoBehaviour
{
    [Header("Tarea que se va a completar")]
    public TMP_Text textoTarea;

    [Header("Control de instrucciones")]
    public ControladorInstrucciones controlador;

    private bool tareaCompletada = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !tareaCompletada)
        {
            tareaCompletada = true;

            if (textoTarea != null)
            {
                textoTarea.fontStyle |= FontStyles.Strikethrough;
            }

            Debug.Log("¡Tarea completada: Llegar al escritorio!");

            // Mostrar la segunda instrucción
            if (controlador != null)
            {
                controlador.MostrarInstruccion(2);
            }

            GetComponent<Collider>().enabled = false;
        }
    }
}
