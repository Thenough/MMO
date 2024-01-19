using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using System;
using RPG.Core;

namespace RPG.Controller
{
    public class IAController : NetworkBehaviour
    {
        [SerializeField] float attackDistance;
        [SerializeField] float fallowDistance;
        [SerializeField] float suspicionTime = 5f;
        [SerializeField] PatrolPath patrolPath;
        private float waypointTolarance = 1f;
        private float wayPointLifeTime = 5f;
        GameObject player;
        NavMeshAgent agent;
        EnemyHealth enemyhealth;
        

        Vector3 enemylocation;
        float timeSinceLastSawPlayer;
        float timeSinceArrivedWayPoint;
        int currentWayPointIndex = 0;
        void Start()
        {
            if (agent == null)
            {
                agent = GetComponent<NavMeshAgent>();
            }

            enemyhealth = GetComponent<EnemyHealth>();

            enemylocation = transform.position;
        }

        void FixedUpdate()
        {
            if(enemyhealth.IsDead())
            {
                return;
            }
            UpdateAnimator();
            if(DistanceToPlayer() < fallowDistance)
            {
                agent.destination = player.transform.position;
                GetComponent<Animator>().SetTrigger("stopp");
                agent.isStopped = false;
                agent.speed = 3.5f; // Hýzý sýfýrla
                agent.acceleration = 8f;
                timeSinceLastSawPlayer = 0;

            }
            else if(timeSinceLastSawPlayer < suspicionTime)
            {
                Stop();
            }
            else
            {
                Vector3 nextPosition = enemylocation;
                if (patrolPath != null)
                {
                    if (AtWayPoint())
                    {
                        timeSinceArrivedWayPoint = 0;
                        CycleWayPoint();
                    }
                    nextPosition = GetNextWayPoint();
                }

                if (timeSinceArrivedWayPoint > wayPointLifeTime)
                {
                    agent.isStopped = false;
                    agent.speed = 2f; // Hýzý sýfýrla
                    agent.acceleration = 8f;
                    agent.destination = nextPosition;
                }

            }
            if(DistanceToPlayer() < attackDistance)
            {
                Stop();
                this.transform.LookAt(player.transform.position);
                GetComponent<NetworkAnimator>().SetTrigger("MutantAttack");
               
            }
            else
            {
                GetComponent<NetworkAnimator>().ResetTrigger("MutantAttack");
            }
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedWayPoint += Time.deltaTime;
        }

        private Vector3 GetNextWayPoint()
        {
            return patrolPath.GetWayPointPosition(currentWayPointIndex);
        }

        private void CycleWayPoint()
        {
            currentWayPointIndex = patrolPath.GetNextIndex(currentWayPointIndex);
        }

        private bool AtWayPoint()
        {
            float distanceWayPoint = Vector3.Distance(transform.position,GetNextWayPoint());
            return distanceWayPoint < attackDistance;
        }

        private float DistanceToPlayer()
        {
            if (player == null)
            {
                player = GameObject.FindWithTag("Player");
            }
            return Vector3.Distance(player.transform.position,transform.position);
        }
        private void UpdateAnimator()
        {
            Vector3 velocity = agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("MutantForward", speed);
        }

        public void Stop()
        {
            if (agent != null)
            {
                agent.velocity = Vector3.zero;
                agent.speed = 0f; // Hýzý sýfýrla
                agent.acceleration = 0f; // Ývmeyi sýfýrla
                agent.isStopped = true;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,fallowDistance);
        }
    }
}