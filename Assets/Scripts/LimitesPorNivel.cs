using UnityEngine;


[DefaultExecutionOrder(-50)] 
public class LimitesPorNivel : MonoBehaviour
{
    [Header("Elevación")]
    [Tooltip("El objeto Carro (el que tiene el Articulation Body Prismatic en Y).")]
    public ArticulationBody carro;

    [Tooltip("Nuevo límite superior de elevación, en metros.")]
    public float limiteElevacion = 5f;

    [Header("Mástil (solo visual)")]
    [Tooltip("El hijo Visual del Mastil. Se estira para acompañar la nueva altura.")]
    public Transform visualMastil;

    [Tooltip("Cuánto sobresale el mástil por encima del límite de elevación.")]
    public float margenMastil = 0.6f;

    [Header("Opcional")]
    public AutoelevadorController control;

    [Tooltip("Nueva velocidad de elevación (m/s). 0 = no cambia.")]
    public float velocidadElevacion = 0f;

    void Start()
    {
        
        ArticulationDrive d = carro.yDrive;
        d.upperLimit = limiteElevacion;
        carro.yDrive = d;

        if (visualMastil != null)
        {
            float altura = limiteElevacion + margenMastil;

            Vector3 escala = visualMastil.localScale;
            escala.y = altura;
            visualMastil.localScale = escala;

            Vector3 pos = visualMastil.localPosition;
            pos.y = altura / 2f;
            visualMastil.localPosition = pos;
        }

        if (control != null && velocidadElevacion > 0f)
        {
            foreach (AutoelevadorController.Junta j in control.juntas)
                if (j.nombre == "Elevación") j.velocidadMax = velocidadElevacion;
        }
    }
}