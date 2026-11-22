using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

// Lleva la cuenta de la partida, calcula la puntuación final y la muestra en un panel.
public class ScoreManager : MonoBehaviour
{
    [Header("Referencias")]
    public GameManager gameManager;

    [Header("Panel de resultados")]
    [Tooltip("El panel completo (se oculta al empezar y aparece al terminar)")]
    public GameObject panelResultados;
    [Tooltip("Texto dentro del panel donde se escriben los resultados")]
    public TMP_Text textoResultados;

    [Header("Fórmula de puntos")]
    public int puntosPorTarea = 100;
    public int puntosPorError = -50;
    public int puntosEmergencia = 300;
    public int puntosPorSegundoSobrante = 5;

    [Header("Pruebas en el editor")]
    [Tooltip("F6 = tarea, F7 = error, F8 = 20 segundos sobrantes")]
    public bool usarTeclasDePrueba = true;

    [Header("Datos de la partida (solo para mirar)")]
    [SerializeField] private int tareasCompletadas;
    [SerializeField] private int errores;
    [SerializeField] private float segundosSobrantes;
    [SerializeField] private int puntuacionFinal;

    // Nombre con el que se guarda el récord en el equipo.
    private const string ClaveRecord = "GenesisXR_Record";

    void Start()
    {
        if (panelResultados != null) panelResultados.SetActive(false);
    }

    void Update()
    {
        if (!usarTeclasDePrueba || Keyboard.current == null) return;

        if (Keyboard.current.f6Key.wasPressedThisFrame) SumarTarea();
        if (Keyboard.current.f7Key.wasPressedThisFrame) SumarError();
        if (Keyboard.current.f8Key.wasPressedThisFrame) RegistrarSegundosSobrantes(20f);
    }

    // ===== Métodos que llaman las etapas =====

    public void SumarTarea()
    {
        tareasCompletadas++;
        Debug.Log("[Puntos] Tarea completada. Total: " + tareasCompletadas);
    }

    public void SumarError()
    {
        errores++;
        Debug.Log("[Puntos] Error. Total: " + errores);
    }

    // Cata: cuántos segundos le sobraron al controlar la emergencia.
    public void RegistrarSegundosSobrantes(float segundos)
    {
        segundosSobrantes = Mathf.Max(0f, segundos);
        Debug.Log("[Puntos] Segundos sobrantes: " + segundosSobrantes.ToString("0"));
    }

    public void RegistrarSegundosDe(EmergenciaManager emergencia)
    {
        if (emergencia != null) RegistrarSegundosSobrantes(emergencia.TiempoRestante);
    }

    // ===== Cálculo y resultados =====

    public int CalcularPuntuacion()
    {
        int puntos = tareasCompletadas * puntosPorTarea + errores * puntosPorError;

        bool emergenciaControlada = gameManager != null && gameManager.EmergenciaFueControlada;
        if (emergenciaControlada)
        {
            puntos += puntosEmergencia;
            puntos += Mathf.FloorToInt(segundosSobrantes) * puntosPorSegundoSobrante;
        }

        return Mathf.Max(0, puntos); // nunca menos de 0
    }

    // El GameManager llama a estos dos al terminar el juego.
    public void MostrarResultadosExito()   { MostrarResultados(true); }
    public void MostrarResultadosFracaso() { MostrarResultados(false); }

    private void MostrarResultados(bool exito)
    {
        puntuacionFinal = CalcularPuntuacion();

        // Récord guardado en el equipo con PlayerPrefs.
        int record = PlayerPrefs.GetInt(ClaveRecord, 0);
        bool nuevoRecord = puntuacionFinal > record;
        if (nuevoRecord)
        {
            record = puntuacionFinal;
            PlayerPrefs.SetInt(ClaveRecord, record);
            PlayerPrefs.Save();
        }

        float tiempo = gameManager != null ? gameManager.TiempoTotal : Time.timeSinceLevelLoad;
        int minutos = (int)(tiempo / 60f);
        int segundos = (int)(tiempo % 60f);
        bool emergenciaControlada = gameManager != null && gameManager.EmergenciaFueControlada;

        string titulo = exito
            ? "<size=140%><color=#5CFFC8>¡MISIÓN CUMPLIDA!</color></size>"
            : "<size=140%><color=#FF7A59>MISIÓN FALLIDA</color></size>";

        string texto =
            titulo + "\n\n" +
            "Tiempo: <b>" + minutos + ":" + segundos.ToString("00") + "</b>\n" +
            "Tareas completadas: <b>" + tareasCompletadas + "</b>\n" +
            "Errores: <b>" + errores + "</b>\n" +
            "Emergencia controlada: <b>" + (emergenciaControlada ? "Sí" : "No") + "</b>\n\n" +
            "<size=120%>Puntuación: <color=#FFD84D><b>" + puntuacionFinal + "</b></color></size>\n" +
            "Récord: " + record + (nuevoRecord ? "  <color=#FFD84D>¡NUEVO RÉCORD!</color>" : "");

        Debug.Log("[Puntos] RESULTADOS\n" + texto);

        if (textoResultados != null) textoResultados.text = texto;
        if (panelResultados != null) panelResultados.SetActive(true);
    }
}