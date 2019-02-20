using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityRoyale
{
    public class ThinkingPlaceable : Placeable
    {
        protected States state = States.Dragged;
        protected enum States
        {
            Dragged, //when the player is dragging it as a card on the play field
            Idle, //at the very beginning, when dropped
            Walking, //going for the target
            Attacking, //attack cycle animation, not moving
            Dead, //dead animation, before removal from play field
        }
        protected ThinkingPlaceable target;
        protected Vector3 targetPosition;
        protected float hitPoints;
        protected float attackRange;
        protected float attackRatio;
        protected float lastAttackTime;
        protected float damage;
        
        public float timeToActNext = 0f;
        public UnityAction<ThinkingPlaceable, ThinkingPlaceable, float> OnDealDamage;

        public virtual void SetTarget(ThinkingPlaceable t)
        {
            target = t;
            t.OnDie += TargetIsDead;
            targetPosition = target.transform.position;
        }

        public bool IsIdle()
        {
            return state == States.Idle;
        }

        public bool IsDead()
        {
            return state == States.Dead;
        }

        public virtual void UpdateLoop()
        {

        }

        protected void TargetIsDead(Placeable p)
        {
            Debug.Log("My target " + p.name + " is dead", gameObject);
            state = States.Idle;
            
            target.OnDie -= TargetIsDead;
            target = null;

            timeToActNext = lastAttackTime + attackRatio;
        }
        
        protected bool IsTargetInRange()
        {
            return (transform.position-target.transform.position).sqrMagnitude <= attackRange*attackRange;
        }

        protected bool DidTargetMove()
        {
            return targetPosition == target.transform.position;
        }

        public void SufferDamage(float amount)
        {
            hitPoints -= amount;
            Debug.Log("Suffering damage, new health: " + hitPoints, gameObject);
            if(hitPoints <= 0f)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            state = States.Dead;
            OnDie(this);
        }
    }
}
