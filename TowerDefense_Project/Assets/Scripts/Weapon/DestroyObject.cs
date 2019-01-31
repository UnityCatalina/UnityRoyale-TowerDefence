using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    void Awake()
    {
        Destroy(this.gameObject, 2.0f);
    }

  
}
