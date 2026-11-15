
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

    [Header("Iluminación del tutorial")]
    public Light luzEscritorio;
    public Light luzMatrazRosa;
    public Light luzZonaMatrazRosa;
    public Light luzMatrazAzul;
    public Light luzZonaMatrazAzul;

    void Start()
    {
        panelBienvenida.SetActive(true);
        fondoPanel.SetActive(false);

        OcultarInstrucciones();
        ApagarTodasLasLuces();
    }

    public void ComenzarInduccion()
    {
        panelBienvenida.SetActive(false);
        fondoPanel.SetActive(true);

        MostrarInstruccion(1);

        if (luzEscritorio != null)
            luzEscritorio.enabled = true;
    }

    public void ApagarLuzEscritorio()
    {
        if (luzEscritorio != null)
            luzEscritorio.enabled = false;
    }

    void ApagarTodasLasLuces()
    {
        if (luzEscritorio != null)
            luzEscritorio.enabled = false;

        if (luzMatrazRosa != null)
            luzMatrazRosa.enabled = false;

        if (luzZonaMatrazRosa != null)
            luzZonaMatrazRosa.enabled = false;

        if (luzMatrazAzul != null)
            luzMatrazAzul.enabled = false;

        if (luzZonaMatrazAzul != null)
            luzZonaMatrazAzul.enabled = false;
    }

    public void MostrarInstruccion(int numero)
    {
        texto1.gameObject.SetActive(numero == 1);
        texto2.gameObject.SetActive(numero == 2);
        texto3.gameObject.SetActive(numero == 3);
        texto4.gameObject.SetActive(numero == 4);

        // Actividad 2: matraz rosa
        if (luzMatrazRosa != null)
            luzMatrazRosa.enabled = numero == 2;

        if (luzZonaMatrazRosa != null)
            luzZonaMatrazRosa.enabled = numero == 2;

        // Actividad 3: matraz azul
        if (luzMatrazAzul != null)
            luzMatrazAzul.enabled = numero == 3;

        if (luzZonaMatrazAzul != null)
            luzZonaMatrazAzul.enabled = numero == 3;
    }

    public void OcultarInstrucciones()
    {
        texto1.gameObject.SetActive(false);
        texto2.gameObject.SetActive(false);
        texto3.gameObject.SetActive(false);
        texto4.gameObject.SetActive(false);
    }
}
