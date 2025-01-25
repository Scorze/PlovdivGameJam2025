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
    public TMP_Text gameStarsText;
    
    private Dictionary<ulong, int> playerToCheckPoint = new Dictionary<ulong, int>();
    private bool isGameStarted = false;
    private int gameStartsIn = 3;

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
        StartCoroutine(CountDown());
    }

    private void Update()
    {
        if (!isGameStarted)
        {
            gameStarsText.text = "Game starts in " + gameStartsIn;
        }
    }

    public void AddPlayer(ulong playerId, GameObject player)
    {
        if (!IsHost)
        {
            return;
        }
        playerToCheckPoint.Add(playerId, 0);
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
        KartController kartController = player.GetComponent<KartController>();
        ulong playerId = kartController.OwnerClientId;
        print($"Player {playerId + 1} ate grass!");
        kartController.EatGrass();
        StartCoroutine(Respawn(grass, 5f));
        EatGrassClientRpc(grass.GetComponent<NetworkObject>().NetworkObjectId, player.GetComponent<KartController>().NetworkObjectId);
    }

    [ClientRpc]
    void EatGrassClientRpc(ulong grassNetworkId, ulong playerNetworkId)
    {
        GameObject grass = NetworkManager.Singleton.SpawnManager.SpawnedObjects[grassNetworkId].gameObject;
        GameObject player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetworkId].gameObject;
        KartController kartController = player.GetComponent<KartController>();
        if (!kartController.IsHost)
        {
            player.GetComponent<KartController>().EatGrass();
        }
        StartCoroutine(Respawn(grass, 5f));
    }
    
    IEnumerator Respawn(GameObject grass, float timeToRespawn) {
        grass.SetActive(false);
        yield return new WaitForSeconds(timeToRespawn);
        grass.SetActive(true);
    }
    
    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(1f);
        gameStartsIn--;
        if (gameStartsIn > 0)
        {
            StartCoroutine(CountDown());
        }
        else
        {
            gameStarsText.gameObject.SetActive(false);
            isGameStarted = true;
        }
    }

    public bool GetIsGameStarted()
    {
        return isGameStarted;
    }
}
