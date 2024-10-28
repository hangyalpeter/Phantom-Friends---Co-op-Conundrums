using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartingSceneController : NetworkBehaviour
{

    public static StartingSceneController Instance;

    private static PlayMode _choosenPlayMode;

    public enum PlayMode { CouchCoop, Host, Client };

    public static event Action<PlayMode> PlayModeChanged;

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

        joinBtn.onClick.AddListener(() =>
        {
            ChoosenPlayMode = PlayMode.Client;
            NetworkManager.Singleton.OnClientStopped += NetowrkManager_OnClientStopped;
            NetworkManager.Singleton.StartClient();
            ShowTryingToConnectPanel(true);
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

    private void NetowrkManager_OnClientStopped(bool obj)
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

            SetUpButtonClickListeners();

            HideDisconnectMessagePanel();
            ShowTryingToConnectPanel(false);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallbackCouchCoop;
        NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallbackHost;
        NetworkManager.Singleton.OnClientStopped -= NetowrkManager_OnClientStopped;
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