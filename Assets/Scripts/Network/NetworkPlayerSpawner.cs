using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerChildPrefab;
    [SerializeField] private GameObject playerGhostPrefab;

   
    public override void OnNetworkSpawn()
    {
        if (IsClient && !IsServer) // Clients (excluding the host) request player spawn via ServerRPC
        {
            RequestSpawnPlayerServerRpc();
        }

        if (IsServer) // The host spawns itself as the server
        {
            SpawnPlayer(NetworkManager.Singleton.LocalClientId); // Host spawns its own player
        }

        // couch-coop
        /*  if (IsServer)
          {
              SpawnPlayerPair();
          }*/
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnPlayerServerRpc(ServerRpcParams rpcParams = default)
    {
        // Server handles the player spawning for the requesting client
        SpawnPlayer(rpcParams.Receive.SenderClientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        GameObject playerPrefabToSpawn;

        // TODO make it choosable later from a dialog maybe? low prio

        // Decide which prefab to spawn based on whether this is the host or a client
        if (clientId == NetworkManager.Singleton.LocalClientId) // Host player
        {
            playerPrefabToSpawn = playerChildPrefab; // Host gets playerChildPrefab
        }
        else // Any other client
        {
            playerPrefabToSpawn = playerGhostPrefab; // Clients get playerGhostPrefab
        }

        // Instantiate and spawn the player prefab
        GameObject playerInstance = Instantiate(playerPrefabToSpawn, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();

        // Spawn the player object for the specific client
        networkObject.SpawnAsPlayerObject(clientId);
    }   



     private void SpawnPlayerPair()
    {
        // Spawn Player 1 and assign ownership to the host
        GameObject player1 = Instantiate(playerChildPrefab);
        NetworkObject netObj1 = player1.GetComponent<NetworkObject>();
        netObj1.SpawnWithOwnership(NetworkManager.Singleton.LocalClientId); // Host owns Player 1

        // Spawn Player 2 and also assign ownership to the host
        GameObject player2 = Instantiate(playerGhostPrefab);
        NetworkObject netObj2 = player2.GetComponent<NetworkObject>();
        netObj2.SpawnWithOwnership(NetworkManager.Singleton.LocalClientId); // Host owns Player 2
    }

}
