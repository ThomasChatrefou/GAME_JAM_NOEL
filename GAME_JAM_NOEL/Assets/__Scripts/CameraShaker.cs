using System.Collections;
using UnityEngine;
using Cinemachine;
using Unity.Mathematics;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance;

    private CinemachineVirtualCamera virtualCamera;

    private float timer = 0f;
    private float shakeDuration;
    private float startIntensity;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;

        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCamera(float intensity, float time, float frequency)
    {
        CinemachineBasicMultiChannelPerlin noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_FrequencyGain = frequency;
        noise.m_AmplitudeGain = intensity;
        startIntensity = intensity;

        shakeDuration = time;
        timer = time;
        StartCoroutine(LinearShakeCoroutine());
    }

    private IEnumerator LinearShakeCoroutine()
    {
        CinemachineBasicMultiChannelPerlin noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            noise.m_AmplitudeGain = Mathf.Lerp(startIntensity, 0f, timer / shakeDuration);
            yield return null;
        }
        noise.m_AmplitudeGain = 0f;
    }
}
