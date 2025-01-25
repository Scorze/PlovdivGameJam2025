using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static float maxFunBar = 200f;

    public List<GameObject> playerFunBars;
    public ParticleSystem lightPunchParticle;
    public ParticleSystem heavyPunchParticle;

    private bool gameOver;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        PlayerControls controls = new PlayerControls();
        foreach (var item in controls.bindings)
        {
            print(item.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            return;
        }
        Player[] players = FindObjectsOfType<Player>();
        foreach (var player in players)
        {
            if (player.getCurrentFunBar() >= maxFunBar) 
            {
                print(player.name + " has lost!");
                player.setFrozen(true);
                player.triggerAnimation("lost");
                foreach (var winningPlayer in players.Where(p => p.name != player.name))
                {
                    print(winningPlayer.name + " has won!");
                    winningPlayer.setFrozen(true);
                    winningPlayer.triggerAnimation("won");
                }
                StartCoroutine(startNewGame(5f));
                gameOver = true;
            }
        }
    }

    public List<GameObject> GetPlayerFunBars()
    {
        return playerFunBars;
    }

    IEnumerator startNewGame(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(GameObject.Find("PlayerConfigurationManager"));
        SceneManager.LoadScene("PlayerSetup");
    }
}
