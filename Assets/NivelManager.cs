using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Va en un GameObject vacío. Activa solo el nivel actual, avisa cuando se completa
/// y recarga la escena con el nivel siguiente (así el autoelevador y la física
/// arrancan siempre limpios).
/// </summary>
public class NivelManager : MonoBehaviour
{
    [Tooltip("Un elemento por nivel: el GameObject raíz que tiene NivelObjetivo.")]
    public NivelObjetivo[] niveles;

    public TMP_Text txtNivel;
    public TMP_Text txtMensaje;

    [Tooltip("Segundos que se muestra el cartel antes de pasar de nivel.")]
    public float esperaAntesDeCambiar = 3f;

    // static: se conserva cuando se recarga la escena.
    static int nivelActual;

    NivelObjetivo activo;
    bool cambiando;

    // Por si el editor tiene desactivado el "Reload Domain": cada Play arranca en el nivel 1.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ReiniciarEstatico()
    {
        nivelActual = 0;
    }

    void Awake()
    {
        nivelActual = Mathf.Clamp(nivelActual, 0, niveles.Length - 1);

        for (int i = 0; i < niveles.Length; i++)
            niveles[i].gameObject.SetActive(i == nivelActual);

        activo = niveles[nivelActual];
    }

    void Start()
    {
        txtNivel.text = $"Nivel {nivelActual + 1} / {niveles.Length}";
        txtMensaje.text = "";
    }

    void Update()
    {
        if (cambiando) return;

        if (activo.Completado)
            StartCoroutine(CompletarNivel());
        else if (activo.Progreso > 0f)
            txtMensaje.text = $"¡Mantenelo arriba! {activo.Progreso * 100f:F0}%";
        else
            txtMensaje.text = "";
    }

    IEnumerator CompletarNivel()
    {
        cambiando = true;

        bool hayMasNiveles = nivelActual < niveles.Length - 1;
        txtMensaje.text = hayMasNiveles
            ? $"¡Nivel {nivelActual + 1} completado!"
            : "¡Completaste todos los niveles!";

        yield return new WaitForSeconds(esperaAntesDeCambiar);

        nivelActual = hayMasNiveles ? nivelActual + 1 : 0;
        RecargarEscena();
    }

    /// <summary>Botón: reinicia el nivel actual.</summary>
    public void ReiniciarNivel()
    {
        RecargarEscena();
    }

    /// <summary>Botón: vuelve al nivel 1.</summary>
    public void VolverAlPrimerNivel()
    {
        nivelActual = 0;
        RecargarEscena();
    }

    static void RecargarEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}