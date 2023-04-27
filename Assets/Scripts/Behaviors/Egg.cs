using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    public Species species;

    // dino to instantiate when time is up
    public GameObject dino;

    float startTime;

    // required time to hatch
    public float hatchTime = 10;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // check if hatch time has been reached
        if(Time.time - startTime > hatchTime)
        {
            // create dino
            Instantiate(dino, transform.position, Quaternion.identity);

            // delete this
            Debug.Log("hatched");
            Destroy(gameObject);
        }
    }
}
