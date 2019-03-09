using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityRoyale
{
    public class CPUOpponent : MonoBehaviour
    {
        public DeckData aiDeck;
        public UnityAction<CardData, Vector3, Placeable.Faction> OnCardUsed;

        private bool act = false;
        private Coroutine actingCoroutine;

        public void LoadDeck()
        {
            DeckLoader newDeckLoaderComp = gameObject.AddComponent<DeckLoader>();
            newDeckLoaderComp.OnDeckLoaded += DeckLoaded;
            newDeckLoaderComp.LoadDeck(aiDeck);
        }

        //...

		private void DeckLoaded()
		{
			Debug.Log("AI deck loaded");

			//StartActing();
        }

		public void StartActing()
		{
			Invoke("Bridge", 0f);
		}

        private void Bridge()
        {
            act = true;
            actingCoroutine = StartCoroutine(CreateRandomCards());
        }

        public void StopActing()
        {
            act = false;
            StopCoroutine(actingCoroutine);
        }

        //TODO: create a proper AI
		private IEnumerator CreateRandomCards()
		{
            while(act)
            {
                if(OnCardUsed != null)
                    OnCardUsed(aiDeck.GetNextCardFromDeck(), Vector3.forward*3f, Placeable.Faction.Opponent);
				
			    yield return new WaitForSeconds(5f);
            }
		}
	}
}