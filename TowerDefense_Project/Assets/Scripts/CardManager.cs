using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityRoyale
{
    public class CardManager : MonoBehaviour
    {
        public Camera mainCamera; //public reference
        public LayerMask playingFieldMask;
        public UnityAction<CardData, Vector3> OnCardUsed;
        public CardData fakeCardData; //TODO: REMOVE
        
        public RectTransform backupCardTransform; //the smaller card that sits in the deck
        public RectTransform cardsDashboard; //the UI panel that contains the actual playable cards
        public RectTransform cardsPanel; //the UI panel that contains all cards, the deck, and the dashboard (center aligned)

        public GameObject cardPrefab;
        public Card[] cards; //TODO: make private, when cards are generated dynamically
        
        private bool cardIsActive = false; //when true, a card is being dragged over the play field
        private GameObject previewCard;
        private Vector3 inputCreationOffset = new Vector3(0f, 0f, 2f); //offsets the creation of units so that they are not under the player's finger

        private void Awake()
        {
            previewCard = new GameObject("CardPreview");
        }

        private void Start()
        {
            //setup initial cards and listeners
            
            AddCardToDeck();
            for(int i=0; i<cards.Length; i++)
            {
                PromoteCardFromDeck(i);
            }
        }

        //adds a new card to the deck on the left, ready to be used
        private void AddCardToDeck() //TODO: pass in the CardData dynamically
        {
            backupCardTransform = Instantiate<GameObject>(cardPrefab, cardsPanel).GetComponent<RectTransform>();
            backupCardTransform.anchoredPosition = new Vector2(120f, 0f);
            backupCardTransform.localScale = Vector3.one * 0.7f;

            Card cardScript = backupCardTransform.GetComponent<Card>();
            cardScript.cardData = fakeCardData;
        }

        //moves the preview card from the deck to the active card dashboard
        private void PromoteCardFromDeck(int position)
        {
            backupCardTransform.SetParent(cardsDashboard, true);
            //move and scale into position
            backupCardTransform.anchoredPosition = new Vector2(220f * (position+1), 0f);
            backupCardTransform.localScale = Vector3.one;

            //store a reference to the Card component in the array
            Card cardScript = backupCardTransform.GetComponent<Card>();
            cardScript.cardId = position;
            cards[position] = cardScript;

            //setup listeners on Card events
            cardScript.OnTapDownAction += CardTapped;
            cardScript.OnDragAction += CardDragged;
            cardScript.OnTapReleaseAction += CardReleased;

            AddCardToDeck();
        }

        private void CardTapped(int cardId)
        {
        
        }

        private void CardDragged(int cardId, Vector2 dragAmount)
        {
            cards[cardId].transform.Translate(dragAmount);

            //raycasting to check if the card is on the play field
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            bool planeHit = Physics.Raycast(ray, out hit, Mathf.Infinity, playingFieldMask);

            if(planeHit)
            {
                if(!cardIsActive)
                {
                    cardIsActive = true;
                    previewCard.transform.position = hit.point;
                    cards[cardId].ChangeActiveState(true); //hide card

                    //retrieve arrays from the CardData
                    PlaceableData[] dataToSpawn = cards[cardId].cardData.placeablesData;
                    Vector3[] offsets = cards[cardId].cardData.relativeOffsets;

                    //spawn all the preview Placeables and parent them to the cardPreview
                    for(int i=0; i<dataToSpawn.Length; i++)
                    {
                        GameObject newPlaceable = GameObject.Instantiate<GameObject>(dataToSpawn[i].associatedPrefab,
                                                                                    hit.point + offsets[i] + inputCreationOffset,
                                                                                    Quaternion.identity,
                                                                                    previewCard.transform);
                    }
                }
                else
                {
                    //temporary copy has been created, we move it along with the cursor
                    previewCard.transform.position = hit.point;
                }
            }
            else
            {
                if(cardIsActive)
                {
                    cardIsActive = false;
                    cards[cardId].ChangeActiveState(false); //show card

                    ClearPreviewObjects();
                }
            }
        }

        private void CardReleased(int cardId)
        {
            //raycasting to check if the card is on the play field
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, playingFieldMask))
            {
                if(OnCardUsed != null)
                    OnCardUsed(cards[cardId].cardData, hit.point + inputCreationOffset); //GameManager picks this up to spawn the actual Placeable

                ClearPreviewObjects();
                Destroy(cards[cardId].gameObject); //remove the card itself
                PromoteCardFromDeck(cardId);
            }
            else
            {
                //cards[cardId].ChangeActiveState(false); //show card
                //TODO: return it to position
            }
        }

        //happens when the card is put down on the playing field, and while dragging (when moving out of the play field)
        private void ClearPreviewObjects()
        {
            //destroy all the preview Placeables
            for(int i=0; i<previewCard.transform.childCount; i++)
            {
                Destroy(previewCard.transform.GetChild(i).gameObject);
            }
        }
    }

}
