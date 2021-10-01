using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.RenderStreaming.Samples
{
    public class MultiplaySample : MonoBehaviour
    {
        [SerializeField] ToggleGroup toggleGroupRole;
        [SerializeField] InputField inputFieldUsername;
        [SerializeField] InputField inputFieldUserID;
        [SerializeField] Button buttonStart;
        [SerializeField] RenderStreaming renderStreaming;
        [SerializeField] GameObject prefabHost;
        [SerializeField] GameObject prefabGuest;
        [SerializeField] GameObject prefabPlayer;
        [SerializeField] GameObject menuCamera;
        [SerializeField] GameObject panel;
        [SerializeField] RawImage videoImage;

        enum Role
        {
            Host = 0,
            Guest = 1
        }

        // Start is called before the first frame update
        void Start()
        {
            buttonStart.onClick.AddListener(OnClickButtonStart);
            inputFieldUsername.text = UnityEngine.Random.Range(0, 99999).ToString("00000");
            inputFieldUserID.text = Guid.NewGuid().ToString();
            inputFieldUsername.onValueChanged.AddListener(OnValueChangedUserName);
            inputFieldUserID.onValueChanged.AddListener(OnValueChangedHostID);
        }

        void OnValueChangedUserName(string value)
        {
            bool hasNullValue =
                string.IsNullOrEmpty(inputFieldUsername.text) ||
                string.IsNullOrEmpty(inputFieldUserID.text);
            buttonStart.interactable = !hasNullValue;
        }

        void OnValueChangedHostID(string value)
        {
            bool hasNullValue =
                string.IsNullOrEmpty(inputFieldUsername.text) ||
                string.IsNullOrEmpty(inputFieldUserID.text);
            buttonStart.interactable = !hasNullValue;
        }

        void OnClickButtonStart()
        {
            var toggles = toggleGroupRole.GetComponentsInChildren<Toggle>();
            var toggle = toggleGroupRole.GetFirstActiveToggle();
            var indexRole = Array.FindIndex(toggles, _ => _ == toggle);
            Role role = (Role)indexRole;

            var username = inputFieldUsername.text;
            var connectionId = inputFieldUserID.text;

            panel.SetActive(false);

            switch (role)
            {
                case Role.Host:
                    SetUpHost();
                    break;
                case Role.Guest:
                    StartCoroutine(SetUpGuest(username, connectionId));
                    break;
            }
        }

        void SetUpHost()
        {
            menuCamera.SetActive(false);

            var instance = GameObject.Instantiate(prefabHost);
            var handler = instance.GetComponent<Multiplay>();

            // host player
            GameObject.Instantiate(prefabPlayer);

            renderStreaming.Run(
                hardwareEncoder: RenderStreamingSettings.EnableHWCodec,
                signaling: RenderStreamingSettings.Signaling,
                handlers: new SignalingHandlerBase[] { handler }
                );
        }

        IEnumerator SetUpGuest(string username, string connectionId)
        {
            var instance = GameObject.Instantiate(prefabGuest);
            var handler = instance.GetComponent<SingleConnection>();

            renderStreaming.Run(
                hardwareEncoder: RenderStreamingSettings.EnableHWCodec,
                signaling: RenderStreamingSettings.Signaling,
                handlers: new SignalingHandlerBase[] { handler }
                );

            videoImage.gameObject.SetActive(true);
            var receiveVideoViewer = handler.GetComponent<ReceiveVideoViewer>();
            receiveVideoViewer.OnUpdateReceiveTexture += texture => videoImage.texture = texture;

            // todo(kazuki):
            yield return new WaitForSeconds(1f);

            handler.CreateConnection(connectionId);
            yield return new WaitUntil(() => handler.IsConnected(connectionId));
        }
    }
}

