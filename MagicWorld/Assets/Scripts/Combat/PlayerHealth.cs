using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RPG.Core;
using UnityEngine.AI;

public class PlayerHealth : NetworkBehaviour
{
  public float health = 100;

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        SkillProjectile skillProjectile = other.GetComponent<SkillProjectile>();
        if (skillProjectile != null)
        {
            health = Mathf.Max(health - skillProjectile.damage, 0);
            if (health == 0)
            {
               
                RpcDiePlayer();
            }
        }
    }

    [ClientRpc]
    void RpcDiePlayer()
    {
        GetComponent<NetworkAnimator>().SetTrigger("DiePlayer");
    }
}
