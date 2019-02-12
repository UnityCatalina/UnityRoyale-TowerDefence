using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UnityRoyale
{
    public class GameManager : MonoBehaviour
    {
        public CardData testCard;
        public NavMeshSurface navMesh;

        private CardManager cardManager;
        private InputManager inputManager;

        private void Awake()
        {
            cardManager = GetComponent<CardManager>();
            inputManager = GetComponent<InputManager>();

            cardManager.OnCardUsed += UseCard;
        }

        private void Start()
        {
            
        }

        private void UseCard(CardData cardData, Vector3 position)
        {
            for(int unitNum=0; unitNum<cardData.placeablesData.Length; unitNum++)
            {
                PlaceableData unitDataRef = cardData.placeablesData[unitNum];
                GameObject newUnit = Instantiate<GameObject>(unitDataRef.associatedPrefab, position + cardData.relativeOffsets[unitNum], Quaternion.identity);
                
                //Add the appropriate script
                switch(unitDataRef.pType)
                {
                    case Placeable.PlaceableType.Obstacle:
                        Obstacle oScript = newUnit.AddComponent<Obstacle>();
                        oScript.pType = Placeable.PlaceableType.Obstacle;
                        oScript.life = cardData.life;
                        break;

                    case Placeable.PlaceableType.Human:
                        Unit uScript = newUnit.GetComponent<Unit>();
                        uScript.Activate(); //enables NavMeshAgent, then Unit finds a target
                        break;
                }

                newUnit.GetComponent<Placeable>().OnDie += OnUnitDead;
                
                if(unitDataRef.pType != Placeable.PlaceableType.Human)
                {
                    StartCoroutine(BuildNavmesh()); //rebake the Navmesh
                }
            }
        }

        private IEnumerator BuildNavmesh()
        {
            yield return new WaitForEndOfFrame();

            navMesh.BuildNavMesh();
        }

        private void OnUnitDead(Placeable p)
        {
            p.OnDie -= OnUnitDead; //remove the listener
            
            if(p.pType != Placeable.PlaceableType.Human)
            {
                StartCoroutine(BuildNavmesh()); //rebake the Navmesh
            }
        }
    }
}