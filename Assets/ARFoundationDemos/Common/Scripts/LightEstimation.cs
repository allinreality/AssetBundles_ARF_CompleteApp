using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

// Process the most recently received light estimation information about the physical environment by the AR camera.
[RequireComponent(typeof(Light))]
public class LightEstimation : MonoBehaviour
{
    // Produces frame events containing light estimation information.
    [SerializeField]
    ARCameraManager _cameraManager;

    public ARCameraManager CameraManager
    {
        get { return _cameraManager; }
        set
        {
            if (_cameraManager == value)
                return;

            if (_cameraManager != null)
                _cameraManager.frameReceived -= FrameChanged;

            _cameraManager = value;

            if (_cameraManager != null & enabled)
                _cameraManager.frameReceived += FrameChanged;
        }
    }

    [SerializeField]
    float _brightnessMod = 2.0f;

    Light _light;

    /// The estimated brightness of the physical environment, if available.
    public float? Brightness { get; private set; }

    /// The estimated color temperature of the physical environment, if available.
    public float? ColorTemperature { get; private set; }

    /// The estimated color correction value of the physical environment, if available.
    public Color? ColorCorrection { get; private set; }

    /// The estimated direction of the main light of the physical environment, if available.
    public Vector3? MainLightDirection { get; private set; }

    /// The estimated color of the main light of the physical environment, if available.
    public Color? MainLightColor { get; private set; }

    /// The estimated intensity in lumens of main light of the physical environment, if available.
    public float? MainLightIntensityLumens { get; private set; }

    /// The estimated spherical harmonics coefficients of the physical environment, if available.
    public SphericalHarmonicsL2? SphericalHarmonics { get; private set; }


    void Awake ()
    {
        _light = GetComponent<Light>();
    }

    void OnEnable()
    {
        if (_cameraManager != null)
            _cameraManager.frameReceived += FrameChanged;
    }

    void OnDisable()
    {
        if (_cameraManager != null)
            _cameraManager.frameReceived -= FrameChanged;
    }

    void FrameChanged(ARCameraFrameEventArgs args)
    {
        if (args.lightEstimation.averageBrightness.HasValue)
        {
            Brightness = args.lightEstimation.averageBrightness.Value;
            _light.intensity = Brightness.Value * _brightnessMod;
        }

        if (args.lightEstimation.averageColorTemperature.HasValue)
        {
            ColorTemperature = args.lightEstimation.averageColorTemperature.Value;
            _light.colorTemperature = ColorTemperature.Value;
        }
        
        if (args.lightEstimation.colorCorrection.HasValue)
        {
            ColorCorrection = args.lightEstimation.colorCorrection.Value;
            _light.color = ColorCorrection.Value;
        }

        if (args.lightEstimation.mainLightDirection.HasValue)
        {
            MainLightDirection = args.lightEstimation.mainLightDirection;
            _light.transform.rotation = Quaternion.LookRotation(MainLightDirection.Value);
        }

        if (args.lightEstimation.mainLightColor.HasValue)
        {
            MainLightColor = args.lightEstimation.mainLightColor;
            _light.color = MainLightColor.Value;
        }

        if (args.lightEstimation.mainLightIntensityLumens.HasValue)
        {
            MainLightIntensityLumens = args.lightEstimation.mainLightIntensityLumens;
            _light.intensity = args.lightEstimation.averageMainLightBrightness.Value;
        }

        if (args.lightEstimation.ambientSphericalHarmonics.HasValue)
        {
            SphericalHarmonics = args.lightEstimation.ambientSphericalHarmonics;
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientProbe = SphericalHarmonics.Value;
        }
    }

}
