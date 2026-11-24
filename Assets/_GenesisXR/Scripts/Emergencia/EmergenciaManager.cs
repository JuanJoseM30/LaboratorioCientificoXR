using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
public class EmergenciaManager : MonoBehaviour
{
    [Header("Duración de la emergencia")]
    [SerializeField] private float tiempoMaximo = 60f;
    [SerializeField] private float tiempoAntesDeExplosion = 2f;

    [Header("Cronómetro en pantalla")]
    [SerializeField] private TMP_Text textoCronometro;

    [Header("Scripts de interacción")]
    [SerializeField] private PalancaEmergencia palancaVentilacion;
    [SerializeField] private ProtocoloEmergencia protocolo;

    [Header("Efectos de la emergencia")]
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private ParticleSystem humo;
    [SerializeField] private Light[] lucesEmergencia;

    [Header("Ventilador")]
    [SerializeField] private Animation animacionVentilador;
    [SerializeField] private AudioSource sonidoVentilador;

    [Header("Sonidos")]
    [SerializeField] private AudioSource sonidoExplosion;
    [SerializeField] private AudioSource sonidoAlarma;
    [SerializeField] private AudioSource sonidoFalloVentilacion;
    [SerializeField] private AudioSource sonidoFalloProtocolo;
    [SerializeField] private AudioSource sonidoEmergenciaControlada;

    [Header("Eventos para otros sistemas")]
    public UnityEvent AlFallarEmergencia = new UnityEvent();
    public UnityEvent AlControlarEmergencia = new UnityEvent();
    public UnityEvent AlFallarVentilacion = new UnityEvent();

    private float tiempoRestante;
    private bool emergenciaActiva;
    private bool emergenciaTerminada;
    private Color[] coloresOriginales;
    private float[] intensidadesOriginales;
    private Coroutine parpadeoLuces;

    public float TiempoRestante => tiempoRestante;
    public bool EmergenciaActiva => emergenciaActiva;


    private void Awake()
    {
        GuardarEstadoOriginalLuces();

        if (protocolo != null)
            protocolo.enabled = false;
    }

    private void OnEnable()
    {
        if (palancaVentilacion != null)
            palancaVentilacion.AlFallarVentilacion.AddListener(FallarVentilacion);
    }

    private void OnDisable()
    {
        if (palancaVentilacion != null)
            palancaVentilacion.AlFallarVentilacion.RemoveListener(FallarVentilacion);
    }

    private void GuardarEstadoOriginalLuces()
    {
        if (lucesEmergencia == null)
            return;

        coloresOriginales = new Color[lucesEmergencia.Length];
        intensidadesOriginales = new float[lucesEmergencia.Length];

        for (int i = 0; i < lucesEmergencia.Length; i++)
        {
            if (lucesEmergencia[i] == null)
                continue;

            coloresOriginales[i] = lucesEmergencia[i].color;
            intensidadesOriginales[i] = lucesEmergencia[i].intensity;
        }
    }

    // Obi podrá llamar este método al colocar el recipiente caliente.
    public void IniciarEmergencia()
    {
        if (emergenciaActiva || emergenciaTerminada)
            return;

        emergenciaActiva = true;
        tiempoRestante = tiempoMaximo;
        if (textoCronometro != null)
        {
            textoCronometro.gameObject.SetActive(true);
            ActualizarCronometro();
        }

        Debug.Log("EMERGENCIA: iniciando protocolo.");

        if (palancaVentilacion != null)
            palancaVentilacion.PrepararEmergencia();

        if (protocolo != null)
        {
            protocolo.ReiniciarProtocolo();
            protocolo.enabled = false;
        }

        StartCoroutine(SecuenciaInicioEmergencia());
    }

    private IEnumerator SecuenciaInicioEmergencia()
    {
        // Esperar 2 segundos después de colocar el recipiente
        yield return new WaitForSeconds(tiempoAntesDeExplosion);

        // Explosión inesperada
        if (explosion != null)
        {
            explosion.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            explosion.Play(true);
            Debug.Log("Explosión activada desde EmergenciaManager.");
        }

        if (sonidoExplosion != null)
            sonidoExplosion.Play();

        // Activar humo
        if (humo != null)
            humo.Play(true);

        // Activar alarma
        if (sonidoAlarma != null)
            sonidoAlarma.Play();

        // Activar luces rojas
        IniciarParpadeoLuces();

        // Habilitar los botones del protocolo
        if (protocolo != null)
            protocolo.enabled = true;

        Debug.Log("EMERGENCIA ACTIVA: tienes " + tiempoRestante.ToString("F1") + " segundos.");
    }

    private void Update()
    {
        if (!emergenciaActiva || emergenciaTerminada)
            return;

        tiempoRestante -= Time.deltaTime;
        ActualizarCronometro();

        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            FallarEmergencia();
        }
    }

    private void FallarVentilacion()
    {
        if (!emergenciaActiva || emergenciaTerminada)
            return;

        Debug.LogWarning("FALLO: la ventilación no responde.");

        if (sonidoFalloVentilacion != null)
            sonidoFalloVentilacion.Play();

        AlFallarVentilacion.Invoke();
    }

    // Conecta este método al evento de fallo de secuencia del protocolo.
    public void FallarSecuenciaProtocolo()
    {
        if (!emergenciaActiva || emergenciaTerminada)
            return;

        if (sonidoFalloProtocolo != null)
            sonidoFalloProtocolo.Play();

        Debug.LogWarning("Secuencia incorrecta. Intenta nuevamente.");
    }

    // Conecta este método al evento de protocolo completado.
    public void ControlarEmergencia()
    {
        if (!emergenciaActiva || emergenciaTerminada)
            return;

        emergenciaTerminada = true;
        emergenciaActiva = false;

        Debug.Log("EMERGENCIA CONTROLADA. Tiempo restante: " + tiempoRestante.ToString("F1") + " segundos.");

        if (sonidoEmergenciaControlada != null)
            sonidoEmergenciaControlada.Play();

        if (sonidoAlarma != null)
            sonidoAlarma.Stop();

        if (humo != null)
            humo.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        DetenerParpadeoLuces();
        RestaurarLuces();

        if (animacionVentilador != null)
            animacionVentilador.Play();

        if (sonidoVentilador != null)
            sonidoVentilador.Play();

        if (protocolo != null)
            protocolo.enabled = false;

        if (palancaVentilacion != null)
            palancaVentilacion.FinalizarEmergencia();

        AlControlarEmergencia.Invoke();
    }

    private void FallarEmergencia()
    {
        if (emergenciaTerminada)
            return;

        emergenciaTerminada = true;
        emergenciaActiva = false;

        Debug.LogWarning("TIEMPO AGOTADO: emergencia no controlada.");

        if (sonidoAlarma != null)
            sonidoAlarma.Stop();

        if (protocolo != null)
            protocolo.enabled = false;

        if (palancaVentilacion != null)
            palancaVentilacion.FinalizarEmergencia();

        DetenerParpadeoLuces();
        RestaurarLuces();

        AlFallarEmergencia.Invoke();
    }

    private void IniciarParpadeoLuces()
    {
        if (parpadeoLuces != null)
            StopCoroutine(parpadeoLuces);

        parpadeoLuces = StartCoroutine(ParpadearLuces());
    }

    private IEnumerator ParpadearLuces()
    {
        Color rojoEmergencia = new Color(1f, 0f, 0f);

        // Tiempo que permanece cada estado
        float duracionEncendida = 0.45f;
        float duracionApagada = 0.35f;

        while (emergenciaActiva)
        {
            // FASE ROJA: luces encendidas en rojo intenso
            for (int i = 0; i < lucesEmergencia.Length; i++)
            {
                if (lucesEmergencia[i] == null)
                    continue;

                lucesEmergencia[i].color = rojoEmergencia;
                lucesEmergencia[i].intensity = intensidadesOriginales[i] * 2.5f;
            }

            yield return new WaitForSeconds(duracionEncendida);

            // FASE NORMAL: recuperar temporalmente la iluminación original
            for (int i = 0; i < lucesEmergencia.Length; i++)
            {
                if (lucesEmergencia[i] == null)
                    continue;

                lucesEmergencia[i].color = coloresOriginales[i];
                lucesEmergencia[i].intensity = intensidadesOriginales[i] * 0.2f;
            }

            yield return new WaitForSeconds(duracionApagada);
        }
    }

    private void DetenerParpadeoLuces()
    {
        if (parpadeoLuces != null)
        {
            StopCoroutine(parpadeoLuces);
            parpadeoLuces = null;
        }
    }

    private void RestaurarLuces()
    {
        if (lucesEmergencia == null)
            return;

        for (int i = 0; i < lucesEmergencia.Length; i++)
        {
            if (lucesEmergencia[i] == null)
                continue;

            lucesEmergencia[i].color = coloresOriginales[i];
            lucesEmergencia[i].intensity = intensidadesOriginales[i];
        }
    }
    private void ActualizarCronometro()
    {
        if (textoCronometro == null) return;

        int minutos = Mathf.FloorToInt(tiempoRestante / 60f);
        int segundos = Mathf.FloorToInt(tiempoRestante % 60f);

        textoCronometro.text = minutos.ToString("00") + ":" +
                               segundos.ToString("00");
    }
}