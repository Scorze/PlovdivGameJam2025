using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Kart {
    public class KartSpawner : NetworkBehaviour {
        [SerializeField] Transform[] spawnPoints;
        
        [SerializeField] GameObject playerKartPrefab;
        
        private NetworkObject spawnedNetworkObject;
        private int currentSpawnPoint = 0;

        public override void OnNetworkSpawn()
        {
            NetworkManager.Singleton.OnClientConnectedCallback  += OnClientConnected;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
        }

        private void OnSceneLoaded(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
        {
            if (scenename == "Main" && IsHost)
            {
                foreach (ulong clientId in clientscompleted)
                {
                    Debug.Log($"[Spawning] Client-{clientId} is spawning at spawn point-{currentSpawnPoint} with a position of {spawnPoints[currentSpawnPoint].position}.");
                    GameObject instantiatedPlayer = Instantiate(playerKartPrefab, spawnPoints[currentSpawnPoint].position, spawnPoints[currentSpawnPoint].rotation);
                    instantiatedPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                    GameStateManager.Instance.AddPlayer(clientId, instantiatedPlayer);
                    currentSpawnPoint++;
                }
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            enabled = IsServer;
            if (!enabled || playerKartPrefab == null)
            {
                return;
            }
            Debug.Log($"[Spawning] Client-{clientId} is spawning at spawn point-{currentSpawnPoint} with a position of {spawnPoints[currentSpawnPoint].position}.");
            GameObject instantiatedPlayer = Instantiate(playerKartPrefab, spawnPoints[currentSpawnPoint].position, spawnPoints[currentSpawnPoint].rotation);
            instantiatedPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            GameStateManager.Instance.AddPlayer(clientId, instantiatedPlayer);
            currentSpawnPoint++;
        }
        // Use this to spawn objects
        /* public override void OnNetworkSpawn()
        {
            // Only the server spawns, clients will disable this component on their side
            enabled = IsServer;
            if (!enabled || playerKartPrefab == null)
            {
                return;
            }
            // Instantiate the GameObject Instance
            playerKartPrefab = Instantiate(playerKartPrefab);

            // Optional, this example applies the spawner's position and rotation to the new instance
            playerKartPrefab.transform.position = spawnPoints[currentSpawnPoint].position;
            playerKartPrefab.transform.rotation = spawnPoints[currentSpawnPoint].rotation;
            currentSpawnPoint++;

            // Get the instance's NetworkObject and Spawn
            spawnedNetworkObject = playerKartPrefab.GetComponent<NetworkObject>();
            spawnedNetworkObject.Spawn();
        }*/
    }
}