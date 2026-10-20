using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Va en un GameObject vacío de la escena del menú.
/// Empezar carga la escena del simulador. Salir cierra el juego
/// (en WebGL no se puede cerrar la pestaña, así que muestra un mensaje).
/// </summary>
public class MenuPrincipal : MonoBehaviour
{
    [Tooltip("Nombre exacto de la escena del simulador (tiene que estar en la Scene List del build).")]
    public string escenaJuego = "SampleScene";

    [Tooltip("Texto de despedida que se muestra en WebGL. Tiene que arrancar desactivado.")]
    public GameObject mensajeDespedida;

    /// <summary>Botón Empezar.</summary>
    public void Empezar()
    {
        SceneManager.LoadScene(escenaJuego);
    }

    /// <summary>Botón Salir.</summary>
    public void Salir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        // En el navegador Application.Quit() no hace nada.
        if (mensajeDespedida != null) mensajeDespedida.SetActive(true);
#else
        Application.Quit();
#endif
    }
}