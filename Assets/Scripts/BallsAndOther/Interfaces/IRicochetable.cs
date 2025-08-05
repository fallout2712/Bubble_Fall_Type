using UnityEngine;

public interface IRicochetable
{
    void Ricochet(Vector2 contactPoint, Vector2 lastDirection);
}
