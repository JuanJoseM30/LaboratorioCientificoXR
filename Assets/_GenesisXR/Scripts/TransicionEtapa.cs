using System.Collections;
using UnityEngine;

// Cambia lo que se ve en pantalla al pasar de una etapa a otra.
// Se llama desde los eventos del GameManager (ej: Al Iniciar Experimento).
public class TransicionEtapa : MonoBehaviour
{
    [Header("Qué se OCULTA al cambiar de etapa")]
    public GameObject[] objetosQueSeOcultan;

    [Header("Qué se MUESTRA al cambiar de etapa")]
    public GameObject[] objetosQueSeMuestran;

    [Header("Espera antes de cambiar (segundos)")]
    public float segundosDeEspera = 3f;

    void Start()
    {
        // Al empezar el juego, lo de la nueva etapa debe estar escondido
        foreach (GameObject obj in objetosQueSeMuestran)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    // Este es el método que se conecta en el GameManager
    public void Iniciar()
    {
        StartCoroutine(CambiarDespuesDeEsperar());
    }

    private IEnumerator CambiarDespuesDeEsperar()
    {
        yield return new WaitForSeconds(segundosDeEspera);

        foreach (GameObject obj in objetosQueSeOcultan)
        {
            if (obj != null) obj.SetActive(false);
        }

        foreach (GameObject obj in objetosQueSeMuestran)
        {
            if (obj != null) obj.SetActive(true);
        }

        Debug.Log("[TransicionEtapa] Cambio de etapa en pantalla hecho");
    }
}