using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace UnityRoyale
{
    public class Building : ThinkingPlaceable
    {
		//Inspector references
		[Header("Timelines")]
		public PlayableDirector constructionTimeline;
		public PlayableDirector destructionTimeline;

        private void Awake()
        {
            pType = PlaceableType.Building;
        }

        public void Activate(Faction pFaction, PlaceableData pData)
        {
            faction = pFaction;
            hitPoints = pData.hitPoints;
            targetType = pData.targetType;

			constructionTimeline.Play();
        }

        protected override void Die()
        {
            base.Die();

            Debug.Log("Building is dead", gameObject);
			destructionTimeline.Play();
        }
    }
}