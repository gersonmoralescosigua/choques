using UnityEngine;

public class ColisionChoque : MonoBehaviour
{
    // Asigna el Audio Source aquí
    public AudioSource AudioChoque; 
    
    // Esta variable nos dice si el Log apareció, no es necesaria para la colisión, pero ayuda a debuggear
    private bool collisionDetected = false; 

    // Al chocar, intenta disparar el sonido
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collisionDetected)
        {
            // Omitimos la verificación de velocidad por ahora para asegurar el disparo del Log
            
            if (AudioChoque != null && AudioChoque.clip != null)
            {
                AudioChoque.PlayOneShot(AudioChoque.clip, 1.0f);
            }

            Debug.Log("¡CHOQUE FORZADO DETECTADO! (Probando Colisión)");
            collisionDetected = true; // Marca como detectado para evitar spam
        }
    }
    
    // Función de Unity que se llama al inicio.
    void Start()
    {
        // Esto restablece la variable después de 0.5 segundos por si el sistema necesita un momento
        Invoke("ResetCollisionState", 0.5f); 
    }

    void ResetCollisionState()
    {
        // Esto permite que el OnCollisionEnter2D se dispare aunque los colliders empiecen tocándose.
        collisionDetected = false;
    }
}