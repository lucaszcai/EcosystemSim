using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBush : Organism
{
    public float health = 10;

    // Start is called before the first frame update
    void Start()
    {
        
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
            return true;
        }
        return false;
    }
}
