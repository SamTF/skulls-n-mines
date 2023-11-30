using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Screen Shake Stuff
    private float shakeDuration = 0f;       /// Desired duration of the shake effect
    private float shakeMagnitude = 0.1f;    /// A measure of magnitude for the shake
    private float dampingSpeed = 1.0f;      /// A measure of how quickly the shake effect should evaporate
    private bool screenshake = false;       /// If the Camera should trigger a Screen Shake
    private Vector3 startPos;               /// The initial position of the Camera

    private enum ZoomType { In = -1, Out = 1}


    // Singleton thing
    private static CameraController _instance;
    public static CameraController instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        // Singleton Thing
        if (instance == null)   _instance = this;
        else                    Destroy(gameObject);
    }

    private void Start()
    {
        startPos = transform.position;
        GameManager.instance.onVictory += OnVictory;
    }

    private void Update()
    {
        // Screen Shake Stuff
        if (screenshake)
        {
            if (shakeDuration > 0)
            {
                transform.localPosition = startPos + Random.insideUnitSphere * shakeMagnitude;
                shakeDuration -= Time.deltaTime * dampingSpeed;
            }
            else
            {
                screenshake = false;
                shakeDuration = 0f;
                transform.localPosition = startPos;
            }
        }
    }


    // Triggers the Screen Shaking
    public void TriggerShake(float magnitude = 0.1f, float duration = 0.1f)
    {
        screenshake = true;
        shakeDuration += duration;
        shakeMagnitude = magnitude;
    }

    private void OnVictory()
    {
        StartCoroutine(Zoom(ZoomType.In));
    }

    private void OnStart()
    {
        StartCoroutine(Zoom(ZoomType.Out));
    }

    private IEnumerator Zoom(ZoomType z)
    {
        Camera camera = GetComponent<Camera>();

        while(true)
        {
            float value = (int)z / 10f;
            camera.orthographicSize += value;
            transform.Rotate(new Vector3(0, 0, 1));

            yield return new WaitForSeconds(0.05f);
        }
    }
}
