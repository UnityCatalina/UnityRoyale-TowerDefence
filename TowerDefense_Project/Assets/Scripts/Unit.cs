using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UnityRoyale
{
    //humanoid or anyway a walking placeable
    public class Unit : ThinkingPlaceable
    {
        //data coming from the PlaceableData
        private float speed;
        //TODO: add more as necessary

        private Animator animator;
        private NavMeshAgent navMeshAgent;

        private void Awake()
        {
            pType = Placeable.PlaceableType.Unit;

            //find references to components
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>(); //will be disabled until Activate is called
        }

        //called by GameManager when this Unit is played on the play field
        public void Activate(Faction pFaction, PlaceableData pData)
        {
            faction = pFaction;
            hitPoints = pData.hitPoints;
            targetType = pData.targetType;
            attackRange = pData.attackRange;
            attackRatio = pData.attackRatio;
            speed = pData.speed;
            damage = pData.damagePerAttack;
            //TODO: add more as necessary
            
            navMeshAgent.speed = speed;
            animator.SetFloat("MoveSpeed", speed); //will act as multiplier to the speed of the run animation clip

            state = States.Idle;
            navMeshAgent.enabled = true;
        }

        public void StartAttack()
        {
            state = States.Attacking;
            navMeshAgent.isStopped = true;
            animator.SetBool("IsMoving", false);
            DealAttack();
        }

        private void DealAttack()
        {
            Debug.Log("Dealing attack at " + Time.time, gameObject);
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;

            transform.forward = (target.transform.position - transform.position).normalized; //turn towards the target

            if(OnDealDamage != null)
                OnDealDamage(this, target, damage);
        }

        public override void SetTarget(ThinkingPlaceable t)
        {
            base.SetTarget(t);
            Debug.Log("Setting target");
        }

        private void WalkToTarget()
        {
            if(target == null)
                return;

            state = States.Walking;
            navMeshAgent.SetDestination(target.transform.position);
            navMeshAgent.isStopped = false;
            animator.SetBool("IsMoving", true);
        }

        public override void UpdateLoop()
        {
            switch(state)
            {
                case States.Idle:
                    WalkToTarget();
                    break;


                case States.Walking:
                    if(IsTargetInRange())
                    {
                        StartAttack();
                    }
                    else
                    {
                        WalkToTarget();
                    }
                    break;


                case States.Attacking:
                    if(IsTargetInRange())
                    {
                        if(Time.time >= lastAttackTime + attackRatio)
                        {
                            DealAttack();
                        }
                    }
                    else
                    {
                        Debug.Log("Repositioning during attack", gameObject);
                        state = States.Idle;
                    }
                    break;
            }
        }

        protected override void Die()
        {
            base.Die();

            navMeshAgent.enabled = false;
            animator.SetTrigger("IsDead");
        }
    }
}
