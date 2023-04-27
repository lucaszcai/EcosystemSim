using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    public Species species;

    public Genes mother;
    public Genes father;

    // dino to instantiate when time is up
    public GameObject dino;

    public GameObject floor;

    Environment environment;

    float startTime;

    // required time to hatch
    public float hatchTime = 10;

    bool hatched = false;

    public void setParentGenes(Genes m, Genes f)
    {
        mother = m;
        father = f;
    }

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        floor = GameObject.Find("Environment");
        environment = floor.GetComponent<Environment>();
    }

    // Update is called once per frame
    void Update()
    {
        // check if hatch time has been reached
        if(!hatched && Time.time - startTime > hatchTime)
        {
            hatched = true;
            Hatch();
        }
    }

    void Hatch()
    {
        // create dino
        GameObject newDino = Instantiate(dino, transform.position, Quaternion.identity);

        environment.addDino(newDino);

        // update the dino's genes
        newDino.GetComponent<DinoBehavior>().reInitGenes(mother, father);

        // delete this
        Debug.Log("hatched");
        Destroy(gameObject);
    }
}
