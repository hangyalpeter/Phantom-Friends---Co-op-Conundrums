using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button hostBtn;
    [SerializeField]
    private Button joinBtn;


    private void Awake()
    {
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        joinBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }

}
