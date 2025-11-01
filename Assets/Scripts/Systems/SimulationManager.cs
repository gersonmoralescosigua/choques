using UnityEngine;
using TMPro;
using UnityEngine.UI; // para Button

public class SimulationManager : MonoBehaviour
{
    [Header("Referencias de Carros")]
    public CarController car1;
    public CarController car2;

    [Header("UI - TMP Inputs")]
    public TMP_InputField inputCar1Mass;
    public TMP_InputField inputCar1Velocity;
    public TMP_InputField inputCar2Mass;
    public TMP_InputField inputCar2Velocity;

    [Header("UI - Result Texts")]
    public TMP_Text statusText;
    public TMP_Text resultTypeText;
    public TMP_Text resultEText;
    public TMP_Text resultEnergyText;

    [Header("Botones")]
    public Button startButton;
    public Button resetButton;

    private bool simulationRunning = false;

    void Start()
    {
        UpdateStartButtonInteractable();
        if (statusText != null) statusText.text = "Listo. Complete los campos para iniciar.";
        // añade listeners para validar en vivo
        inputCar1Mass.onValueChanged.AddListener(_ => UpdateStartButtonInteractable());
        inputCar1Velocity.onValueChanged.AddListener(_ => UpdateStartButtonInteractable());
        inputCar2Mass.onValueChanged.AddListener(_ => UpdateStartButtonInteractable());
        inputCar2Velocity.onValueChanged.AddListener(_ => UpdateStartButtonInteractable());
    }

    // Validación: todos los campos deben ser números > 0
    private bool InputsValid()
    {
        float temp;
        if (!float.TryParse(inputCar1Mass.text, out temp) || temp <= 0) return false;
        if (!float.TryParse(inputCar1Velocity.text, out temp) || temp < 0) return false; // permitimos 0 velocidad
        if (!float.TryParse(inputCar2Mass.text, out temp) || temp <= 0) return false;
        if (!float.TryParse(inputCar2Velocity.text, out temp) || temp < 0) return false;
        return true;
    }

    private void UpdateStartButtonInteractable()
    {
        if (startButton == null) return;
        // start button interactable si inputs válidos y no hay simulación corriendo
        startButton.interactable = InputsValid() && !simulationRunning;
    }

    /*public void StartSimulation()
    {
        if (simulationRunning) return;
        if (!InputsValid())
        {
            if (statusText != null) statusText.text = "Complete correctamente todos los campos.";
            return;
        }

        // Parsear y aplicar
        float m1 = float.Parse(inputCar1Mass.text);
        float v1 = float.Parse(inputCar1Velocity.text);
        float m2 = float.Parse(inputCar2Mass.text);
        float v2 = float.Parse(inputCar2Velocity.text);

        car1.ApplyMass(m1);
        car2.ApplyMass(m2);
        car1.initialVelocityMagnitude = v1;
        car2.initialVelocityMagnitude = v2;

        // Aplicar velocidades
        car1.ApplyInitialVelocity();
        car2.ApplyInitialVelocity();

        simulationRunning = true;
        UpdateStartButtonInteractable();

        if (statusText != null) statusText.text = "Simulación iniciada...";
    }
    */

    public void StartSimulation()
    {
        if (simulationRunning) return;
        if (!InputsValid())
        {
            if (statusText != null) statusText.text = "Complete correctamente todos los campos.";
            return;
        }

        // Parsear y aplicar
        float m1 = float.Parse(inputCar1Mass.text);
        float v1 = float.Parse(inputCar1Velocity.text);
        float m2 = float.Parse(inputCar2Mass.text);
        float v2 = float.Parse(inputCar2Velocity.text);

        car1.ApplyMass(m1);
        car2.ApplyMass(m2);
        car1.initialVelocityMagnitude = v1;
        car2.initialVelocityMagnitude = v2;

        // IMPORTANTE: Iniciar el analizador de colisiones
        CollisionAnalyzer analyzer = FindObjectOfType<CollisionAnalyzer>();
        if (analyzer != null)
            analyzer.StartSimulation();

        // Aplicar velocidades
        car1.ApplyInitialVelocity();
        car2.ApplyInitialVelocity();

        simulationRunning = true;
        UpdateStartButtonInteractable();

        if (statusText != null) statusText.text = "Simulación iniciada... Esperando colisión";
    }

    

    /*public void ResetSimulation()
    {
        car1.ResetToInitial();
        car2.ResetToInitial();
        simulationRunning = false;
        UpdateStartButtonInteractable();
        if (statusText != null) statusText.text = "Simulación reiniciada. Completa campos para iniciar.";
        // limpiar resultados
        if (resultTypeText != null) resultTypeText.text = "-";
        if (resultEText != null) resultEText.text = "-";
        if (resultEnergyText != null) resultEnergyText.text = "-";
    }*/
    
    public void ResetSimulation()
{
    car1.ResetToInitial();
    car2.ResetToInitial();
    simulationRunning = false;
    
    // Resetear el analyzer también
    CollisionAnalyzer analyzer = FindObjectOfType<CollisionAnalyzer>();
    if (analyzer != null)
        analyzer.StartSimulation();
    
    UpdateStartButtonInteractable();
    if (statusText != null) statusText.text = "Simulación reiniciada.";
    
    // limpiar resultados
    if (resultTypeText != null) resultTypeText.text = "-";
    if (resultEText != null) resultEText.text = "-";
    if (resultEnergyText != null) resultEnergyText.text = "-";
}

    // Called by CollisionAnalyzer when collision data is ready
    public void OnCollisionResults(
        float v1_before, float v2_before, float v1_after, float v2_after,
        float e_measured,
        float Ek_before, float Ek_after,
        float momentum_before, float momentum_after,
        float percentEnergyConserved,
        string collisionType)
    {
        simulationRunning = false; // la simulación considerada "terminada" para efectos de UI
        UpdateStartButtonInteractable();

        if (statusText != null) statusText.text = "Simulación finalizada.";
        if (resultTypeText != null) resultTypeText.text = $"{collisionType}";
        if (resultEText != null) resultEText.text = $"e = {e_measured:F3}";
        if (resultEnergyText != null) resultEnergyText.text = $"E_before={Ek_before:F2}, E_after={Ek_after:F2} ({percentEnergyConserved:F1}%)";

        Debug.Log($"Collision results: type={collisionType}, e={e_measured}, Ek%={percentEnergyConserved:F1}");
    }
}
