using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PlayAnimOnKeyUp : MonoBehaviour
{

    public TrailRenderer trail;
    // Update is called once per frame
    private void Awake()
    {
        trail = this.transform.GetChild(0).GetComponent<TrailRenderer>();
    }
    void Update()
    {
        if(this.gameObject.transform.position.z<=1)
        {
            trail.Clear();
        }

    }

}
	
