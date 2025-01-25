using System;
using System.Collections.Generic;
using Kart;
using TMPro;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    
    public GameObject[] checkpoints;
    
    public TMP_Text gameWonText;
    
    private Dictionary<ulong, int> playerToCheckPoint = new Dictionary<ulong, int>();

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
    }

    public void AddPlayer(ulong playerId)
    {
        playerToCheckPoint.Add(playerId, 0);
    }

    public void PassCheckpoint(GameObject checkPoint, GameObject player)
    {
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
            }
            playerToCheckPoint[playerId]++;
        }
        else
        {
            print("Incorrect checkpoint");
        }
    }
}
