using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Puncher : MonoBehaviour
{
    //private Animator animator;
    public float lightPunchDamageAmount = 10f;
    public float heavyPunchDamageAmount = 20f;
    public bool heavyOnly;
    Player player;

    void Start()
    {
        //animator = GetComponentInParent<Animator>();
        player = gameObject.GetComponentInParent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (heavyOnly && !player.getIsHeavyPunch())
        {
            return;
        }
        if (other.gameObject.tag != gameObject.tag && !player.getHasHit() && (player.getIsLightPunch() || player.getIsHeavyPunch()))
        {
            Player otherPlayer = other.gameObject.GetComponentInParent<Player>();
            player.setHasHit(true);
            float damageAmount = 0f;
            if (player.getIsLightPunch())
            {
                damageAmount = lightPunchDamageAmount;
                otherPlayer.pushBackLightPunch();
            }
            else if (player.getIsHeavyPunch())
            {
                damageAmount = heavyPunchDamageAmount;
            }
            if (otherPlayer.getIsBlocking()) 
            {
                damageAmount /= 2;
            }
            if (player.getIsHeavyPunch())
            {
                StartCoroutine(delayHeavyPunch(0.2f, otherPlayer, damageAmount));
                return;
            }
            print("Increase bar by:" + damageAmount);
            otherPlayer.increaseFunBar(damageAmount);
        }
    }

    IEnumerator delayHeavyPunch(float delay, Player otherPlayer, float damageAmount)
    {
        yield return new WaitForSeconds(delay);
        if (!otherPlayer.getHasHit())
        {
            otherPlayer.pushBackHeavyPunch();
            print("Increase bar by:" + damageAmount);
            otherPlayer.increaseFunBar(damageAmount);
        }
    }
}
