using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Organism : MonoBehaviour
{
    public Vector2 coord;
    protected bool dead;

    //public void Awake()
    //{
    //    // temp
    //    coord = new Vector2(0, 0);
    //}
    public void setCoord(int x, int y)
    {
        coord = new Vector2(x, y);
    }

    public void Die()
    {
        if (!dead)
        {
            dead = true;
            Debug.Log("destroying this object");
            // pause for 4 seconds before dying
            Destroy(gameObject);
        }
    }
}
