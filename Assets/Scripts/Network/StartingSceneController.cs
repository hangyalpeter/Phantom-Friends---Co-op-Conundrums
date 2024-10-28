using System;
using System.Net;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartingSceneController : NetworkBehaviour
{

    public static StartingSceneController Instance;

    private static PlayMode _choosenPlayMode;

    public enum PlayMode { CouchCoop, Host, Client };

    public static event Action<PlayMode> PlayModeChanged;

    [SerializeField] private NetworkManager networkManager;

    public static PlayMode ChoosenPlayMode
    {
        get => _choosenPlayMode;
        set
        {
            _choosenPlayMode = value;
            PlayModeChanged?.Invoke(_choosenPlayMode);
        }
    }

    [SerializeField] private Button couchCoopBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button joinBtn;
    [SerializeField] private Button quitToDesktopBtn;
    [SerializeField] private Button backButton;

    [SerializeField] private GameObject disconnectedPanel;
    [SerializeField] private GameObject tryingToConnectPanel;
    [SerializeField] private Button retryButton;

    [SerializeField] private Button cancelOnPanelButton;
    [SerializeField] private Button joinOnPanelBtn;
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private TMP_InputField ipAddressInputField;
    private TMP_Text localIPText;
    private TMP_Text requiredErrorText;
    private TMP_Text invalidErrorText;

    private string ipAddress;

    private void SetUpButtonClickListeners()
    {
        couchCoopBtn.onClick.AddListener(() =>
        {
            ChoosenPlayMode = PlayMode.CouchCoop;
            NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallbackHost;
            NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallbackCouchCoop;
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallbackCouchCoop;
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene(Scene.Main_Menu.ToString(), LoadSceneMode.Single);

        });

        hostBtn.onClick.AddListener(() =>
        {
            ChoosenPlayMode = PlayMode.Host;
            NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallbackHost;
            NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallbackCouchCoop;
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallbackHost;
           
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene(Scene.Main_Menu.ToString(), LoadSceneMode.Single);
        });

        joinOnPanelBtn.onClick.AddListener(() =>
        {
            ChoosenPlayMode = PlayMode.Client;
            NetworkManager.Singleton.OnClientStopped += NetworkManager_OnClientStopped;
            UnityTransport transport = GameObject.Find("NetworkManager").GetComponent<UnityTransport>();

            ipAddress = ipAddressInputField.text;
            transport.ConnectionData.Address = ipAddressInputField.text;

            NetworkManager.Singleton.StartClient();
            ShowJoinPanel(false);
            ShowTryingToConnectPanel(true);
        });
        joinBtn.onClick.AddListener(() =>
        {
            ShowJoinPanel(true);
        });
        cancelOnPanelButton.onClick.AddListener(() =>
        {
            ShowJoinPanel(false);
        });

        retryButton.onClick.AddListener(() =>
        {
            HideDisconnectMessagePanel();
            ChoosenPlayMode = PlayMode.Client;
            NetworkManager.Singleton.StartClient();
            ShowTryingToConnectPanel(true);
        });

        quitToDesktopBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        backButton.onClick.AddListener(() =>
        {
            HideDisconnectMessagePanel();
        });
    }

    private void NetworkManager_OnClientStopped(bool obj)
    {
        ShowTryingToConnectPanel(false);
        ShowDisconnectMessagePanel();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == Scene.First.ToString())
        {
            couchCoopBtn = GameObject.Find("CouchCoopButton").GetComponent<Button>();
            hostBtn = GameObject.Find("HostButton").GetComponent<Button>();
            joinBtn = GameObject.Find("JoinButton").GetComponent<Button>();
            quitToDesktopBtn = GameObject.Find("QuitToDesktopButton").GetComponent<Button>();
            disconnectedPanel = GameObject.Find("DisconnectPanel");
            retryButton = GameObject.Find("RetryButton").GetComponent<Button>();
            backButton = GameObject.Find("BackButton").GetComponent<Button>();
            tryingToConnectPanel = GameObject.Find("TryingToConnectPanel");
            joinPanel = GameObject.Find("JoinPanel");
            joinOnPanelBtn = GameObject.Find("JoinOnPanelButton").GetComponent<Button>();
            cancelOnPanelButton = GameObject.Find("CancelOnPanelButton").GetComponent<Button>();
            ipAddressInputField = GameObject.Find("IPAddressInputField").GetComponent<TMP_InputField>();
            localIPText = GameObject.Find("LocalIPtext").GetComponent<TMP_Text>();
            localIPText.text = localIPText.text + GetLocalIPAddress();
            requiredErrorText = GameObject.Find("RequiredErrorText").GetComponent<TMP_Text>();
            invalidErrorText = GameObject.Find("InvalidErrorText").GetComponent<TMP_Text>();

            ipAddressInputField.text = ipAddress;

            SetUpButtonClickListeners();

            HideDisconnectMessagePanel();
            ShowTryingToConnectPanel(false);
            ShowJoinPanel(false);
        }
    }
    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString(); 
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
    private void Update()
    {

        if (ipAddressInputField == null || requiredErrorText == null || invalidErrorText == null || joinOnPanelBtn == null) return;
        string ipPattern = @"^([0-9]{1,3}\.){3}[0-9]{1,3}$";
        if (ipAddressInputField.text.Length == 0 || !Regex.IsMatch(ipAddressInputField.text, ipPattern))
        {
            joinOnPanelBtn.gameObject.SetActive(false);
        }
        else
        {
            joinOnPanelBtn.gameObject.SetActive(true);
        }
        if (ipAddressInputField.text.Length == 0)
        {
            requiredErrorText.gameObject.SetActive(true);
        } else
        {

            requiredErrorText.gameObject.SetActive(false);
        }
        if (!Regex.IsMatch(ipAddressInputField.text, ipPattern))
        {
            invalidErrorText.gameObject.SetActive(true);
        } else
        {
            invalidErrorText.gameObject.SetActive(false);
        }

    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallbackCouchCoop;
        NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallbackHost;
        NetworkManager.Singleton.OnClientStopped -= NetworkManager_OnClientStopped;
    }

    private void ShowDisconnectMessagePanel()
    {
        ShowConnectButtons(false);

        disconnectedPanel.SetActive(true);

        retryButton.gameObject.SetActive(true);
    }


    private void HideDisconnectMessagePanel()
    {
        disconnectedPanel.gameObject.SetActive(false);

        ShowConnectButtons(true);
    }

    private void ShowTryingToConnectPanel(bool show)
    {
        tryingToConnectPanel.SetActive(show);
        ShowConnectButtons(!show);
    }

    private void ShowJoinPanel(bool show)
    {
        joinPanel.SetActive(show);
        ShowConnectButtons(!show);
    }


    private void ShowConnectButtons(bool show)
    {
        hostBtn.gameObject.SetActive(show);
        couchCoopBtn.gameObject.SetActive(show);
        joinBtn.gameObject.SetActive(show);
        quitToDesktopBtn.gameObject.SetActive(show);
    }


    private void NetworkManager_ConnectionApprovalCallbackCouchCoop(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (request.ClientNetworkId == NetworkManager.ServerClientId)
        {
            response.Approved = true;
        }
        else
        {
            response.Approved = false;
        }
    }

    private void NetworkManager_ConnectionApprovalCallbackHost(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (GameStateManager.GetGameStateTypeFromState(GameStateManager.CurrentState) == GameStateType.MainMenu && NetworkManager.Singleton.ConnectedClientsList.Count == 1)
        {
            response.Approved = true;
        }

    }
}