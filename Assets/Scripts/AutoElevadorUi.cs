using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Va en el Canvas. Conecta los sliders con los targets y muestra target / valor real por junta.
/// </summary>
public class AutoelevadorUI : MonoBehaviour
{
    /// <summary>Una fila de la UI: qué junta muestra, su slider (vacío si es pasiva) y su texto.</summary>
    [System.Serializable]
    public class FilaUI
    {
        public int indiceJunta;   // posición en AutoelevadorController.juntas
        public Slider slider;     // dejar vacío en la junta pasiva (Tope)
        public TMP_Text texto;
    }

    public AutoelevadorController control;
    public FilaUI[] filas;

    void Start()
    {
        foreach (FilaUI f in filas)
        {
            if (f.slider == null) continue;

            AutoelevadorController.Junta j = control.juntas[f.indiceJunta];

            // El rango del slider sale de los límites del drive.
            f.slider.minValue = j.Minimo;
            f.slider.maxValue = j.Maximo;
            f.slider.SetValueWithoutNotify(Mathf.Clamp(j.Target, j.Minimo, j.Maximo));

            int i = f.indiceJunta; // copia local para el lambda
            f.slider.onValueChanged.AddListener(v => control.juntas[i].deseado = v);
        }
    }

    void Update()
    {
        foreach (FilaUI f in filas)
        {
            AutoelevadorController.Junta j = control.juntas[f.indiceJunta];
            f.texto.text = $"{j.nombre}\nTarget: {j.Target:F2}{j.Unidad}   |   Real: {j.Real:F2}{j.Unidad}";
        }
    }

    /// <summary>Para el botón Reiniciar: recarga la escena.</summary>
    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}