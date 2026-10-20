using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Va en un GameObject con un AudioSource. Ponelo en la escena del menú Y en la del juego:
/// la copia que llega primero sobrevive entre escenas y la otra se destruye sola.
/// Elige la música según la escena y no la reinicia cuando se recarga el nivel.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MusicaManager : MonoBehaviour
{
    [Header("Pistas")]
    public AudioClip musicaMenu;
    public AudioClip musicaJuego;

    [Tooltip("Nombre exacto de la escena del menú. Cualquier otra escena usa la música del juego.")]
    public string escenaMenu = "MenuPrincipal";

    [Header("Volumen")]
    [Range(0f, 1f)] public float volumen = 0.4f;

    static MusicaManager instancia;
    AudioSource fuente;

    void Awake()
    {
        // Si ya existe una (viene de la escena anterior), esta copia sobra.
        if (instancia != null)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);

        fuente = GetComponent<AudioSource>();
        fuente.loop = true;
        fuente.playOnAwake = false;
        fuente.spatialBlend = 0f; // 2D
        fuente.volume = volumen;

        SceneManager.sceneLoaded += AlCargarEscena;
        Reproducir(ClipPara(SceneManager.GetActiveScene().name));
    }

    void OnDestroy()
    {
        if (instancia == this)
        {
            SceneManager.sceneLoaded -= AlCargarEscena;
            instancia = null;
        }
    }

    void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        Reproducir(ClipPara(escena.name));
    }

    AudioClip ClipPara(string nombreEscena)
    {
        return nombreEscena == escenaMenu ? musicaMenu : musicaJuego;
    }

    void Reproducir(AudioClip clip)
    {
        // Misma pista que ya está sonando (por ejemplo, al pasar de nivel): no la reinicia.
        if (clip == null || (fuente.clip == clip && fuente.isPlaying)) return;

        fuente.clip = clip;
        fuente.Play();
    }
}