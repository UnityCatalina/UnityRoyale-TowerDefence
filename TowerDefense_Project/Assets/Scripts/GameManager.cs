using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UnityRoyale
{
    public class GameManager : MonoBehaviour
    {
        public NavMeshSurface navMesh;
		public GameObject playersCastle, opponentCastle;
        public PlaceableData castlePData;

        [Header("Managers")]
        private CardManager cardManager;
        private CPUOpponent CPUOpponent;
        private InputManager inputManager;

        private List<ThinkingPlaceable> playerUnits, opponentUnits;
        private List<ThinkingPlaceable> playerBuildings, opponentBuildings;
        private List<ThinkingPlaceable> allPlayers, allOpponents; //contains both Buildings and Units
		private List<ThinkingPlaceable> allThinkinPlaceables;
        private bool gameOver = false;
        private bool updateAllPlaceables = false; //used to force an update of all AIBrains in the Update loop
        private const float THINKING_DELAY = 3f;

        private void Awake()
        {
            cardManager = GetComponent<CardManager>();
            CPUOpponent = GetComponent<CPUOpponent>();
            inputManager = GetComponent<InputManager>();

            //listeners on other managers
            cardManager.OnCardUsed += UseCard;
            CPUOpponent.OnCardUsed += UseCard;

            //initialise Placeable lists, for the AIs to pick up and find a target
            playerUnits = new List<ThinkingPlaceable>();
            playerBuildings = new List<ThinkingPlaceable>();
            opponentUnits = new List<ThinkingPlaceable>();
            opponentBuildings = new List<ThinkingPlaceable>();
            allPlayers = new List<ThinkingPlaceable>();
            allOpponents = new List<ThinkingPlaceable>();
			allThinkinPlaceables = new List<ThinkingPlaceable>();

			//Insert castles into lists
			SetupPlaceable(playersCastle, castlePData, Placeable.Faction.Player);
            SetupPlaceable(opponentCastle, castlePData, Placeable.Faction.Opponent);
        }

        private void Start()
        {
			cardManager.LoadDeck();
            CPUOpponent.LoadDeck();
        }

        //the Update loop pings all the AIBrains in the scene, and makes them act
        private void Update()
        {
            if(gameOver)
                return;

            ThinkingPlaceable targetToPass; //ref
			ThinkingPlaceable p; //ref

			for(int pN=0; pN<allThinkinPlaceables.Count; pN++)
            {
                p = allThinkinPlaceables[pN];

                if(p.IsIdle()
                    && p.targetType != Placeable.PlaceableTarget.None)
                {
                    if(updateAllPlaceables
                        || p.timeToActNext <= Time.time)
                    {
                        bool targetFound = FindClosestInList(p.transform.position, GetAttackList(p.faction, p.targetType), out targetToPass);
                        if(!targetFound) Debug.LogError("No more targets!"); //this should only happen on Game Over
                        p.SetTarget(targetToPass);

                        p.timeToActNext = Time.time + THINKING_DELAY;
                    }
                }

				p.UpdateLoop();
            }

            updateAllPlaceables = false; //is set to true by UseCard()
        }

        private List<ThinkingPlaceable> GetAttackList(Placeable.Faction f, Placeable.PlaceableTarget t)
        {
            switch(t)
            {
                case Placeable.PlaceableTarget.Both:
                    return (f == Placeable.Faction.Player) ? allOpponents : allPlayers;
				case Placeable.PlaceableTarget.OnlyBuildings:
                    return (f == Placeable.Faction.Player) ? opponentBuildings : playerBuildings;
				default:
					Debug.LogError("What faction is this?? Not Player nor Opponent.");
					return null;
            }
        }

        private bool FindClosestInList(Vector3 p, List<ThinkingPlaceable> list, out ThinkingPlaceable t)
        {
            t = null;
            bool targetFound = false;
            float closestDistanceSqr = Mathf.Infinity; //anything closer than here becomes the new designated target

            for(int i=0; i<list.Count; i++)
            {                
				float sqrDistance = (p - list[i].transform.position).sqrMagnitude;
                if(sqrDistance < closestDistanceSqr)
                {
                    t = list[i];
                    closestDistanceSqr = sqrDistance;
                    targetFound = true;
                }
            }

            return targetFound;
        }

        private void UseCard(CardData cardData, Vector3 position, Placeable.Faction pFaction)
        {
            for(int pNum=0; pNum<cardData.placeablesData.Length; pNum++)
            {
                PlaceableData pDataRef = cardData.placeablesData[pNum];
                Quaternion rot = (pFaction == Placeable.Faction.Player) ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);
                GameObject newPlaceableGO = Instantiate<GameObject>(pDataRef.associatedPrefab, position + cardData.relativeOffsets[pNum], rot);
                
                SetupPlaceable(newPlaceableGO, pDataRef, pFaction);
            }

            updateAllPlaceables = true; //will force all AIBrains to update next time the Update loop is run
        }


        //setups all scripts and listeners on a Placeable GameObject
        private void SetupPlaceable(GameObject go, PlaceableData pDataRef, Placeable.Faction pFaction)
        {
            //Add the appropriate script
                switch(pDataRef.pType)
                {
                    case Placeable.PlaceableType.Unit:
                        Unit uScript = go.AddComponent<Unit>();
                        uScript.Activate(pFaction, pDataRef); //enables NavMeshAgent
                        AddPlaceableToList(uScript); //add the Unit to the appropriate list
						uScript.OnDealDamage += OnDealDamage;
                        break;

                    case Placeable.PlaceableType.Building:
                    case Placeable.PlaceableType.Castle:
                        Building bScript = go.AddComponent<Building>();
                        bScript.Activate(pFaction, pDataRef);
                        AddPlaceableToList(bScript); //add the Building to the appropriate list
						bScript.OnDealDamage += OnDealDamage;

                        //special case for castles
                        if(pDataRef.pType == Placeable.PlaceableType.Castle)
                        {
                            bScript.OnDie += OnCastleDead;
                        }
                        
                        navMesh.BuildNavMesh(); //rebake the Navmesh
                        break;

                    case Placeable.PlaceableType.Obstacle:
                        Obstacle oScript = go.AddComponent<Obstacle>();
                        oScript.Activate(pDataRef);
                        navMesh.BuildNavMesh(); //rebake the Navmesh
                        break;

                    case Placeable.PlaceableType.Spell:
                        //Spell sScript = newPlaceable.AddComponent<Spell>();
                        //sScript.Activate(pFaction, cardData.hitPoints);
                        //TODO: activate the spell and… ?
                        break;
                }

                go.GetComponent<Placeable>().OnDie += OnPlaceableDead;
        }

		private void OnCastleDead(Placeable p)
		{
			//TODO: implement better game over
            Debug.Log("Castle destroyed");
            p.OnDie -= OnCastleDead;
            gameOver = true; //stops the thinking loop
		}

		private void OnDealDamage(ThinkingPlaceable attacker, ThinkingPlaceable target, float amount)
		{
            Debug.Log(attacker.name + " dealing " + amount + " damage to " + target.name);
			target.SufferDamage(amount);
		}

        private void OnPlaceableDead(Placeable p)
        {
            p.OnDie -= OnPlaceableDead; //remove the listener
            
            switch(p.pType)
            {
                case Placeable.PlaceableType.Unit:
                    //remove Unit from the appropriate list
                    RemovePlaceableFromList((Unit)p);
                    break;

                case Placeable.PlaceableType.Building:
                    //remove Building from the appropriate list
                    RemovePlaceableFromList((Building)p);
                    navMesh.BuildNavMesh();
                    break;

                case Placeable.PlaceableType.Obstacle:
                    navMesh.BuildNavMesh();
                    break;

                case Placeable.PlaceableType.Spell:
                    //TODO: can spells die?
                    break;
            }
        }

        private void AddPlaceableToList(ThinkingPlaceable p)
        {
			allThinkinPlaceables.Add(p);

			if(p.faction == Placeable.Faction.Player)
            {
				allPlayers.Add(p);
            	
				if(p.pType == Placeable.PlaceableType.Unit)
                    playerUnits.Add(p);
				else
                    playerBuildings.Add(p);
            }
            else if(p.faction == Placeable.Faction.Opponent)
            {
				allOpponents.Add(p);
            	
				if(p.pType == Placeable.PlaceableType.Unit)
                    opponentUnits.Add(p);
				else
                    opponentBuildings.Add(p);
            }
            else
            {
                Debug.LogError("Error in adding a Placeable in one of the player/opponent lists");
            }
        }

        private void RemovePlaceableFromList(ThinkingPlaceable p)
        {
			allThinkinPlaceables.Remove(p);

			if(p.faction == Placeable.Faction.Player)
            {
				allPlayers.Remove(p);
            	
				if(p.pType == Placeable.PlaceableType.Unit)
                    playerUnits.Remove(p);
				else
                    playerBuildings.Remove(p);
            }
            else if(p.faction == Placeable.Faction.Opponent)
            {
				allOpponents.Remove(p);
            	
				if(p.pType == Placeable.PlaceableType.Unit)
                    opponentUnits.Remove(p);
				else
                    opponentBuildings.Remove(p);
            }
            else
            {
                Debug.LogError("Error in removing a Placeable from one of the player/opponent lists");
            }
        }
    }
}