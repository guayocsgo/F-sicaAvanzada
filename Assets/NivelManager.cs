using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Va en un GameObject vacío. Activa solo el nivel actual, avisa cuando se completa
/// cada pallet y el nivel entero, y recarga la escena con el nivel siguiente
/// (así el autoelevador y la física arrancan siempre limpios).
/// </summary>
public class NivelManager : MonoBehaviour
{
    [Tooltip("Un elemento por nivel: el GameObject raíz que tiene NivelObjetivo.")]
    public NivelObjetivo[] niveles;

    public TMP_Text txtNivel;
    public TMP_Text txtMensaje;

    [Tooltip("Segundos que se muestra el cartel antes de pasar de nivel.")]
    public float esperaAntesDeCambiar = 3f;

    [Header("Sonidos")]
    public AudioSource fuenteEfectos;
    public AudioClip sonidoCargaCompletada;
    public AudioClip sonidoNivelCompletado;

    // static: se conserva cuando se recarga la escena.
    static int nivelActual;

    NivelObjetivo activo;
    bool cambiando;
    int cargasMostradas;
    float mensajeHasta;

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
        ActualizarTitulo();
        txtMensaje.text = "";
    }

    void Update()
    {
        if (cambiando) return;

        if (activo.Completado)
        {
            StartCoroutine(CompletarNivel());
            return;
        }

        // Se completó un pallet (pero faltan otros): cartel por unos segundos.
        if (activo.CargasListas > cargasMostradas)
        {
            cargasMostradas = activo.CargasListas;
            ActualizarTitulo();
            mensajeHasta = Time.time + 3f;
            txtMensaje.text = $"¡Pallet {cargasMostradas} de {activo.TotalCargas} listo! Ahora el siguiente";
            Sonar(sonidoCargaCompletada);
            return;
        }

        if (Time.time < mensajeHasta) return;

        txtMensaje.text = activo.Progreso > 0f
            ? $"¡Mantenelo arriba! {activo.Progreso * 100f:F0}%"
            : "";
    }

    void ActualizarTitulo()
    {
        string titulo = $"Nivel {nivelActual + 1} / {niveles.Length}";

        if (activo.TotalCargas > 1)
            titulo += $"   Pallets {activo.CargasListas} / {activo.TotalCargas}";

        txtNivel.text = titulo;
    }

    IEnumerator CompletarNivel()
    {
        cambiando = true;

        bool hayMasNiveles = nivelActual < niveles.Length - 1;
        txtMensaje.text = hayMasNiveles
            ? $"¡Nivel {nivelActual + 1} completado!"
            : "¡Completaste todos los niveles!";
        Sonar(sonidoNivelCompletado);

        yield return new WaitForSeconds(esperaAntesDeCambiar);

        nivelActual = hayMasNiveles ? nivelActual + 1 : 0;
        RecargarEscena();
    }

    void Sonar(AudioClip clip)
    {
        if (fuenteEfectos != null && clip != null)
            fuenteEfectos.PlayOneShot(clip);
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