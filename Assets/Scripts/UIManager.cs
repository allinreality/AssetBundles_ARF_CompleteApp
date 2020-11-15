using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] ARUXAnimationManager animationManager;

    bool prepared;
    bool goalReached;

    void OnEnable()
    {
        // Hide tap UI when OnObjectPlaced event is fired.
        PlaceObjectsOnPlane.OnObjectPlaced += () => animationManager.FadeOffUI();
        ARSession.stateChanged += ShowFindPlaneUI;
    }

    void ShowFindPlaneUI(ARSessionStateChangedEventArgs args)
    {
        if (args.state == ARSessionState.SessionTracking && !PlanesFound())
        {
            // Show scan UI
            animationManager.ShowCrossPlatformFindAPlane();
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }

    void Update()
    {
        // Proceed only when the session is in tracking state
        if (ARSession.state != ARSessionState.SessionTracking)
        {
            return;
        }

        if (PlanesFound() && !prepared)
        {
            // Hide scan UI
            animationManager.FadeOffUI();
            prepared = true;
        }

        if (prepared && !goalReached && animationManager.fadeOffComplete)
        {
            // Show tap UI
            animationManager.ShowTapToPlace();
            goalReached = true;
        }
    }

    bool PlanesFound()
    {
        return planeManager?.trackables.count > 0;
    }
}

