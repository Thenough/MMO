using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RPG.Core;
using RPG.Controller;
using UnityEngine.AI;

public class EnemyHealth : NetworkBehaviour
{
  public float health = 100;
  private bool isDead = false;

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        SkillProjectile skillProjectile = other.GetComponent<SkillProjectile>();
        if (skillProjectile != null)
        {
            health = Mathf.Max(health - skillProjectile.damage, 0);

            if (health == 0)
            {
                GetComponent<NavMeshAgent>().enabled = false;
                GetComponent<NetworkAnimator>().SetTrigger("Die");
                Invoke("Die", 3f);
                GetComponent<CapsuleCollider>().enabled = false;
                GetComponent<Outline>().enabled = false;
                GetComponent<SelectTarget>().enabled = false;
                isDead = true;
            }
        }
    }
    public bool IsDead()
    {
        return isDead;
    }
    private void Die()
    {
        FindObjectOfType<EnemySpawn>().OnEnemyDeath();
        NetworkServer.Destroy(gameObject);
    }
}
