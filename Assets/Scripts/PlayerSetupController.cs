using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerSetupController : MonoBehaviour
{
    private int playerIndex;

    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private GameObject readyPanel;
    [SerializeField]
    private UnityEngine.UI.Button readyButton;

    private float ignoreInputTime = 0.3f;
    private bool inputEnabled;

    void Awake()
    {
        GameObject readyParent;
        if (PlayerConfigurationManager.Instance.currentPrefab == PlayerConfigurationManager.Instance.player1Prefab)
        {
            readyParent = GameObject.FindGameObjectWithTag("Player1Spawn");
        }
        else
        {
            readyParent = GameObject.FindGameObjectWithTag("Player2Spawn");
        }
        GameObject ready = Instantiate(PlayerConfigurationManager.Instance.currentPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        ready.transform.parent = readyParent.transform;
        ready.transform.rotation = Quaternion.identity;
        ready.transform.localRotation = Quaternion.identity;
        ready.transform.position = Vector3.zero;
        ready.transform.localPosition = Vector3.zero;
        ready.transform.localScale = Vector3.one;

        PlayerConfigurationManager.Instance.currentPrefab = PlayerConfigurationManager.Instance.player2Prefab;
    }
    public void setPlayerIndex(int pi)
    {
        playerIndex = pi;
        titleText.SetText("P" + (pi + 1).ToString());
        ignoreInputTime = Time.time + ignoreInputTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled) { return; }

        PlayerConfigurationManager.Instance.ReadyPlayer(playerIndex);
        GameObject readiedPlayer = GameObject.FindGameObjectsWithTag("PlayerReady")[playerIndex];
        readiedPlayer.GetComponentInChildren<Animator>().SetBool("isBlocking", true);
        readyButton.gameObject.SetActive(false);
    }
}
