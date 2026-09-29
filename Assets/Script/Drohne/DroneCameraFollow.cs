using UnityEngine;

/// <summary>
/// Lässt die Kamera der Drohne von schräg hinten-oben folgen (Third-Person-
/// Chase-Kamera). Nutzt für die Positionierung nur die Yaw-Rotation der
/// Drohne, damit die Kamera nicht mit kippt, wenn sich die Drohne neigt.
///
/// Dieses Script gehört auf die "Main Camera".
/// </summary>
public class DroneCameraFollow : MonoBehaviour
{
    [Header("Ziel")]
    [Tooltip("Das Drohnen-Objekt, dem die Kamera folgen soll")]
    public Transform target;

    [Header("Position relativ zur Drohne")]
    [Tooltip("Abstand hinter der Drohne")]
    public float distance = 7f;
    [Tooltip("Höhe über der Drohne")]
    public float height = 4f;

    [Header("Glättung")]
    [Tooltip("Wie schnell die Kamera der Zielposition folgt (höher = strafferes Folgen)")]
    public float positionSmoothing = 5f;
    [Tooltip("Wie schnell die Kamera sich zur Blickrichtung dreht")]
    public float rotationSmoothing = 5f;

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (target == null) return;

        // Nur den Yaw-Winkel der Drohne verwenden (Pitch/Roll ignorieren),
        // damit die Kamera bei Neigungen der Drohne stabil bleibt
        float yaw = target.eulerAngles.y;
        Quaternion yawRotation = Quaternion.Euler(0f, yaw, 0f);

        // Zielposition: schräg hinter und über der Drohne
        Vector3 desiredOffset = yawRotation * new Vector3(0f, height, -distance);
        Vector3 desiredPosition = target.position + desiredOffset;

        // Sanft zur Zielposition bewegen
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            1f / positionSmoothing);

        // Kamera sanft auf die Drohne ausrichten (leicht über dem Zentrum anvisieren)
        Vector3 lookTarget = target.position + Vector3.up * 0.5f;
        Quaternion desiredRotation = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            Time.deltaTime * rotationSmoothing);
    }
}
