using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARUXReasonsManager : MonoBehaviour
{
    public bool showNotTrackingReasons = true;

    [SerializeField] GameObject _reasonParent;
    [SerializeField] TMP_Text _reasonText;
    [SerializeField] Image _reasonIcon;
    [SerializeField] Sprite _initRelocalSprite;
    [SerializeField] Sprite _motionSprite;
    [SerializeField] Sprite _lightSprite;
    [SerializeField] Sprite _featuresSprite;
    [SerializeField] Sprite _unsupportedSprite;
    [SerializeField] Sprite _noneSprite;

    NotTrackingReason _currentReason;
    bool _sessionTracking;

    const string k_InitRelocalText = "Initializing augmented reality.";
    const string k_MotionText = "Try moving at a slower pace.";
    const string k_LightText = "It’s too dark. Try going to a more well lit area.";
    const string k_FeaturesText = "Look for more textures or details in the area.";
    const string k_UnsupportedText = "AR content is not supported.";
    const string k_NoneText = "Wait for tracking to begin.";
    
    void OnEnable()
    {
        ARSession.stateChanged += ARSessionOnstateChanged;

        if (!showNotTrackingReasons)
        {
            _reasonParent.SetActive(false);
        }
    }

    void OnDisable()
    {
        ARSession.stateChanged -= ARSessionOnstateChanged;
    }

    void Update()
    {
        if (showNotTrackingReasons)
        {
            if (!_sessionTracking)
            {
                _currentReason = ARSession.notTrackingReason;
                ShowReason();
            }
            else
            {
                if (_reasonText.gameObject.activeSelf)
                {
                    _reasonParent.SetActive(false);
                }
            }
        }
    }

    void ARSessionOnstateChanged(ARSessionStateChangedEventArgs args)
    {
        _sessionTracking = args.state == ARSessionState.SessionTracking ? true : false;
        Debug.Log(args.state);
    }

    void ShowReason()
    {
        _reasonParent.SetActive(true);
        SetReason();
    }

    void SetReason()
    {
        switch (_currentReason)
        {
            case NotTrackingReason.Initializing:
            case NotTrackingReason.Relocalizing:
                _reasonText.text = k_InitRelocalText;
                _reasonIcon.sprite = _initRelocalSprite;
                break;
            case NotTrackingReason.ExcessiveMotion:
                _reasonText.text = k_MotionText;
                _reasonIcon.sprite = _motionSprite;
                break;
            case NotTrackingReason.InsufficientLight:
                _reasonText.text = k_LightText;
                _reasonIcon.sprite = _lightSprite;
                break;
            case NotTrackingReason.InsufficientFeatures:
                _reasonText.text = k_FeaturesText;
                _reasonIcon.sprite = _featuresSprite;
                break;
            case NotTrackingReason.Unsupported:
                _reasonText.text = k_UnsupportedText;
                _reasonIcon.sprite = _unsupportedSprite;
                break;
            case NotTrackingReason.None:
                _reasonText.text = k_NoneText;
                _reasonIcon.sprite = _noneSprite;
                break;
        }
    }

    public void TestForceShowReason(NotTrackingReason reason)
    {
        _currentReason = reason;
        ShowReason();
    }
    
}
