using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : NetworkBehaviour
    {
        public Animator animator;
        public List<GameObject> projectilePrefab;
        public Transform projectileMount;
        public void Attack()
        {
           
                CmdAttack();
            
        }

        [Command]
        public void CmdAttack()
        {
            
            RpcOnFire0();
            
        }
        public void Hit()
        {
            if (isServer)
            {
                GameObject projectile = Instantiate(projectilePrefab[0], projectileMount.position, projectileMount.rotation);
                NetworkServer.Spawn(projectile);
            }
        }

        // this is called on the tank that fired for all observers
        [ClientRpc]
        void RpcOnFire0()
        {
                animator.SetTrigger("Attack");
        }

    }
}
