using UnityEngine;

public interface Buildable
{
    void Place(Vector3 position);
    void Move(Vector3 newPosition);
    void Destroy();
}
