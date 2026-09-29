using UnityEngine;

/// <summary>
/// Steuert die Drohne: Vorwärts/Rückwärts, Seitwärts (Strafe), Hoch/Runter,
/// Drehung (Yaw) sowie eine visuelle Neigung (Pitch/Roll) in Bewegungsrichtung.
///
/// Dieses Script gehört auf das Objekt "Drohne" (das Elternobjekt der Cubes
/// und Cylinder in deiner Hierarchie). Es benötigt ein Rigidbody-Component.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    [Header("Bewegung")]
    [Tooltip("Geschwindigkeit für Vorwärts/Rückwärts/Seitwärts")]
    public float moveSpeed = 20f;
    [Tooltip("Geschwindigkeit für Hoch- und Runterfliegen")]
    public float verticalSpeed = 5f;
    [Tooltip("Drehgeschwindigkeit (Yaw) in Grad pro Sekunde")]
    public float yawSpeed = 90f;

    [Header("Neigung (Tilt)")]
    [Tooltip("Maximaler Neigungswinkel in Grad, wenn voll beschleunigt wird")]
    public float maxTiltAngle = 20f;
    [Tooltip("Wie schnell die Neigung dem Zielwinkel folgt (höher = schneller)")]
    public float tiltSmoothing = 5f;

    private Rigidbody rb;

    private float currentYaw;
    private float currentPitch;
    private float currentRoll;

    private float inputForward;
    private float inputStrafe;
    private float inputVertical;
    private float inputYaw;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;      // Die Drohne schwebt selbständig
        rb.linearDamping = 2f;      // sorgt für sanftes Abbremsen (Unity 6 Benennung)
        rb.angularDamping = 5f;

        currentYaw = transform.eulerAngles.y;
    }

    void Update()
    {
        ReadInput();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ReadInput()
    {
        // W/S bzw. Pfeiltasten hoch/runter = vor/zurück
        inputForward = Input.GetAxis("Vertical");
        // A/D bzw. Pfeiltasten links/rechts = seitwärts (strafe)
        inputStrafe = Input.GetAxis("Horizontal");

        // Hoch- / Runterfliegen
        inputVertical = 0f;
        if (Input.GetKey(KeyCode.Space)) inputVertical = 1f;
        if (Input.GetKey(KeyCode.LeftControl)) inputVertical = -1f;

        // Drehung (Yaw) mit Q/E
        inputYaw = 0f;
        if (Input.GetKey(KeyCode.Q)) inputYaw = -1f;
        if (Input.GetKey(KeyCode.E)) inputYaw = 1f;
    }

    private void ApplyMovement()
    {
        // 1) Yaw (Drehung um die Hochachse) aktualisieren
        currentYaw += inputYaw * yawSpeed * Time.fixedDeltaTime;

        // 2) Zielneigung berechnen: Vorwärtsbewegung -> Pitch, Seitwärts -> Roll
        float targetPitch = inputForward * maxTiltAngle;
        float targetRoll = -inputStrafe * maxTiltAngle;

        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.fixedDeltaTime * tiltSmoothing);
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.fixedDeltaTime * tiltSmoothing);

        // 3) Rotation zusammensetzen: erst Yaw, dann Neigung überlagern
        Quaternion yawRotation = Quaternion.Euler(0f, currentYaw, 0f);
        Quaternion tiltRotation = Quaternion.Euler(currentPitch, 0f, currentRoll);
        rb.MoveRotation(yawRotation * tiltRotation);

        // 4) Bewegungsrichtung relativ zur Yaw-Ausrichtung (NICHT zur Neigung),
        //    damit die Drohne beim Neigen nicht "abdriftet"
        Vector3 moveDirection = yawRotation * new Vector3(inputStrafe, 0f, inputForward);
        Vector3 velocity = moveDirection * moveSpeed + Vector3.up * inputVertical * verticalSpeed;

        rb.linearVelocity = velocity;
    }
}
