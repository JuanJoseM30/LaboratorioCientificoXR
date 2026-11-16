using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Va en el VasoMezcla. Revisa qué reactivo se suelta en el socket del vaso.
public class SocketValidador : MonoBehaviour
{
    [Header("Socket del vaso")]
    public XRSocketInteractor socket;

    [Header("Reactivos")]
    public GameObject reactivoCorrecto;
    public GameObject[] reactivosIncorrectos;

    [Header("Líquido del vaso y colores de la mezcla")]
    public Renderer liquidoDelVaso;
    [Tooltip("Número de la ranura del material del líquido (0 si es un cilindro)")]
    public int ranuraDelLiquido = 0;
    public Material materialMezclaCorrecta;
    public Material materialMezclaIncorrecta;

    [Header("Cuánto tarda el tubo en desaparecer (segundos)")]
    public float tiempoAntesDeDesaparecer = 0.5f;

    [Header("Tarea que se tacha al acertar (opcional)")]
    public TMP_Text textoTarea;

    [Header("Eventos (conectar al ScoreManager)")]
    public UnityEvent alMezclaCorrecta;
    public UnityEvent alMezclaIncorrecta;

    private bool mezclaHecha = false;
    private HashSet<GameObject> erroresYaContados = new HashSet<GameObject>();

    private void OnEnable()
    {
        if (socket != null) socket.selectEntered.AddListener(AlSoltarEnVaso);
    }

    private void OnDisable()
    {
        if (socket != null) socket.selectEntered.RemoveListener(AlSoltarEnVaso);
    }

    private void AlSoltarEnVaso(SelectEnterEventArgs args)
    {
        GameObject objeto = args.interactableObject.transform.gameObject;

        if (mezclaHecha)
        {
            // La mezcla ya está lista: no acepta nada más
            StartCoroutine(Rechazar());
            return;
        }

        if (objeto == reactivoCorrecto)
        {
            mezclaHecha = true;
            CambiarColor(materialMezclaCorrecta);

            if (textoTarea != null)
                textoTarea.fontStyle |= FontStyles.Strikethrough;

            Debug.Log("[SocketValidador] ¡Reactivo correcto! Mezcla hecha.");
            alMezclaCorrecta.Invoke();
            StartCoroutine(Desaparecer(objeto));
        }
        else if (EsIncorrecto(objeto))
        {
            CambiarColor(materialMezclaIncorrecta);

            if (erroresYaContados.Add(objeto))
            {
                Debug.Log("[SocketValidador] Reactivo incorrecto: " + objeto.name);
                alMezclaIncorrecta.Invoke();
            }

            StartCoroutine(Desaparecer(objeto));
        }
        else
        {
            // No es un reactivo (ej: la probeta): se rechaza sin error
            StartCoroutine(Rechazar());
        }
    }

    private bool EsIncorrecto(GameObject objeto)
    {
        foreach (GameObject r in reactivosIncorrectos)
        {
            if (r == objeto) return true;
        }
        return false;
    }

    private void CambiarColor(Material material)
    {
        if (liquidoDelVaso == null || material == null) return;

        Material[] materiales = liquidoDelVaso.materials;
        if (ranuraDelLiquido >= 0 && ranuraDelLiquido < materiales.Length)
        {
            materiales[ranuraDelLiquido] = material;
            liquidoDelVaso.materials = materiales;
        }
    }

    // El tubo se queda un momento (como vertiendo) y luego desaparece
    private IEnumerator Desaparecer(GameObject objeto)
    {
        yield return new WaitForSeconds(tiempoAntesDeDesaparecer);
        objeto.SetActive(false);
    }

    // Apaga el socket un momento para que suelte el objeto
    private IEnumerator Rechazar()
    {
        yield return null;
        socket.socketActive = false;
        yield return new WaitForSeconds(1.5f);
        socket.socketActive = true;
    }
}