using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


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

    [Header("Final del juego")]
    [Tooltip("Pantalla que se muestra al completar el último nivel. Si está vacía, el juego vuelve al nivel 1.")]
    public PantallaVictoria pantallaVictoria;

    
    static int nivelActual;

    NivelObjetivo activo;
    bool cambiando;
    int cargasMostradas;
    float mensajeHasta;

   
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

        if (!hayMasNiveles && pantallaVictoria != null)
        {
            pantallaVictoria.Mostrar();
            yield break;
        }

        nivelActual = hayMasNiveles ? nivelActual + 1 : 0;
        RecargarEscena();
    }

    void Sonar(AudioClip clip)
    {
        if (fuenteEfectos != null && clip != null)
            fuenteEfectos.PlayOneShot(clip);
    }

    
    public void ReiniciarNivel()
    {
        RecargarEscena();
    }

    
    public void VolverAlPrimerNivel()
    {
        nivelActual = 0;
        RecargarEscena();
    }

    
    public static void ReiniciarProgreso()
    {
        nivelActual = 0;
    }

    static void RecargarEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}