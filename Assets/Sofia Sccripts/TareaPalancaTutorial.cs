
using UnityEngine;
using TMPro;
using UnityEngine.XR.Content.Interaction;

public class TareaPalancaTutorial : MonoBehaviour
{
    [Header("Palanca")]
    public XRLever palanca;

    [Header("Tarea")]
    public TMP_Text textoTarea;

    [Header("Panel final")]
    public GameObject panelTutorialCompletado;
    public GameObject fondoPanel;

    private bool completada = false;

    void Awake()
    {
        if (panelTutorialCompletado != null)
        {
            panelTutorialCompletado.SetActive(false);
        }
    }

    void Update()
    {
        if (palanca.value && !completada)
        {
            completada = true;

            // Tachar la última tarea
            if (textoTarea != null)
            {
                textoTarea.fontStyle |= FontStyles.Strikethrough;
            }

            // Ocultar el panel de instrucciones
            if (fondoPanel != null)
            {
                fondoPanel.SetActive(false);
            }

            // Mostrar la pantalla final
            if (panelTutorialCompletado != null)
            {
                panelTutorialCompletado.SetActive(true);
            }

            Debug.Log("¡Tutorial completado!");
        }
    }
}
