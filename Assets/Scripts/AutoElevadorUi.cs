using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class AutoelevadorUI : MonoBehaviour
{
    
    [System.Serializable]
    public class FilaUI
    {
        public int indiceJunta;   
        public Slider slider;     
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

            
            f.slider.minValue = j.Minimo;
            f.slider.maxValue = j.Maximo;
            f.slider.SetValueWithoutNotify(Mathf.Clamp(j.Target, j.Minimo, j.Maximo));

            int i = f.indiceJunta; 
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

   
    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}