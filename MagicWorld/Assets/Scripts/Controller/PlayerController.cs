using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Combat;
using UnityEngine.AI;

namespace RPG.Controller
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] ParticleSystem clickEffect;
        private SelectTarget currentTarget;
        NavMeshAgent navMeshAgent;
        public void Start()
        {
          navMeshAgent = GetComponent<NavMeshAgent>();
        }
        private void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }
            if(TargetSelect() == true)
            {
                return;
            }
            InteractWithMovement();
            
        }
        private void InteractWithMovement()
        {
                MoveToCursor();
        }

        private bool TargetSelect()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            SelectTarget newTarget = null; // Yeni se�ilen hedefin referans�

            if (hasHit)
            {
                SelectTarget target = hit.collider.GetComponent<SelectTarget>();
                if (target != null)
                {
                    newTarget = target;
                    if (Input.GetMouseButtonDown(0))
                    {
                        // E�er yeni hedef varsa ve �nceki hedef farkl� ise
                        if (newTarget != currentTarget)
                        {
                            // �nceki hedefin Outline'�n� kapat
                            if (currentTarget != null)
                            {
                                Outline outline = currentTarget.GetComponent<Outline>();
                                if (outline != null)
                                {
                                    outline.enabled = false;
                                }
                            }

                            // Yeni hedefin Outline'�n� a�
                            Outline newOutline = newTarget.GetComponent<Outline>();
                            if (newOutline != null)
                            {
                                newOutline.enabled = true;
                            }

                            // Yeni hedefi �u anki hedef olarak ayarla
                            currentTarget = newTarget;
                        }
                    }
                    return true;
                }
            }
            if (Input.GetKeyDown(KeyCode.F) && currentTarget != null)
            {
                    Stop();
                    this.transform.LookAt(currentTarget.transform.position);
                    GetComponent<Fighter>().Attack();
                    return true;
            }
            // E�er hi�bir hedef se�ilmediyse ve �nceki hedef varsa, �nceki hedefin Outline'�n� kapat
            if (currentTarget != null && Input.GetMouseButtonDown(0))
            {
                Outline outline = currentTarget.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = false;
                }
                
                currentTarget = null; // �u anki hedefi temizle
            }

            // E�er t�klanan yer terrain collider ise ve �nceki hedef varsa, �nceki hedefin Outline'�n� kapat
            if (newTarget == null && Input.GetMouseButtonDown(0))
            {
                if (currentTarget != null)
                {
                    Outline outline = currentTarget.GetComponent<Outline>();
                    if (outline != null)
                    {
                        outline.enabled = false;
                    }
                    currentTarget = null; // �u anki hedefi temizle
                }

                // Terrain collider ise print("terrain") yapabilirsiniz.
                // Bu duruma �zel bir i�lem eklemek istiyorsan�z burada gerekli kodu ekleyebilirsiniz.
            }
            return false;
        }
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        public void Stop()
        {
            if (navMeshAgent != null && isLocalPlayer)
            {
                navMeshAgent.velocity = Vector3.zero;
                navMeshAgent.speed = 0f; // H�z� s�f�rla
                navMeshAgent.acceleration = 0f; // �vmeyi s�f�rla
                navMeshAgent.isStopped = true;
            }
        }
        public void MoveToCursor()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GetComponent<Animator>().SetTrigger("stopAttack");
                    navMeshAgent.isStopped = false;
                    navMeshAgent.speed = 7f; // H�z� s�f�rla
                    navMeshAgent.acceleration = 8f;
                    if (clickEffect != null)
                    {
                        ParticleSystem click = Instantiate(clickEffect, hit.point += new Vector3(0,0.2f,0),clickEffect.transform.rotation);
                        Destroy(click.gameObject, 1);
                    }
                    GetComponent<Mover>().MoveTo(hit.point);
                }
            }
        }
    }
}