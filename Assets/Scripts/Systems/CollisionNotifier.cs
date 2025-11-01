// CollisionNotifier.cs
using UnityEngine;

public class CollisionNotifier : MonoBehaviour
{
    public CollisionAnalyzer analyzer; // arrastra el CollisionAnalyzer desde Inspector

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // opcional: comprobar que la colisión sea con el otro carro (por tag o nombre)
        if (collision.collider != null)
        {
            analyzer?.OnCarsCollisionEnter();
        }
    }
}
