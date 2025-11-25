// IKnockbackable.cs
using UnityEngine; // <--- ¡Esto es lo que faltaba!

public interface IKnockbackable
{
    void Knockback(Vector2 force, float duration);
}