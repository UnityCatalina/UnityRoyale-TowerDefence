using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityRoyale
{
    public class ThinkingPlaceable : Placeable
    {
        public States state = States.Dragged;
        public enum States
        {
            Dragged, //when the player is dragging it as a card on the play field
            Idle, //at the very beginning, when dropped
            Seeking, //going for the target
            Attacking, //attack cycle animation, not moving
            Dead, //dead animation, before removal from play field
        }
        public ThinkingPlaceable target;
        public float hitPoints;
        public float attackRange;
        public float attackRatio;
        public float lastBlowTime;
        public float damage;
        
        public float timeToActNext = 0f;
        public UnityAction<ThinkingPlaceable, ThinkingPlaceable, float> OnDealDamage;

        public virtual void SetTarget(ThinkingPlaceable t)
        {
            target = t;
            t.OnDie += TargetIsDead;
        }

        public virtual void StartAttack()
        {
            state = States.Attacking;
            DealBlow();
        }

        public virtual void DealBlow()
        {
            lastBlowTime = Time.time;
        }

        public virtual void Seek()
        {
            state = States.Seeking;
        }

        protected void TargetIsDead(Placeable p)
        {
            Debug.Log("My target " + p.name + " is dead", gameObject);
            state = States.Idle;
            
            target.OnDie -= TargetIsDead;

            timeToActNext = lastBlowTime + attackRatio;
        }
        
        public bool IsTargetInRange()
        {
            return (transform.position-target.transform.position).sqrMagnitude <= attackRange*attackRange;
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
