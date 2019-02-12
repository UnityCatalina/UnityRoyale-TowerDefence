using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UnityRoyale
{
    //humanoid or anyway a walking placeable
    public class Unit : Placeable
    {
        private UnitStates state = UnitStates.Dragged;
        private enum UnitStates
        {
            Dragged, //when the player is dragging the unit as a card on the play field
            Idle, //at the very beginning, when dropped
            Walking, //going for the target
            Attacking, //attack cycle animation, not moving
            Dead, //dead animation, before removal from play field
        }

        private Animator animator;
        private NavMeshAgent navMeshAgent;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();

            navMeshAgent.enabled = false; //allows for previewing the Unit while dragging the card
        }

        public void Activate()
        {
            state = UnitStates.Idle;
            navMeshAgent.enabled = true;

            StartCoroutine(FindTarget(1f));
        }

        private IEnumerator FindTarget(float delay)
        {
            yield return new WaitForSeconds(delay);

            GoTo(Vector3.forward * 3f); //TEST
        }

        private void GoTo(Vector3 targetPosition)
        {
            state = UnitStates.Walking;
            navMeshAgent.SetDestination(targetPosition);
            animator.SetBool("IsMoving", true);
        }
    }
}
