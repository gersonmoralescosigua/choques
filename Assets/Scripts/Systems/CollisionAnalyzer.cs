using System.Collections;
using UnityEngine;
using TMPro;

public class CollisionAnalyzer : MonoBehaviour
{
    [Header("Referencias")]
    public CarController car1;
    public CarController car2;
    public SimulationManager simulationManager;

    [Header("Debug")]
    public bool enableDebug = true;
    
    // Variables para muestreo preciso
    private Vector2 velocityBeforeCollision1;
    private Vector2 velocityBeforeCollision2;
    private bool collisionDetected = false;
    private bool hasRecordedVelocities = false;
    private bool simulationCompleted = false; // NUEVA: Evitar múltiples análisis

    void FixedUpdate()
    {
        // SOLUCIÓN: Siempre muestrear velocidades, pero guardarlas solo si no hay colisión en progreso
        if (!collisionDetected && !simulationCompleted)
        {
            velocityBeforeCollision1 = car1.rb.linearVelocity;
            velocityBeforeCollision2 = car2.rb.linearVelocity;
            hasRecordedVelocities = true;
            
            if (enableDebug)
                Debug.Log($"[SAMPLING] Car1: {velocityBeforeCollision1.x:F2}, Car2: {velocityBeforeCollision2.x:F2}");
        }
    }

    public void OnCarsCollisionEnter()
    {

        // NUEVO: Debug de materiales
    Debug.Log($"🔧 MATERIALES - Car1: {car1.GetComponent<Collider2D>().sharedMaterial?.bounciness}, Car2: {car2.GetComponent<Collider2D>().sharedMaterial?.bounciness}");
        if (collisionDetected || simulationCompleted) return; 
        
        // NUEVO: Evitar múltiples análisis
        
        if (enableDebug) 
            Debug.Log("🎯 COLISIÓN DETECTADA - Iniciando análisis...");

        collisionDetected = true;
        
        if (!hasRecordedVelocities)
        {
            Debug.LogError("❌ ERROR: No se registraron velocidades antes de la colisión");
            return;
        }

        StartCoroutine(AnalyzeCollisionCoroutine());
    }

    private IEnumerator AnalyzeCollisionCoroutine()
    {
        // PASO 1: Usar velocidades guardadas ANTES de la colisión
        float v1_before = velocityBeforeCollision1.x;
        float v2_before = velocityBeforeCollision2.x;

        if (enableDebug)
            Debug.Log($"📊 ANTES - Car1: {v1_before:F2}, Car2: {v2_before:F2}");

        // PASO 2: Esperar para que Unity procese la colisión
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForFixedUpdate();

        // PASO 3: Medir velocidades DESPUÉS
        float v1_after = car1.rb.linearVelocity.x;
        float v2_after = car2.rb.linearVelocity.x;

        if (enableDebug)
            Debug.Log($"📊 DESPUÉS - Car1: {v1_after:F2}, Car2: {v2_after:F2}");

        // PASO 4: Calcular coeficiente de restitución
        float denominator = v1_before - v2_before;
        
        if (enableDebug)
            Debug.Log($"🧮 Denominador: {denominator:F2}");

        float e_calculated = 0f;
        bool e_computed = false;

        if (Mathf.Abs(denominator) > 0.01f) // Evitar división por cero
        {
            e_calculated = (v2_after - v1_after) / denominator;
            e_computed = true;
            
            // Asegurar que e esté en rango físico [0, 1]
            e_calculated = Mathf.Clamp(e_calculated, 0f, 1f);
            
            if (enableDebug)
                Debug.Log($"✅ e CALCULADO: {e_calculated:F3}");
        }
        else
        {
            Debug.LogWarning("⚠️ Denominador muy pequeño, no se puede calcular e");
        }

        // PASO 5: Calcular energías
        float m1 = car1.rb.mass;
        float m2 = car2.rb.mass;
        
        float Ek_before = 0.5f * m1 * v1_before * v1_before + 0.5f * m2 * v2_before * v2_before;
        float Ek_after = 0.5f * m1 * v1_after * v1_after + 0.5f * m2 * v2_after * v2_after;
        float percentEnergyConserved = (Ek_before > 0) ? (Ek_after / Ek_before) * 100f : 0f;

        // PASO 6: Determinar tipo de choque
        string collisionType = DetermineCollisionType(e_computed ? e_calculated : -1f, percentEnergyConserved);

        // PASO 7: Notificar al SimulationManager
        if (simulationManager != null)
        {
            simulationManager.OnCollisionResults(
                v1_before, v2_before, v1_after, v2_after,
                e_calculated,
                Ek_before, Ek_after,
                m1 * v1_before + m2 * v2_before, // momentum before
                m1 * v1_after + m2 * v2_after,   // momentum after  
                percentEnergyConserved,
                collisionType
            );
        }

        // NUEVO: Detener los carros después del análisis para evitar múltiples choques
        yield return new WaitForSeconds(0.5f);
        car1.rb.linearVelocity = Vector2.zero;
        car2.rb.linearVelocity = Vector2.zero;

        // Resetear para próxima simulación
        collisionDetected = false;
        hasRecordedVelocities = false;
        simulationCompleted = true; // NUEVO: Marcar simulación como completada
    }

    private string DetermineCollisionType(float e, float energyConserved)
    {
        if (e < 0) // No se pudo calcular e
        {
            if (energyConserved >= 95f) return "ELÁSTICO";
            if (energyConserved >= 50f) return "INELÁSTICO";
            return "PERFECTAMENTE INELÁSTICO";
        }

        // Usar e para clasificación
        if (e >= 0.95f) return "ELÁSTICO";
        if (e >= 0.5f) return "PARCIALMENTE ELÁSTICO";
        if (e > 0.1f) return "INELÁSTICO";
        return "PERFECTAMENTE INELÁSTICO";
    }

    // Llamar este método al iniciar/resetear simulación
    public void StartSimulation()
    {
        collisionDetected = false;
        hasRecordedVelocities = false;
        simulationCompleted = false; // NUEVO: Permitir nueva simulación
    }
}




/*using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Detecta colisión entre dos carros (1D en X), captura velocidades pre/post, calcula e, energías y notifica al SimulationManager.
/// Adjuntar a un GameObject central (ej: CollisionSystem) y arrastrar referencias a car1/car2 y manager.
/// </summary>
public class CollisionAnalyzer : MonoBehaviour
{
    [Header("Referencias")]
    public CarController car1; // por convención: Car1 (izquierda o derecha según config)
    public CarController car2;
    public SimulationManager simulationManager; // para notificar resultados

    [Header("Ajustes")]
    [Tooltip("Si true: después de detectar colisión, aplicamos las velocidades analíticas calculadas (determinista).")]
    public bool applyAnalyticalPostVelocities = false;

    [Tooltip("Tiempo en segundos para esperar y muestrear post-colisión si no hay OnCollisionExit. (ej: si se quedan 'pegados').")]
    public float postCollisionSampleDelay = 0.05f; // 1-2 frames = 0.02-0.04s por defecto

    // almacenamiento de velocidades previas (actualizadas en FixedUpdate)
    private Vector2 lastVel1;
    private Vector2 lastVel2;

    // estado
    private bool collisionInProgress = false;

    void FixedUpdate()
    {
        if (car1?.rb != null) lastVel1 = car1.rb.linearVelocity;
        if (car2?.rb != null) lastVel2 = car2.rb.linearVelocity;
    }

    // Este método se debe llamar desde OnCollisionEnter2D de alguno de los carros, o lo llamamos aquí si preferimos:
    public void OnCarsCollisionEnter()
    {
        if (collisionInProgress) return;
        collisionInProgress = true;

        // Velocidades justo antes del choque (las guardadas en FixedUpdate)
        float v1_before = lastVel1.x;
        float v2_before = lastVel2.x;

        // Lanzamos coroutine que intentará obtener v después del choque
        StartCoroutine(HandleCollisionSequence(v1_before, v2_before));
    }

    private IEnumerator HandleCollisionSequence(float v1_before, float v2_before)
    {
        // Espera un frame de física para que Unity calcule la respuesta inmediata
        yield return new WaitForFixedUpdate();

        // Intento 1: leer las velocidades inmediatamente después
        float v1_after = car1.rb.linearVelocity.x;
        float v2_after = car2.rb.linearVelocity.x;

        // Si las velocidades son idénticas a antes (ej: se "pegó"), espera un pequeño tiempo extra y vuelve a leer
        if (Mathf.Approximately(v1_after, v1_before) && Mathf.Approximately(v2_after, v2_before))
        {
            yield return new WaitForSeconds(postCollisionSampleDelay);
            v1_after = car1.rb.linearVelocity.x;
            v2_after = car2.rb.linearVelocity.x;
        }

        // Si se desea forzar las velocidades analíticas, calculamos e usando v_after (si se midió), o usando e = 1 por defecto.
        // First compute e_measured (si denominador no es 0)
        float denom = (v1_before - v2_before);
        float e_measured = 0f;
        bool computed = false;
        if (!Mathf.Approximately(denom, 0f))
        {
            e_measured = (v2_after - v1_after) / denom;
            computed = true;
        }

        // Si se solicita aplicar solución analítica (determinista), calculamos v1,v2 usando la fórmula y el e_measured (o 1 si NaN)
        if (applyAnalyticalPostVelocities)
        {
            float e_use = (computed) ? e_measured : 1f;
            float m1 = car1.rb.mass;
            float m2 = car2.rb.mass;
            float u1 = v1_before;
            float u2 = v2_before;

            float v1_calc = (m1 * u1 + m2 * u2 - m2 * e_use * (u1 - u2)) / (m1 + m2);
            float v2_calc = (m1 * u1 + m2 * u2 + m1 * e_use * (u1 - u2)) / (m1 + m2);

            // Aplicar como velocities (esto sobrescribe la respuesta física de Unity)
            car1.rb.linearVelocity = new Vector2(v1_calc, car1.rb.linearVelocity.y);
            car2.rb.linearVelocity = new Vector2(v2_calc, car2.rb.linearVelocity.y);

            v1_after = v1_calc;
            v2_after = v2_calc;
        }

        // Calcular energías y momento
        float m1f = car1.rb.mass;
        float m2f = car2.rb.mass;
        float Ek_before = 0.5f * m1f * v1_before * v1_before + 0.5f * m2f * v2_before * v2_before;
        float Ek_after  = 0.5f * m1f * v1_after * v1_after + 0.5f * m2f * v2_after * v2_after;
        float momentum_before = m1f * v1_before + m2f * v2_before;
        float momentum_after  = m1f * v1_after + m2f * v2_after;

        float percentEnergyConserved = (Ek_before > 0f) ? (Ek_after / Ek_before) * 100f : 0f;

        // Clasificación simple según e_measured (si no calculable, usar heurística con Ek)
        string collisionType = "Indeterminado";
        if (computed)
        {
            if (e_measured >= 0.9f) collisionType = "Elástico";
            else if (e_measured > 0.0f) collisionType = "Inelástico";
            else collisionType = "Perfectamente Inelástico";
        }
        else
        {
            // fallback por energía
            if (percentEnergyConserved > 90f) collisionType = "Elástico (por energía)";
            else if (percentEnergyConserved > 30f) collisionType = "Inelástico (por energía)";
            else collisionType = "Altamente inelástico (por energía)";
        }

        // Notificar al SimulationManager con datos numéricos
        if (simulationManager != null)
        {
            simulationManager.OnCollisionResults(
                v1_before, v2_before, v1_after, v2_after,
                e_measured,
                Ek_before, Ek_after,
                momentum_before, momentum_after,
                percentEnergyConserved,
                collisionType
            );
        }

        // Marcamos que la colisión terminó desde la perspectiva de este analizador
        collisionInProgress = false;
    }
}

*/