using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityRoyale
{
    //Base class for all objects that can be placed on the play area: units, obstacles, structures, etc.
    public class Placeable : MonoBehaviour
    {
        public PlaceableType pType;
        public UnityAction<Placeable> OnDie;

        public enum PlaceableType
        {
            Human,
            Obstacle,
            Building,
        }
    }
}