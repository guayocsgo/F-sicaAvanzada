using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuPrincipal : MonoBehaviour
{
    [Tooltip("Nombre exacto de la escena del simulador (tiene que estar en la Scene List del build).")]
    public string escenaJuego = "SampleScene";

    [Tooltip("Texto de despedida que se muestra en WebGL. Tiene que arrancar desactivado.")]
    public GameObject mensajeDespedida;

   
    public void Empezar()
    {
        SceneManager.LoadScene(escenaJuego);
    }

   
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