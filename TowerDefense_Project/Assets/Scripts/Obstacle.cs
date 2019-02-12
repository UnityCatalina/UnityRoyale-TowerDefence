using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRoyale
{
    //A static, non-moving obstacle that disappears on its own after a while
    public class Obstacle : Placeable
    {
        public float life;

        private void Start()
        {
            StartCoroutine(Die());
        }

        private IEnumerator Die()
        {
            yield return new WaitForSeconds(life);

            if(OnDie != null)
                OnDie(this);

            Destroy(this.gameObject);
        }
    }
}
