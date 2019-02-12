using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRoyale
{
    [CreateAssetMenu(fileName = "NewCard", menuName = "Unity Royale/Card Data")]
    public class CardData : ScriptableObject
    {
        public float life = 5f; //the maximum life of the Placeable. Especially important for obstacle types, so they are removed after a while
        public PlaceableData[] placeablesData; //link to all the Placeables that this card spawns
        public Vector3[] relativeOffsets; //the relative offsets (from cursor) where the placeables will be dropped
    }
}