using UnityEngine;
using TMPro;


public class CamaraSelector : MonoBehaviour
{
    [Tooltip("Cámaras en el orden en que se alternan. La primera es la que se ve al empezar.")]
    public Camera[] camaras;

    [Tooltip("Texto del botón (opcional): muestra el nombre de la cámara activa.")]
    public TMP_Text textoBoton;

    int actual;

    void Start()
    {
        Activar(0);
    }

   
    public void SiguienteCamara()
    {
        if (camaras == null || camaras.Length == 0) return;
        Activar((actual + 1) % camaras.Length);
    }

    void Activar(int indice)
    {
        if (camaras == null || camaras.Length == 0) return;

        actual = indice;

        for (int i = 0; i < camaras.Length; i++)
            camaras[i].gameObject.SetActive(i == actual);

        if (textoBoton != null)
            textoBoton.text = $"Cámara: {camaras[actual].name}";
    }
}