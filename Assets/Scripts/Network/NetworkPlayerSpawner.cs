using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerChildPrefab;
    [SerializeField] private GameObject playerGhostPrefab;

    private void Start()
    {
        if (IsClient && !IsServer)
        {
            // comment/uncomment for / couch/ online
            RequestSpawnPlayerServerRpc();
        }

        if (IsServer)
        {
            // comment/uncomment for / couch/ online
            SpawnPlayer(NetworkManager.Singleton.LocalClientId);

            //Couch-coop TODO: get it from a singleton if we should make it couch-coop or not, maybe sessionmanager or something like that could store it
            //SpawnLocalPlayersForCouchCoop();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnPlayerServerRpc(ServerRpcParams rpcParams = default)
    {
        SpawnPlayer(rpcParams.Receive.SenderClientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        GameObject playerPrefabToSpawn;

        if (clientId == NetworkManager.Singleton.LocalClientId) // Host player
        {
            playerPrefabToSpawn = playerGhostPrefab; // Host gets playerChildPrefab
        }
        else 
        {
            playerPrefabToSpawn = playerChildPrefab; // Clients get playerGhostPrefab
        }

        GameObject playerInstance = Instantiate(playerPrefabToSpawn, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();

        networkObject.SpawnAsPlayerObject(clientId, destroyWithScene: true);
    }


    private void SpawnLocalPlayersForCouchCoop()
    {
        GameObject player1Instance = Instantiate(playerChildPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkObject player1NetworkObject = player1Instance.GetComponent<NetworkObject>();
        player1NetworkObject.SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId, destroyWithScene: true);

        GameObject player2Instance = Instantiate(playerGhostPrefab, new Vector3(2, 0, 0), Quaternion.identity); 
        NetworkObject player2NetworkObject = player2Instance.GetComponent<NetworkObject>();
        player2NetworkObject.SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId, destroyWithScene: true);
    }
   
}
