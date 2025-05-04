using UnityEngine;

public class Falcon : MonoBehaviour
{
    [Header("Model Tilting")]
    [SerializeField] private Transform modelTransform;
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private float minTilt = 10f;
    [SerializeField] private float maxTilt = 45f;

    [Header("Engine Glow")]
    [SerializeField] private Renderer leftGlow;
    [SerializeField] private Renderer rightGlow;
    [SerializeField] private Transform leftGlowTransform;
    [SerializeField] private Transform rightGlowTransform;
    [SerializeField] private Color idleColor = Color.black;
    [SerializeField] private Color boostColor = Color.cyan;
    [SerializeField] private Vector3 minScale = Vector3.one * 0.3f;
    [SerializeField] private Vector3 maxScale = Vector3.one * 1.6f;
    [SerializeField] private float glowLerpSpeed = 5f;
    [SerializeField] private float scaleSpeed = 5f;

    [Header("Engine Particles")]
    [SerializeField] private ParticleSystem leftEngineParticles;
    [SerializeField] private ParticleSystem rightEngineParticles;

    [Header("Ship Speed")]
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 30f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float currentSpeed = 0f;

    private bool accelerating;
    private float currentZRotation = 0f;
    private float targetZRotation = 0f;
    private float zVelocity = 0f;
    private Quaternion initialModelRotation;

    void Start()
    {
        if (modelTransform != null)
            initialModelRotation = modelTransform.localRotation;
    }

    void Update()
    {
        // --- ACCELERATION LOGIC ---
        accelerating = Input.GetKey(KeyCode.W);

        if (accelerating)
            currentSpeed += acceleration * Time.deltaTime;
        else
            currentSpeed -= deceleration * Time.deltaTime;

        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
        float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);

        // --- TILT LOGIC ---
        float input = 0f;
        if (Input.GetKey(KeyCode.D)) input = -1f;
        else if (Input.GetKey(KeyCode.A)) input = 1f;

        float dynamicTiltAngle = Mathf.Lerp(minTilt, maxTilt, speedRatio);
        targetZRotation = input * dynamicTiltAngle;
        currentZRotation = Mathf.SmoothDampAngle(currentZRotation, targetZRotation, ref zVelocity, smoothTime);
        modelTransform.localRotation = initialModelRotation * Quaternion.Euler(0f, 0f, currentZRotation);

        // --- ENGINE GLOW COLOR ---
        Color targetColor = accelerating ? boostColor : idleColor;
        leftGlow.material.color = Color.Lerp(leftGlow.material.color, targetColor, Time.deltaTime * glowLerpSpeed);
        rightGlow.material.color = Color.Lerp(rightGlow.material.color, targetColor, Time.deltaTime * glowLerpSpeed);

        // --- ENGINE GLOW SCALE ---
        Vector3 targetScale = accelerating ? maxScale : minScale;
        leftGlowTransform.localScale = Vector3.Lerp(leftGlowTransform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        rightGlowTransform.localScale = Vector3.Lerp(rightGlowTransform.localScale, targetScale, Time.deltaTime * scaleSpeed);

        // --- PARTICLES ---
        var leftEmission = leftEngineParticles.emission;
        leftEmission.rateOverTime = accelerating ? 100f : 0f;

        var rightEmission = rightEngineParticles.emission;
        rightEmission.rateOverTime = accelerating ? 100f : 0f;
    }
}
