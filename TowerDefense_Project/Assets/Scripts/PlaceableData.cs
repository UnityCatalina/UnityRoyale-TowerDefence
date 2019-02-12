using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRoyale
{
    [CreateAssetMenu(fileName = "NewPlaceable", menuName = "Unity Royale/Placeable Data")]
    public class PlaceableData : ScriptableObject
    {
        public Placeable.PlaceableType pType;

        public GameObject associatedPrefab;
    }
}