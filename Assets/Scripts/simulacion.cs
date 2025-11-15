using UnityEngine;
using UnityEngine.SceneManagement;

public class simulacion : MonoBehaviour
{
    public void IrASimulacion()
    {
        SceneManager.LoadScene("camionvspickup");
    }
}

