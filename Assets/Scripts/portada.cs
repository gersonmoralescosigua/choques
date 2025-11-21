using UnityEngine;

public class portada : MonoBehaviour
{
    public void SalirDelJuego()
    {
        Application.Quit();
        Debug.Log("El juego se cerraría aquí (solo funciona en build).");
    }
}

