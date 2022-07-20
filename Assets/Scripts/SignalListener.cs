using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SignalListener : MonoBehaviour
{
    public GameSignal gamesignal;
    public UnityEvent signalEvent;

    public void OnSignalRaised()
    {
        signalEvent.Invoke();
    }

    private void OnEnable()
    {
        gamesignal.RegisterListener(this);
    }

    private void OnDisable()
    {
        gamesignal.DeRegisterListener(this);
    }
}
