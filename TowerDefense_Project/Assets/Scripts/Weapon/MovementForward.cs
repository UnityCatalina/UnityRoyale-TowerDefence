using UnityEngine;

public class MovementForward : MonoBehaviour
{
    public GameObject burn;
    public float speed;
    public GameObject target,org;
    public string name;
    Vector3 dir;
    void Start()
    {
        target = GameObject.FindWithTag("Target");
       org= GameObject.FindWithTag(name);
        dir = (target.transform.position - org.transform.position).normalized;
        transform.forward = dir;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (transform.forward) * speed*Time.deltaTime;
    }
    void  OnTriggerEnter(Collider other)
    {
        if(burn)
            Instantiate( burn, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
