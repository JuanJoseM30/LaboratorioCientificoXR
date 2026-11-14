
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketMatraz : MonoBehaviour
{
    [Header("Configuración")]
    public XRSocketInteractor socket;
    public TMP_Text textoTarea;

    [Header("Siguiente instrucción")]
    public ControladorInstrucciones controlador;
    public int siguienteInstruccion = 3;

    private bool completado = false;

    private void OnEnable()
    {
        if (socket != null)
            socket.selectEntered.AddListener(AlColocarMatraz);
    }

    private void OnDisable()
    {
        if (socket != null)
            socket.selectEntered.RemoveListener(AlColocarMatraz);
    }

    private void AlColocarMatraz(SelectEnterEventArgs args)
    {
        if (completado) return;

        completado = true;

        if (textoTarea != null)
        {
            textoTarea.fontStyle |= FontStyles.Strikethrough;
        }

        if (controlador != null)
        {
            controlador.MostrarInstruccion(siguienteInstruccion);
        }

        Debug.Log("¡Matraz colocado! Siguiente instrucción activada.");
    }
}
