using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : NetworkBehaviour
    {
        [SerializeField] private GameObject cam;
        private NavMeshAgent agent;
        private float lastUpdateTime;
        private float lastSpeed;

        private float updateInterval = 0.5f;

        private void Start()
        {
            if (agent == null)
            {
                agent = GetComponent<NavMeshAgent>();
            }

            if (isServer)
            {
                SpawnCamera();
            }
        }

        [Server]
        private void SpawnCamera()
        {
            GameObject camObj = Instantiate(cam);
            NetworkServer.Spawn(camObj);
        }

        private void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            UpdateAnimator();
        }

        public void MoveTo(Vector3 hit)
        {
            agent.destination = hit;
        }

        private void UpdateAnimator()
        {
            
            Vector3 velocity = agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;

            // Sadece belirli bir s�re ge�tikten sonra veya h�z de�i�ti�inde g�ncelle
            if (Time.time - lastUpdateTime > updateInterval || Mathf.Abs(speed - lastSpeed) > 0.1f)
            {
                print("ad�m");
                GetComponent<Animator>().SetFloat("forwardSpeed", speed);
                lastUpdateTime = Time.time;
                lastSpeed = speed;
            }
        }
    }
}
