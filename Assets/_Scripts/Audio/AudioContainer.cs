using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioContainer : MonoBehaviour
{
    public Dictionary<EventReference, EventInstance> eventInstances = new();

    public void PlaySound(EventReference sound)
    {
        if (sound.IsNull)
        {
            Debug.LogWarning($"Audio event {sound} not assigned");
            return;
        }
        RuntimeManager.PlayOneShot(sound, transform.position);
    }

    public void PlaySoundWithParam(EventReference sound, string paramName, float paramValue)
    {
        if (sound.IsNull)
        {
            Debug.LogWarning($"Audio event {sound} not assigned");
            return;
        }
        FMODExtensions.PlayOneShot(sound, paramName, paramValue, transform.position);
    }

    /// <summary>
    /// Syntax use case: <br />
    /// container.PlaySoundWithParams(container.sound, new(string,float)[] { ("param1", 0.5f), ("param2", 1f) });
    /// </summary>
    public void PlaySoundWithParams(EventReference sound, params (string, float)[] parameters)
    {
        if (sound.IsNull)
        {
            Debug.LogWarning($"Audio event {sound} not assigned");
            return;
        }
        FMODExtensions.PlayOneShotWithParameters(sound, transform.position, parameters);
    }

    public EventInstance CreateInstance(EventReference reference, bool playInstance = false)
    {
        EventInstance instance = RuntimeManager.CreateInstance(reference);
        RuntimeManager.AttachInstanceToGameObject(instance, gameObject);
        eventInstances.Add(reference, instance);
        if (playInstance)
            instance.start();
        return instance;
    }

    private void OnDestroy()
    {
        foreach (var pair in eventInstances)
        {
            pair.Value.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            pair.Value.release();
        }
    }
}

public static class FMODExtensions
{
    public static void PlayOneShot(FMOD.GUID guid, string parameterName, float parameterValue, Vector3 position = new Vector3())
    {
        var instance = RuntimeManager.CreateInstance(guid);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        instance.setParameterByName(parameterName, parameterValue);
        instance.start();
        instance.release();
    }

    public static void PlayOneShot(EventReference eventReference, string parameterName, float parameterValue, Vector3 position = new Vector3())
    {
        try
        {
            PlayOneShot(eventReference.Guid, parameterName, parameterValue, position);
        }
        catch (EventNotFoundException)
        {
            RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + eventReference);
        }
    }

    public static void PlayOneShotWithParameters(FMOD.GUID guid, Vector3 position = new Vector3(), params (string name, float value)[] parameters)
    {
        var instance = RuntimeManager.CreateInstance(guid);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        foreach (var (name, value) in parameters)
            instance.setParameterByName(name, value);
        instance.start();
        instance.release();
    }

    public static void PlayOneShotWithParameters(EventReference eventReference, Vector3 position = new Vector3(), params (string name, float value)[] parameters)
    {
        try
        {
            PlayOneShotWithParameters(eventReference.Guid, position, parameters);
        }
        catch (EventNotFoundException)
        {
            RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + eventReference);
        }
    }
}