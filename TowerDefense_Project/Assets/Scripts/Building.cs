using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRoyale
{
    public class Building : ThinkingPlaceable
    {
        private void Awake()
        {
            pType = PlaceableType.Building;
        }

        public void Activate(Faction pFaction, PlaceableData pData)
        {
            faction = pFaction;
            hitPoints = pData.hitPoints;
            targetType = pData.targetType;
        }

        protected override void Die()
        {
            base.Die();

            Debug.Log("Building is dead", gameObject);
        }
    }
}