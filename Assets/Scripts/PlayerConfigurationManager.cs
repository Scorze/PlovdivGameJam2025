using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerConfigurationManager : MonoBehaviour
{
    private List<PlayerConfiguration> playerConfigs;
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public GameObject currentPrefab;

    [SerializeField]
    private int MinPlayers = 2;

    public static PlayerConfigurationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            playerConfigs = new List<PlayerConfiguration>();
            currentPrefab = player1Prefab;
        }
    }

    public void ReadyPlayer(int index) 
    {
        playerConfigs[index].isReady = true;
        print("ready");
        if (playerConfigs.Count >= MinPlayers && playerConfigs.All(p => p.isReady == true))
        {
            SceneManager.LoadScene("Game");
        }
    }

    public void HandlePlayerJoined(PlayerInput pi)
    {
        Debug.Log("Player joined" + pi.playerIndex);

        if (!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            pi.transform.SetParent(transform);
            playerConfigs.Add(new PlayerConfiguration(pi));
        }
    }

    public List<PlayerConfiguration> GetPlayerConfigs()
    {
        return playerConfigs;
    }
}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi) 
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
    }
    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }
    public bool isReady { get; set; }

}