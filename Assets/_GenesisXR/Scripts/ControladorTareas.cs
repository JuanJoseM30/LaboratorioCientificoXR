using UnityEngine;
using TMPro;

// Muestra las tareas de una etapa UNA POR UNA, en orden.
// Al completar una: la tacha, apaga sus luces y muestra la siguiente con sus luces.
// Va en el panel de la etapa (ej: PanelEtapa2).
public class ControladorTareas : MonoBehaviour
{
    // Grupo de luces de UNA tarea (puede tener una o varias)
    [System.Serializable]
    public class LucesDeTarea
    {
        public Light[] luces;
    }

    [Header("Textos de las tareas, en orden")]
    public TMP_Text[] textosTareas;

    [Header("Luces guía de cada tarea (mismo orden, opcional)")]
    public LucesDeTarea[] lucesPorTarea;

    // Tarea que el jugador debe hacer ahora (1 = la primera)
    private int tareaActual = 0;

    public int TareaActual => tareaActual;

    private void OnEnable()
    {
        for (int i = 0; i < textosTareas.Length; i++)
        {
            if (textosTareas[i] == null) continue;
            textosTareas[i].fontStyle &= ~FontStyles.Strikethrough;
            textosTareas[i].gameObject.SetActive(false);
        }
        ApagarTodasLasLuces();

        tareaActual = 1;
        MostrarTarea(tareaActual);
    }

    private void OnDisable()
    {
        ApagarTodasLasLuces();
    }

    // Llamar desde zonas, sockets, botones... con el número de la tarea (1, 2, 3...)
    public void CompletarTarea(int numero)
    {
        if (numero != tareaActual) return; // fuera de orden: se ignora

        int i = numero - 1;
        if (i < textosTareas.Length && textosTareas[i] != null)
            textosTareas[i].fontStyle |= FontStyles.Strikethrough;

        EncenderLuces(numero, false);
        Debug.Log("[ControladorTareas] Tarea " + numero + " completada");

        tareaActual++;
        MostrarTarea(tareaActual);
    }

    private void MostrarTarea(int numero)
    {
        int i = numero - 1;
        if (i >= textosTareas.Length) return;

        if (textosTareas[i] != null)
            textosTareas[i].gameObject.SetActive(true);

        EncenderLuces(numero, true);
    }

    private void EncenderLuces(int numero, bool encendidas)
    {
        int i = numero - 1;
        if (lucesPorTarea == null || i < 0 || i >= lucesPorTarea.Length) return;
        if (lucesPorTarea[i] == null || lucesPorTarea[i].luces == null) return;

        foreach (Light luz in lucesPorTarea[i].luces)
        {
            if (luz != null) luz.enabled = encendidas;
        }
    }

    private void ApagarTodasLasLuces()
    {
        if (lucesPorTarea == null) return;
        foreach (LucesDeTarea grupo in lucesPorTarea)
        {
            if (grupo == null || grupo.luces == null) continue;
            foreach (Light luz in grupo.luces)
            {
                if (luz != null) luz.enabled = false;
            }
        }
    }
}