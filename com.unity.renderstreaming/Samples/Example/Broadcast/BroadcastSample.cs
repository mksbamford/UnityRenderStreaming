using UnityEngine;

namespace Unity.RenderStreaming.Samples
{
    class BroadcastSample : MonoBehaviour
    {
        [SerializeField] RenderStreaming renderStreaming;

        private void Awake()
        {
#if URS_USE_AR_FOUNDATION
            InputSystem.RegisterLayout<UnityEngine.XR.ARSubsystems.HandheldARInputDevice>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                );
#endif
        }

        // Start is called before the first frame update
        void Start()
        {
            if (renderStreaming.runOnAwake)
                return;
            renderStreaming.Run(
                hardwareEncoder: RenderStreamingSettings.EnableHWCodec,
                signaling: RenderStreamingSettings.Signaling);
        }
    }
}
