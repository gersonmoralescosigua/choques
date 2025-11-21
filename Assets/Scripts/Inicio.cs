using UnityEngine;
using UnityEngine.SceneManagement;

public class inicio : MonoBehaviour
{
    public string escenaDestino = "inicio"; // Cambia este nombre por tu escena
    public float tiempoEspera = 3f;               // Segundos que dura la portada

    void Start()
    {
        Invoke("CambiarEscena", tiempoEspera);
    }

    void CambiarEscena()
    {
        SceneManager.LoadScene(escenaDestino);
    }
}

