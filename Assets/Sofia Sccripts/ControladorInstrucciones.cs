
using UnityEngine;
using TMPro;

public class ControladorInstrucciones : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelBienvenida;
    public GameObject fondoPanel;

    [Header("Instrucciones")]
    public TMP_Text texto1;
    public TMP_Text texto2;
    public TMP_Text texto3;
    public TMP_Text texto4;

    void Start()
    {
        panelBienvenida.SetActive(true);
        fondoPanel.SetActive(false);

        OcultarInstrucciones();
    }

    public void ComenzarInduccion()
    {
        panelBienvenida.SetActive(false);
        fondoPanel.SetActive(true);

        MostrarInstruccion(1);
    }

    public void MostrarInstruccion(int numero)
    {
        texto1.gameObject.SetActive(numero == 1);
        texto2.gameObject.SetActive(numero == 2);
        texto3.gameObject.SetActive(numero == 3);
        texto4.gameObject.SetActive(numero == 4);
    }

    public void OcultarInstrucciones()
    {
        texto1.gameObject.SetActive(false);
        texto2.gameObject.SetActive(false);
        texto3.gameObject.SetActive(false);
        texto4.gameObject.SetActive(false);
    }
}
