using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleController : MonoBehaviour
{
    private Dictionary<int, List<float>> currentIntensities = new Dictionary<int, List<float>>();

    public float IntensityFactor = 1.0f;

    void Destroy()
    {
        Reset();
    }

    void OnApplicationQuit()
    {
        Reset();
    }

    public void AddRumbleForAll(float intensity, float duration)
    {
        foreach (var gamepad in Gamepad.all)
        {
            AddRumble(gamepad, intensity, duration);
        }
    }

    public void AddRumble(Gamepad gamepad, float intensity, float duration)
    {
        if (gamepad == null) return;
        StartCoroutine(_AddRumble(gamepad.id, intensity, duration));
    }

    private IEnumerator _AddRumble(int gamepadId, float intensity, float duration)
    {
        if (!currentIntensities.TryGetValue(gamepadId, out var gamepadIntensities))
        {
            gamepadIntensities = new List<float>();
            currentIntensities.Add(gamepadId, gamepadIntensities);
        }
        gamepadIntensities.Add(intensity);
        UpdateMotor(gamepadId);
        yield return new WaitForSeconds(duration);
        gamepadIntensities.Remove(intensity);
        UpdateMotor(gamepadId);

    }

    private void UpdateMotor(int gamepadId)
    {
        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.id != gamepadId) continue;
            float maxIntensity = 0;
            foreach (var intensity in currentIntensities[gamepadId])
            {
                maxIntensity = Mathf.Max(maxIntensity, intensity);
            }
            gamepad.SetMotorSpeeds(maxIntensity * IntensityFactor, maxIntensity * IntensityFactor);

        }
    }

    private void Reset()
    {
        CancelInvoke();
        foreach (var gamepad in Gamepad.all)
        {
            if (!currentIntensities.ContainsKey(gamepad.id)) continue;
            gamepad.SetMotorSpeeds(0, 0);
        }
        currentIntensities.Clear();
    }
}