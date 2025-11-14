using UnityEngine;

public class ColisionChoque : MonoBehaviour
{
    // CRÍTICO: Debes arrastrar el Audio Source (el componente) a este campo en el Inspector.
    public AudioSource AudioChoque; 
    
    // Omitimos la velocidad mínima por ahora para garantizar que el Log se dispare.

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Mostrar el mensaje. Si esto se imprime, la física FUNCIONA.
        Debug.Log("¡CONTACTO DETECTADO con " + collision.gameObject.name + "!");

        // 2. Verificar que el AudioSource esté asignado y tenga un clip.
        if (AudioChoque != null && AudioChoque.clip != null)
        {
            // 3. Reproducir el sonido una sola vez
            AudioChoque.PlayOneShot(AudioChoque.clip, 1.0f); 
        }
    }
}