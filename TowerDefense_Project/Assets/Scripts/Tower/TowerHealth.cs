using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHealth : MonoBehaviour
{
    public int health = 3;
    public float delay = 5;
   public float count;
   public float currentHealth;
   public  bool play;
    Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
        count = delay;
        currentHealth = health;
    }
    void OnTriggerEnter(Collider other)
    {
        if (currentHealth > 0)
            currentHealth -= 1;
        if (currentHealth <= 0 && !play)
        {
            animator.Play("MagicTower_Des",0,0);
            play = true;
        }

    }
    void Update()
    {
       if(play)
        {
            count -= Time.deltaTime;
            if(count<=0)
            {
                animator.Play("Base", 0, 0);
                currentHealth=health;
                count = delay;
                play = false;
            }
        }
    }
}
