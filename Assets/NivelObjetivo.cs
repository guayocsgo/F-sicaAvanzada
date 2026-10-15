using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Va en el GameObject raíz de cada nivel (Nivel1, Nivel2...).
/// Cada pallet cuenta cuando llega a la altura meta con todas sus cajas encima y se
/// mantiene ahí un tiempo. El nivel se completa cuando todos los pallets están listos.
/// </summary>
public class NivelObjetivo : MonoBehaviour
{
    /// <summary>Un pallet con sus cajas.</summary>
    [System.Serializable]
    public class Carga
    {
        public Rigidbody pallet;
        public Rigidbody[] cajas;

        // Estado de juego: no se guarda en la escena.
        [System.NonSerialized] public bool lista;
        [System.NonSerialized] public float acumulado;
    }

    [Header("Primer pallet del nivel")]
    public Rigidbody pallet;
    public Rigidbody[] cajas;

    [Header("Pallets extra (solo en niveles con más de uno)")]
    public Carga[] cargasAdicionales;

    [Tooltip("Si está marcado, los pallets tienen que subirse en orden: primero el principal, después los extra.")]
    public bool enOrden = true;

    [Header("Condición (vale para cada pallet)")]
    [Tooltip("Altura (en Y) del origen del Pallet a partir de la cual cuenta como elevado.")]
    public float alturaMeta = 2.8f;

    [Tooltip("Segundos que tiene que mantenerse arriba.")]
    public float tiempoSostenido = 1f;

    [Tooltip("Distancia horizontal máxima de una caja al centro de su pallet.")]
    public float distanciaMaxCaja = 0.5f;

    public bool Completado { get; private set; }

    /// <summary>De 0 a 1: avance del pallet que se está subiendo en este momento.</summary>
    public float Progreso { get; private set; }

    /// <summary>Cantidad de pallets ya logrados.</summary>
    public int CargasListas { get; private set; }

    public int TotalCargas => 1 + (cargasAdicionales != null ? cargasAdicionales.Length : 0);

    readonly List<Carga> cargas = new List<Carga>();

    void Awake()
    {
        cargas.Add(new Carga { pallet = pallet, cajas = cajas });
        if (cargasAdicionales != null) cargas.AddRange(cargasAdicionales);
    }

    void Update()
    {
        if (Completado) return;

        Progreso = 0f;

        foreach (Carga c in cargas)
        {
            if (c.lista) continue;

            Evaluar(c);

            if (c.lista) CargasListas++;
            else Progreso = Mathf.Max(Progreso, Mathf.Clamp01(c.acumulado / tiempoSostenido));

            // En orden: solo cuenta el primer pallet que todavía falta.
            if (enOrden) break;
        }

        if (CargasListas >= cargas.Count)
            Completado = true;
    }

    void Evaluar(Carga c)
    {
        bool cumple = c.pallet != null
                      && c.pallet.position.y >= alturaMeta
                      && CajasSobrePallet(c);

        c.acumulado = cumple ? c.acumulado + Time.deltaTime : 0f;

        if (c.acumulado >= tiempoSostenido)
            c.lista = true;
    }

    // Todas las cajas tienen que seguir arriba del pallet y cerca de su centro.
    bool CajasSobrePallet(Carga c)
    {
        if (c.cajas == null) return true;

        Vector3 p = c.pallet.position;

        foreach (Rigidbody caja in c.cajas)
        {
            if (caja == null) return false;

            Vector3 d = caja.position - p;
            if (d.y < 0.2f) return false;
            if (new Vector2(d.x, d.z).magnitude > distanciaMaxCaja) return false;
        }
        return true;
    }
}