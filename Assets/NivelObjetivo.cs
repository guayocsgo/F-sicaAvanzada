using UnityEngine;

/// <summary>
/// Va en el GameObject raíz de cada nivel (Nivel1, Nivel2...).
/// El nivel se completa cuando el pallet llega a la altura meta con todas las
/// cajas encima y se mantiene ahí durante un tiempo.
/// </summary>
public class NivelObjetivo : MonoBehaviour
{
    [Header("Objetos de este nivel")]
    public Rigidbody pallet;
    public Rigidbody[] cajas;

    [Header("Condición para completar")]
    [Tooltip("Altura (en Y) del origen del Pallet a partir de la cual cuenta como elevado.")]
    public float alturaMeta = 2.8f;

    [Tooltip("Segundos que tiene que mantenerse arriba.")]
    public float tiempoSostenido = 1f;

    [Tooltip("Distancia horizontal máxima de una caja al centro del pallet.")]
    public float distanciaMaxCaja = 0.5f;

    public bool Completado { get; private set; }

    /// <summary>De 0 a 1: cuánto del tiempo sostenido lleva acumulado.</summary>
    public float Progreso { get; private set; }

    float acumulado;

    void Update()
    {
        if (Completado || pallet == null) return;

        bool cumple = pallet.position.y >= alturaMeta && CajasSobrePallet();
        acumulado = cumple ? acumulado + Time.deltaTime : 0f;
        Progreso = Mathf.Clamp01(acumulado / tiempoSostenido);

        if (acumulado >= tiempoSostenido)
            Completado = true;
    }

    // Todas las cajas tienen que seguir arriba del pallet y cerca de su centro.
    bool CajasSobrePallet()
    {
        Vector3 p = pallet.position;

        foreach (Rigidbody c in cajas)
        {
            if (c == null) return false;

            Vector3 d = c.position - p;
            if (d.y < 0.2f) return false;
            if (new Vector2(d.x, d.z).magnitude > distanciaMaxCaja) return false;
        }
        return true;
    }
}