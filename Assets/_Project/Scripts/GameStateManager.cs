using System;
using System.Collections;
using System.Collections.Generic;
using Kart;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameStateManager : NetworkBehaviour
{
    public static GameStateManager Instance { get; private set; }
    
    public GameObject[] checkpoints;
    
    public TMP_Text gameWonText;
    
    private Dictionary<ulong, int> playerToCheckPoint = new Dictionary<ulong, int>();
    private Dictionary<ulong, GameObject> playerIdToPlayer = new Dictionary<ulong, GameObject>();
    private List<GameObject> grasses = new List<GameObject>();

    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
        grasses.AddRange(GameObject.FindGameObjectsWithTag("Grass"));
    }

    public void AddPlayer(ulong playerId, GameObject player)
    {
        if (!IsHost)
        {
            return;
        }
        playerToCheckPoint.Add(playerId, 0);
        playerIdToPlayer.Add(playerId, player);
    }

    public void PassCheckpoint(GameObject checkPoint, GameObject player)
    {
        if (!IsHost)
        {
            return;
        }
        int checkpointIndex = 0;
        bool found = false;
        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (checkPoint == checkpoints[i])
            {
                checkpointIndex = i;
                found = true;
                break;
            }
        }

        if (!found)
        {
            return;
        }

        ulong playerId = player.GetComponent<KartController>().OwnerClientId;
        
        if (checkpointIndex == playerToCheckPoint[playerId])
        {
            print("Correct checkpoint");
            if (playerToCheckPoint[playerId] == checkpoints.Length - 1)
            {
                print($"Player {playerId + 1} won the game!");
                gameWonText.text = $"Player {playerId + 1} won the game!";
                gameWonText.gameObject.SetActive(true);
                SetTextClientRpc(playerId);
            }
            playerToCheckPoint[playerId]++;
        }
        else
        {
            print("Incorrect checkpoint");
        }
    }

    [ClientRpc]
    void SetTextClientRpc(ulong playerId)
    {
        gameWonText.text = $"Player {playerId + 1} won the game!";
        gameWonText.gameObject.SetActive(true);
    }

    public void EatGrass(GameObject grass, GameObject player)
    {
        if (!IsHost)
        {
            return;
        }
        ulong playerId = player.GetComponent<KartController>().OwnerClientId;
        print($"Player {playerId + 1} ate grass!");
        StartCoroutine(Respawn(grass, 5f));
        EatGrassClientRpc(grass.GetComponent<NetworkObject>().NetworkObjectId);
    }

    [ClientRpc]
    void EatGrassClientRpc(ulong grassNetworkId)
    {
        GameObject grass = NetworkManager.Singleton.SpawnManager.SpawnedObjects[grassNetworkId].gameObject;
        StartCoroutine(Respawn(grass, 5f));
    }
    
    IEnumerator Respawn(GameObject grass, float timeToRespawn) {
        grass.SetActive(false);
        yield return new WaitForSeconds(timeToRespawn);
        grass.SetActive(true);
    }
}
