using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InitializeLevel : MonoBehaviour
{
    [SerializeField]
    private Transform[] PlayerSpawns;

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject player2Prefab;

    // Start is called before the first frame update
    void Start()
    {
        var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();
        var playerFunBars = GameManager.Instance.GetPlayerFunBars().ToArray();
        print(playerConfigs.Length);
        for (int i = 0; i < playerConfigs.Length; i++)
        {
            if (i == 1)
            {
                var player = Instantiate(player2Prefab, PlayerSpawns[i].position, PlayerSpawns[i].rotation, gameObject.transform);
                player.GetComponent<Player>().InitializePlayer(playerConfigs[i], playerFunBars[i], true);

            }
            else 
            {
                var player = Instantiate(playerPrefab, PlayerSpawns[i].position, PlayerSpawns[i].rotation, gameObject.transform);
                player.GetComponent<Player>().InitializePlayer(playerConfigs[i], playerFunBars[i], false);
            }
        }

    }
}
