using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 100, 0); // Velocidad de rotación en grados por segundo
    public float speedMultiplier = 1f; // Factor de velocidad ajustable

    void Update()
    {
        transform.Rotate(rotationSpeed * speedMultiplier * Time.deltaTime, Space.Self);
    }
}