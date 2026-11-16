using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Va en el VasoMezcla. Revisa qué reactivo se echa y muestra la reacción química.
public class SocketValidador : MonoBehaviour
{
    // Datos de cada reactivo incorrecto: su resultado y su mensaje
    [System.Serializable]
    public class ReactivoIncorrecto
    {
        public GameObject reactivo;
        public Material resultado;
        [TextArea(2, 4)] public string mensaje;
    }

    [Header("Socket del vaso")]
    public XRSocketInteractor socket;

    [Header("Reactivo correcto")]
    public GameObject reactivoCorrecto;
    public Material resultadoCorrecto;
    [TextArea(2, 4)] public string mensajeCorrecto;

    [Header("Reactivos incorrectos")]
    public ReactivoIncorrecto[] reactivosIncorrectos;

    [Header("Líquido del vaso")]
    public Renderer liquidoDelVaso;
    public int ranuraDelLiquido = 0;

    [Header("Texto donde se explica la reacción (opcional)")]
    public TMP_Text textoReaccion;

    [Header("Efectos visuales (opcional)")]
    public GameObject efectoCorrecto;
    public GameObject efectoIncorrecto;

    [Header("Cuánto tarda el tubo en desaparecer (segundos)")]
    public float tiempoAntesDeDesaparecer = 0.5f;

    [Header("Eventos (conectar al ScoreManager, tareas, luces...)")]
    public UnityEvent alMezclaCorrecta;
    public UnityEvent alMezclaIncorrecta;

    private bool mezclaHecha = false;
    private HashSet<GameObject> erroresYaContados = new HashSet<GameObject>();

    private void Start()
    {
        if (textoReaccion != null) textoReaccion.text = "";
        if (efectoCorrecto != null) efectoCorrecto.SetActive(false);
        if (efectoIncorrecto != null) efectoIncorrecto.SetActive(false);
    }

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
            StartCoroutine(Rechazar());
            return;
        }

        // ¿Es el correcto?
        if (objeto == reactivoCorrecto)
        {
            mezclaHecha = true;
            CambiarColor(resultadoCorrecto);
            MostrarMensaje(mensajeCorrecto);
            ActivarEfecto(efectoCorrecto);

            Debug.Log("[SocketValidador] ¡Reacción correcta! " + objeto.name);
            alMezclaCorrecta.Invoke();
            StartCoroutine(Desaparecer(objeto));
            return;
        }

        // ¿Es uno de los incorrectos?
        ReactivoIncorrecto incorrecto = BuscarIncorrecto(objeto);
        if (incorrecto != null)
        {
            CambiarColor(incorrecto.resultado);
            MostrarMensaje(incorrecto.mensaje);
            ActivarEfecto(efectoIncorrecto);

            if (erroresYaContados.Add(objeto))
            {
                Debug.Log("[SocketValidador] Reacción incorrecta: " + objeto.name);
                alMezclaIncorrecta.Invoke();
            }

            StartCoroutine(Desaparecer(objeto));
            return;
        }

        // No es un reactivo (ej: la probeta): se rechaza sin error
        StartCoroutine(Rechazar());
    }

    private ReactivoIncorrecto BuscarIncorrecto(GameObject objeto)
    {
        foreach (ReactivoIncorrecto r in reactivosIncorrectos)
        {
            if (r != null && r.reactivo == objeto) return r;
        }
        return null;
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

    private void MostrarMensaje(string mensaje)
    {
        if (textoReaccion != null) textoReaccion.text = mensaje;
    }

    // Reinicia el efecto para que se vea cada vez que se usa
    private void ActivarEfecto(GameObject efecto)
    {
        if (efecto == null) return;
        efecto.SetActive(false);
        efecto.SetActive(true);
    }

    private IEnumerator Desaparecer(GameObject objeto)
    {
        yield return new WaitForSeconds(tiempoAntesDeDesaparecer);
        objeto.SetActive(false);
    }

    private IEnumerator Rechazar()
    {
        yield return null;
        socket.socketActive = false;
        yield return new WaitForSeconds(1.5f);
        socket.socketActive = true;
    }
}