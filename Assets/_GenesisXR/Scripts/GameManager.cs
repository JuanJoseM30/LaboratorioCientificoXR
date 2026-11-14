using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// El "director" del juego: sabe en qué etapa estamos y avisa cuando cambia.
public class GameManager : MonoBehaviour
{
    // Las etapas posibles del juego, en orden.
    public enum Etapa { Tutorial, Experimento, Emergencia, Final, Resultados }

    [Header("Estado actual (solo para mirar)")]
    [SerializeField] private Etapa etapaActual = Etapa.Tutorial;

    [Header("Qué pasa al empezar cada etapa")]
    public UnityEvent alIniciarTutorial;
    public UnityEvent alIniciarExperimento;
    public UnityEvent alIniciarEmergencia;
    public UnityEvent alIniciarFinal;

    [Header("Qué pasa al terminar el juego")]
    public UnityEvent alGanar;
    public UnityEvent alPerder;

    [Header("Pruebas en el editor")]
    [Tooltip("F1 a F5 para saltar de etapa sin jugar")]
    public bool usarTeclasDePrueba = true;

    private float tiempoInicio;
    private bool emergenciaControlada;

    // Datos que otros scripts (como la puntuación) pueden leer.
    public Etapa EtapaActual => etapaActual;
    public float TiempoTotal => Time.time - tiempoInicio;
    public bool EmergenciaFueControlada => emergenciaControlada;

    void Start()
    {
        tiempoInicio = Time.time;
        CambiarEtapa(Etapa.Tutorial);
    }

    void Update()
    {
        // Teclas de prueba: solo para probar sin jugar todo.
        if (!usarTeclasDePrueba || Keyboard.current == null) return;

        if (Keyboard.current.f1Key.wasPressedThisFrame) TerminarTutorial();
        if (Keyboard.current.f2Key.wasPressedThisFrame) TerminarExperimento();
        if (Keyboard.current.f3Key.wasPressedThisFrame) ControlarEmergencia();
        if (Keyboard.current.f4Key.wasPressedThisFrame) FallarEmergencia();
        if (Keyboard.current.f5Key.wasPressedThisFrame) TerminarFinal();
    }

    // ===== Métodos que llaman las etapas de los compañeros =====

    // Sofía: cuando el jugador termina el tutorial.
    public void TerminarTutorial()
    {
        if (etapaActual != Etapa.Tutorial) return;
        CambiarEtapa(Etapa.Experimento);
    }

    // Garzón: cuando el recipiente caliente llega a la estación experimental.
    public void TerminarExperimento()
    {
        if (etapaActual != Etapa.Experimento) return;
        CambiarEtapa(Etapa.Emergencia);
    }

    // Cata: cuando el jugador controla la emergencia a tiempo.
    public void ControlarEmergencia()
    {
        if (etapaActual != Etapa.Emergencia) return;
        emergenciaControlada = true;
        CambiarEtapa(Etapa.Final);
    }

    // Cata: cuando se acaba el tiempo de la emergencia.
    public void FallarEmergencia()
    {
        if (etapaActual != Etapa.Emergencia) return;
        emergenciaControlada = false;
        TerminarJuego(false);
    }

    // Obi: cuando el jugador coloca la muestra final.
    public void TerminarFinal()
    {
        if (etapaActual != Etapa.Final) return;
        TerminarJuego(true);
    }

    // ===== Funcionamiento interno =====

    private void CambiarEtapa(Etapa nuevaEtapa)
    {
        etapaActual = nuevaEtapa;
        Debug.Log("[GameManager] Etapa actual: " + nuevaEtapa);

        switch (nuevaEtapa)
        {
            case Etapa.Tutorial:    alIniciarTutorial.Invoke();    break;
            case Etapa.Experimento: alIniciarExperimento.Invoke(); break;
            case Etapa.Emergencia:  alIniciarEmergencia.Invoke();  break;
            case Etapa.Final:       alIniciarFinal.Invoke();       break;
        }
    }

    private void TerminarJuego(bool exito)
    {
        etapaActual = Etapa.Resultados;
        Debug.Log("[GameManager] Juego terminado. Resultado: " + (exito ? "ÉXITO" : "FRACASO")
                  + " | Tiempo: " + TiempoTotal.ToString("0") + " s");

        if (exito) alGanar.Invoke();
        else alPerder.Invoke();
    }
}