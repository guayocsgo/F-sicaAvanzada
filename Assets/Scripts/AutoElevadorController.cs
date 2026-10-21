using UnityEngine;


public class AutoelevadorController : MonoBehaviour
{
    public enum EjeJoint { X, Y, Z }

    
    [System.Serializable]
    public class Junta
    {
        public string nombre = "Junta";
        public ArticulationBody cuerpo;

        [Tooltip("Eje del drive: Prismatic = el Axis elegido. Revolute = X.")]
        public EjeJoint eje = EjeJoint.X;

        [Tooltip("Marcar si es Revolute (grados). Desmarcar si es Prismatic (metros).")]
        public bool esRotacion;

        [Tooltip("Velocidad máxima a la que se mueve el target (m/s o grados/s).")]
        public float velocidadMax = 1f;

        [Tooltip("Desmarcar para una junta pasiva: solo se lee, no se controla.")]
        public bool controlada = true;

        
        [HideInInspector] public float deseado;

        public string Unidad => esRotacion ? "°" : " m";

        
        ArticulationDrive LeerDrive()
        {
            switch (eje)
            {
                case EjeJoint.X: return cuerpo.xDrive;
                case EjeJoint.Y: return cuerpo.yDrive;
                default: return cuerpo.zDrive;
            }
        }

        void EscribirDrive(ArticulationDrive d)
        {
            switch (eje)
            {
                case EjeJoint.X: cuerpo.xDrive = d; break;
                case EjeJoint.Y: cuerpo.yDrive = d; break;
                default: cuerpo.zDrive = d; break;
            }
        }

        public float Minimo => LeerDrive().lowerLimit;
        public float Maximo => LeerDrive().upperLimit;

       
        public float Target => LeerDrive().target;

        
        public float Real
        {
            get
            {
                if (cuerpo == null || cuerpo.dofCount == 0) return 0f;
                float v = cuerpo.jointPosition[0];
                return esRotacion ? v * Mathf.Rad2Deg : v;
            }
        }

        
        public void Mover(float dt)
        {
            if (!controlada || cuerpo == null) return;

            ArticulationDrive d = LeerDrive();
            float meta = Mathf.Clamp(deseado, d.lowerLimit, d.upperLimit);
            d.target = Mathf.MoveTowards(d.target, meta, velocidadMax * dt);
            EscribirDrive(d);
        }
    }

    public Junta[] juntas = new Junta[]
    {
        new Junta { nombre = "Avance",        eje = EjeJoint.Z, esRotacion = false, velocidadMax = 1.5f },
        new Junta { nombre = "Inclinación",   eje = EjeJoint.X, esRotacion = true,  velocidadMax = 8f   },
        new Junta { nombre = "Elevación",     eje = EjeJoint.Y, esRotacion = false, velocidadMax = 0.5f },
        new Junta { nombre = "Lateral",       eje = EjeJoint.X, esRotacion = false, velocidadMax = 0.1f },
        new Junta { nombre = "Tope (pasivo)", eje = EjeJoint.X, esRotacion = true,  velocidadMax = 0f, controlada = false },
    };

    void Start()
    {
        
        foreach (Junta j in juntas)
            if (j.cuerpo != null) j.deseado = j.Target;
    }

    void FixedUpdate()
    {
        foreach (Junta j in juntas)
            j.Mover(Time.fixedDeltaTime);
    }
}