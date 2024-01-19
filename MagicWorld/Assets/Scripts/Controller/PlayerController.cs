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
            SelectTarget newTarget = null; // Yeni seçilen hedefin referansý

            if (hasHit)
            {
                SelectTarget target = hit.collider.GetComponent<SelectTarget>();
                if (target != null)
                {
                    newTarget = target;
                    if (Input.GetMouseButtonDown(0))
                    {
                        // Eðer yeni hedef varsa ve önceki hedef farklý ise
                        if (newTarget != currentTarget)
                        {
                            // Önceki hedefin Outline'ýný kapat
                            if (currentTarget != null)
                            {
                                Outline outline = currentTarget.GetComponent<Outline>();
                                if (outline != null)
                                {
                                    outline.enabled = false;
                                }
                            }

                            // Yeni hedefin Outline'ýný aç
                            Outline newOutline = newTarget.GetComponent<Outline>();
                            if (newOutline != null)
                            {
                                newOutline.enabled = true;
                            }

                            // Yeni hedefi þu anki hedef olarak ayarla
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
            // Eðer hiçbir hedef seçilmediyse ve önceki hedef varsa, önceki hedefin Outline'ýný kapat
            if (currentTarget != null && Input.GetMouseButtonDown(0))
            {
                Outline outline = currentTarget.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = false;
                }
                
                currentTarget = null; // Þu anki hedefi temizle
            }

            // Eðer týklanan yer terrain collider ise ve önceki hedef varsa, önceki hedefin Outline'ýný kapat
            if (newTarget == null && Input.GetMouseButtonDown(0))
            {
                if (currentTarget != null)
                {
                    Outline outline = currentTarget.GetComponent<Outline>();
                    if (outline != null)
                    {
                        outline.enabled = false;
                    }
                    currentTarget = null; // Þu anki hedefi temizle
                }

                // Terrain collider ise print("terrain") yapabilirsiniz.
                // Bu duruma özel bir iþlem eklemek istiyorsanýz burada gerekli kodu ekleyebilirsiniz.
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
                navMeshAgent.speed = 0f; // Hýzý sýfýrla
                navMeshAgent.acceleration = 0f; // Ývmeyi sýfýrla
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
                    navMeshAgent.speed = 7f; // Hýzý sýfýrla
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