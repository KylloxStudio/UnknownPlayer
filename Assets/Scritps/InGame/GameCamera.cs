using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public float smoothSpeed;
    public float shakePower, shakeTime;
    public bool isCanCameraShake;

    public Camera mainCamera;
    public Transform target;
    public Vector2 offset;
    public Vector3 initialPosition;

    private void Start()
    {
        smoothSpeed = 3f;
        shakePower = 0f;
        shakeTime = 0f;
        isCanCameraShake = false;

        mainCamera = GetComponent<Camera>();
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (isCanCameraShake)
        {
            if (shakeTime > 0f)
            {
                transform.position = Random.insideUnitSphere * shakePower + initialPosition;
                shakeTime -= Time.deltaTime;
            }
            else
            {
                shakeTime = 0f;
                transform.position = initialPosition;
                isCanCameraShake = false;
            }
        }
    }

    private void LateUpdate()
    {
    }

    public void VibrateForTime(float power, float time)
    {
        isCanCameraShake = true;
        shakePower = power;
        shakeTime = time;
    }
}