using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityRoyale
{
    public class HealthBar : MonoBehaviour
    {
        public RectTransform bar;
        public GameObject wholeWidget;

        private bool isHidden = true;
        private float originalHP;
        private float currentHP;
        private Transform transformToFollow;

		private Color red = new Color(.83f, .28f, .28f);
		private Color blue = new Color(.27f, .43f, .83f);

        public void Initialise(ThinkingPlaceable p)
        {
            originalHP = currentHP = p.hitPoints;
            transformToFollow = p.transform;

            bar.GetComponent<Image>().color = (p.faction == Placeable.Faction.Player) ? red : blue;
            
            wholeWidget.transform.localPosition = new Vector3(0f, (p.pType == Placeable.PlaceableType.Building) ? 4.5f : 2f, 0f); //set the vertical position based on the type of Placeable
            wholeWidget.SetActive(false);
        }

        public void SetHealth(float newHP)
        {
            if(isHidden)
            {
                wholeWidget.SetActive(true);
                isHidden = false;
            }

            float ratio = 0f;
            if(newHP > 0f)
            {
                ratio = newHP/originalHP;
            }

            bar.localScale = new Vector3(ratio, 1f, 1f);
        }

        public void Move()
        {
            if(transformToFollow != null)
                transform.position = transformToFollow.position;
        }
    }
}