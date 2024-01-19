using UnityEngine;
using Mirror;
using System.Collections;

namespace RPG.Core
{
    public class EnemySpawn : NetworkBehaviour
    {
        [SerializeField] GameObject enemyPrefa;
        public float respawnTime = 5f;
        [SerializeField] GameObject spawnPoint;

        [ServerCallback]
        public void OnEnemyDeath()
        {
            StartCoroutine(RespawnEnemy());
        }

        [Server]
        IEnumerator RespawnEnemy()
        {
            yield return new WaitForSeconds(respawnTime);

            GameObject newEnemy = Instantiate(enemyPrefa, spawnPoint.transform.position, Quaternion.identity);
            NetworkServer.Spawn(newEnemy);
        }

    }

}