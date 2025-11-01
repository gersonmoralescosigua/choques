using UnityEngine;

/// <summary>
/// Componente que controla datos del carro y permite aplicar/resetear velocidades y masa.
/// No aplica la velocidad automáticamente en Start: la SimulationManager lo hará cuando se presione "Iniciar".
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class CarController : MonoBehaviour
{
    [Header("Configuración (editable en Inspector)")]
    [Tooltip("Masa del carro (kg). Esto se aplica automáticamente al Rigidbody2D en Awake().")]
    public float mass = 3f;

    [Tooltip("Velocidad inicial (magnitud en m/s). Signo se aplica según initialDirectionRight.")]
    public float initialVelocityMagnitude = 5f;

    [Tooltip("Dirección inicial: true = hacia la derecha (+X). false = hacia la izquierda (-X).")]
    public bool initialDirectionRight = true;

    [Header("Opcionales (solo lectura en Play)")]
    [Tooltip("Posición inicial (guardada en Awake) para poder reiniciar la escena.")]
    public Vector3 initialPosition;

    [Tooltip("Referencia al Rigidbody2D del carro.")]
    public Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Aplicar masa configurada (permite editar en Inspector)
        if (rb != null)
        {
            rb.mass = mass;
            // Asegurar constraints coherentes
            rb.freezeRotation = true;
        }
        // Guardar posición inicial para reset
        initialPosition = transform.position;
    }

    /// <summary>
    /// Aplica la velocidad inicial (usada por SimulationManager).
    /// El signo se aplica en base a initialDirectionRight.
    /// </summary>
    public void ApplyInitialVelocity(float overrideMagnitude = float.NaN)
    {
        float mag = float.IsNaN(overrideMagnitude) ? initialVelocityMagnitude : overrideMagnitude;
        float sign = initialDirectionRight ? 1f : -1f;
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(mag * sign, 0f);
        }
    }

    /// <summary>
    /// Fuerza la masa del rigidbody (útil si se quiere actualizar en tiempo de ejecución).
    /// </summary>
    public void ApplyMass(float newMass)
    {
        mass = newMass;
        if (rb != null) rb.mass = mass;
    }

    /// <summary>
    /// Resetea posición y velocidad al estado inicial guardado.
    /// </summary>
    public void ResetToInitial()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        transform.position = initialPosition;
    }
}
