using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBush : Organism
{
    public float health = 10;
    public GameObject floor;

    Environment environment;

    // Start is called before the first frame update
    void Start()
    {
        floor = GameObject.Find("Environment");
        environment = floor.GetComponent<Environment>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool beEaten(float eatAmt)
    {
        health -= eatAmt;
        if(health <= 0)
        {
            // update our environments tile here
            environment.removeFood((int)(coord.x), (int)(coord.y));
            return true;
        }
        return false;
    }
}
