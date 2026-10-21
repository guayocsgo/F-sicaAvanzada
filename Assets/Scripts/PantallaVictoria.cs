using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class PantallaVictoria : MonoBehaviour
{
    [Header("Textos")]
    public string titulo = "GANASTE";
    public string subtitulo = "Completaste todos los niveles";
    public string textoBoton = "Volver al menú";

    [Header("Colores")]
    public Color colorFondo = new Color(0f, 0f, 0f, 0.8f);
    public Color colorTitulo = new Color(1f, 0.8f, 0.1f);
    public Color colorBoton = new Color(0.95f, 0.72f, 0.02f);
    public Color colorTextoBoton = new Color(0.1f, 0.1f, 0.1f);

    [Header("Navegación")]
    [Tooltip("Nombre exacto de la escena del menú (tiene que estar en la Scene List del build).")]
    public string escenaMenu = "MenuPrincipal";

    GameObject panel;
    RectTransform rtTitulo;

    
    public void Mostrar()
    {
        if (panel == null) Construir();

        panel.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(AnimarTitulo());
    }

    void Construir()
    {
        
        panel = new GameObject("PantallaVictoriaCanvas", typeof(RectTransform));
        panel.transform.SetParent(transform, false);

        Canvas canvas = panel.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        CanvasScaler escalador = panel.AddComponent<CanvasScaler>();
        escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(1920f, 1080f);
        escalador.matchWidthOrHeight = 0.5f;

        panel.AddComponent<GraphicRaycaster>();

        
        GameObject fondo = new GameObject("Fondo", typeof(RectTransform), typeof(Image));
        fondo.transform.SetParent(panel.transform, false);
        RectTransform rf = (RectTransform)fondo.transform;
        rf.anchorMin = Vector2.zero;
        rf.anchorMax = Vector2.one;
        rf.offsetMin = Vector2.zero;
        rf.offsetMax = Vector2.zero;
        fondo.GetComponent<Image>().color = colorFondo;

        
        TextMeshProUGUI txtTitulo = CrearTexto(panel.transform, "Titulo", titulo, 220f,
            colorTitulo, new Vector2(0f, 160f), new Vector2(1600f, 280f));
        rtTitulo = txtTitulo.rectTransform;

        CrearTexto(panel.transform, "Subtitulo", subtitulo, 56f,
            Color.white, new Vector2(0f, -40f), new Vector2(1400f, 100f));

        
        GameObject boton = new GameObject("BtnVolverAlMenu", typeof(RectTransform), typeof(Image), typeof(Button));
        boton.transform.SetParent(panel.transform, false);

        RectTransform rb = (RectTransform)boton.transform;
        rb.anchorMin = rb.anchorMax = new Vector2(0.5f, 0.5f);
        rb.sizeDelta = new Vector2(520f, 120f);
        rb.anchoredPosition = new Vector2(0f, -230f);

        Image imagen = boton.GetComponent<Image>();
        imagen.color = colorBoton;

        Button b = boton.GetComponent<Button>();
        b.targetGraphic = imagen;
        ColorBlock colores = b.colors;
        colores.normalColor = new Color(0.92f, 0.92f, 0.92f);
        colores.highlightedColor = Color.white;
        colores.pressedColor = new Color(0.7f, 0.7f, 0.7f);
        b.colors = colores;
        b.onClick.AddListener(VolverAlMenu);

        CrearTexto(boton.transform, "Texto", textoBoton, 56f,
            colorTextoBoton, Vector2.zero, new Vector2(500f, 100f));
    }

    TextMeshProUGUI CrearTexto(Transform padre, string nombre, string contenido, float tamano,
                               Color color, Vector2 posicion, Vector2 medida)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);

        TextMeshProUGUI t = go.AddComponent<TextMeshProUGUI>();
        t.text = contenido;
        t.fontSize = tamano;
        t.fontStyle = FontStyles.Bold;
        t.color = color;
        t.alignment = TextAlignmentOptions.Center;
        t.raycastTarget = false;

        RectTransform r = t.rectTransform;
        r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
        r.sizeDelta = medida;
        r.anchoredPosition = posicion;
        return t;
    }

   
    IEnumerator AnimarTitulo()
    {
        const float duracion = 0.6f;
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;

        float t = 0f;
        rtTitulo.localScale = Vector3.zero;

        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            float x = Mathf.Clamp01(t / duracion);
            float e = 1f + c3 * Mathf.Pow(x - 1f, 3) + c1 * Mathf.Pow(x - 1f, 2);
            rtTitulo.localScale = Vector3.one * e;
            yield return null;
        }

        rtTitulo.localScale = Vector3.one;
    }

    void VolverAlMenu()
    {
        NivelManager.ReiniciarProgreso(); 
        SceneManager.LoadScene(escenaMenu);
    }
}