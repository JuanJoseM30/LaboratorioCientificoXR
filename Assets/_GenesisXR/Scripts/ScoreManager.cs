using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

// Lleva la cuenta de la partida y calcula la puntuación final.
public class ScoreManager : MonoBehaviour
{
    [Header("Referencias")]
    public GameManager gameManager;
    [Tooltip("Texto del panel donde se muestran los resultados (opcional)")]
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

    void Update()
    {
        if (!usarTeclasDePrueba || Keyboard.current == null) return;

        if (Keyboard.current.f6Key.wasPressedThisFrame) SumarTarea();
        if (Keyboard.current.f7Key.wasPressedThisFrame) SumarError();
        if (Keyboard.current.f8Key.wasPressedThisFrame) RegistrarSegundosSobrantes(20f);
    }

    // ===== Métodos que llaman las etapas =====

    // Cada vez que el jugador completa una tarea.
    public void SumarTarea()
    {
        tareasCompletadas++;
        Debug.Log("[Puntos] Tarea completada. Total: " + tareasCompletadas);
    }

    // Cada vez que el jugador se equivoca (por ejemplo, un reactivo incorrecto).
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

        string texto =
            (exito ? "ÉXITO" : "FRACASO") + "\n" +
            "Tiempo: " + minutos + ":" + segundos.ToString("00") + "\n" +
            "Tareas completadas: " + tareasCompletadas + "\n" +
            "Errores: " + errores + "\n" +
            "Emergencia controlada: " + (emergenciaControlada ? "Sí" : "No") + "\n" +
            "Puntuación: " + puntuacionFinal + "\n" +
            "Récord: " + record + (nuevoRecord ? "  ¡NUEVO RÉCORD!" : "");

        Debug.Log("[Puntos] RESULTADOS\n" + texto);

        if (textoResultados != null)
        {
            textoResultados.text = texto;
            textoResultados.gameObject.SetActive(true);
        }
    }
}