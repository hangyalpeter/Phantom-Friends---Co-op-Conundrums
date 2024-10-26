using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartingSceneController : MonoBehaviour
{

    public static StartingSceneController Instance;

    public enum PlayMode {CouchCoop, Host, Client};

    public static PlayMode ChoosenPlayMode {  get; private set; }

    [SerializeField] private Button couchCoopBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button joinBtn;

    [SerializeField] private GameObject disconnectedPanel;
    [SerializeField] private GameObject tryingToConnectPanel;
    [SerializeField] private Button retryButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Instance = this;
            Destroy(this);
        } else
        {
            Instance = this;
        }

        couchCoopBtn.onClick.AddListener(() =>
        {
            ChoosenPlayMode = PlayMode.CouchCoop;
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallbackCouchCoop;
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);

        });

        hostBtn.onClick.AddListener(() =>
        {
            ChoosenPlayMode = PlayMode.Host;
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallbackHost;
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        });

        joinBtn.onClick.AddListener(() =>
        {
            ChoosenPlayMode = PlayMode.Client;
            NetworkManager.Singleton.StartClient();
            ShowTryingToConnectPanel(true);
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        });


        retryButton.onClick.AddListener(() =>
        {
            HideDisconnectMessagePanel();
            ChoosenPlayMode = PlayMode.Client;
            NetworkManager.Singleton.StartClient();
            ShowTryingToConnectPanel(true);
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        });

    }
    private void Start()
    {
        HideDisconnectMessagePanel();
        ShowTryingToConnectPanel(false);
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        HideDisconnectMessagePanel();
        ShowTryingToConnectPanel(false);
    }


    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        ShowTryingToConnectPanel(false);
        ShowDisconnectMessagePanel();
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
    }


    private void NetworkManager_ConnectionApprovalCallbackCouchCoop(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = false;
    }

    private void NetworkManager_ConnectionApprovalCallbackHost(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
    }

}