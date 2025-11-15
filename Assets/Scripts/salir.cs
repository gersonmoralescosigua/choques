using UnityEngine;
using UnityEngine.SceneManagement;

public class salir : MonoBehaviour
{
    public void RegresarAInicio()
    {
        SceneManager.LoadScene("Inicio");
    }
}
