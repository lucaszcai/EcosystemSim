using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DinoBehavior : Organism
{
    public Species species;

    public BehaviorState currentState;

    // these represent the urges
    [Header("Urges")]
    public float hunger;
    public float thirst;

    // reaching these causes death. this is the number of ticks/second (?)
    public float maxHunger = 40;
    public float maxThirst = 40;

    public float criticalHunger = 10;
    public float criticalThirst = 10;

    // action is defined as changing our currentState
    // set a cooldown on how quickly we can take action
    // wait 1 second before making a new action
    float actionCooldown = 1;
    float lastActionTime;

    float mateCoolDown = 10;
    float lastMateTime;
    float mateDuration = 3;
    float mateStart = -1;

    private SphereCollider sphereCollider;
    private NavMeshAgent navMeshAgent;
    private GameObject indicator;

    public Material maleColor;
    public Material femaleColor;
    public GameObject egg;

    // store closest game objects for nearest food and water. set to null after going to it
    GameObject foodTarget;

    GameObject waterTarget;
    Vector3 waterTargetEdge;

    GameObject mateTarget;
    Vector3 mateLocation;

    private Animator animator;

    public Genes genes;
    string[] traits = new string[] { "speed", "sense" };
    float[] defaultVals = new float[] { 5, 15 };

    // REMOVE THIS LATER
    public bool male;


    // awake called on instantiation
    void Awake()
    {
        hunger = 0;
        thirst = 0;

        currentState = BehaviorState.None;
        sphereCollider = GetComponent<SphereCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        indicator = transform.Find("Indicator").gameObject;

        // initialize our genes
        genes = new Genes(traits, defaultVals);

        // REMOVE THIS LATER
        genes.male = male;

        interpretGenes();
    }

    // look at our genes and do the modifiers
    void interpretGenes()
    {
        // set speed
        navMeshAgent.speed = genes.getVal("speed");
        sphereCollider.radius = genes.getVal("sense");
        if (genes.getMale())
        {
            indicator.GetComponent<Renderer>().material = maleColor;
        }
        else
        {
            indicator.GetComponent<Renderer>().material = femaleColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // update hunger and thirst
        hunger += Time.deltaTime * 1 / 2;
        thirst += Time.deltaTime * 1 / 3;

        float timeSinceLastAction = Time.time - lastActionTime;
        if(timeSinceLastAction > actionCooldown)
        {
            takeAction();
            lastActionTime = Time.time;
        }

        if(hunger > maxHunger)
        {
            currentState = BehaviorState.Dying;
            Die();
        }
        if(thirst > maxThirst)
        {
            currentState = BehaviorState.Dying;
            Die();
        }
    }

    // action changes our currentState
    void takeAction()
    {
        if(currentState == BehaviorState.Dying)
        {
            return;
        }
        // if we're already eating/drinking, we don't want to search for food/water
        if(currentState == BehaviorState.Eating || currentState == BehaviorState.Drinking || currentState == BehaviorState.GoingToMate || currentState == BehaviorState.Mating)
        {
        }
        else
        {
            // get nearby objects, pass them in
            Collider[] colliders = Physics.OverlapSphere(transform.position, sphereCollider.radius);

            // if both hunger and thirst are below critical, and male look for mate
            if (hunger < criticalHunger && thirst < criticalThirst && genes.getMale())
            {
                if (Time.time - lastMateTime > mateCoolDown)
                {
                    searchForMate();
                    makeMove();
                    return;
                }
            }


            // we want to take an action based on which of our urges are highest
            if (hunger > thirst)
            {
                // see if there is food nearby. if there is, go to it
                searchForFood(colliders);
            }
            else
            {
                // we want to see if there is water nearby. if there is, go to it
                searchForWater(colliders);
            }
        }


        // otherwise, we want to continue exploring
        // TODO: change this temporary testing
        //currentState = BehaviorState.Exploring;

        // make move based on our state
        makeMove();

    }

    public bool readyToMate()
    {
        return hunger < criticalHunger && thirst < criticalThirst && Time.time - lastMateTime > mateCoolDown && currentState != BehaviorState.GoingToMate && currentState != BehaviorState.Mating;
    }

    public bool mateRequested(GameObject other, Vector3 location)
    {
        //Debug.Log("received request from" + other.name);
        mateTarget = other;
        mateLocation = location;
        currentState = BehaviorState.GoingToMate;

        return true;
        // go to given location
    }

    bool searchForMate()
    {

        Collider[] nearby = Physics.OverlapSphere(transform.position, sphereCollider.radius);

        Debug.Log("nearby");
        Debug.Log(nearby);
        //return false;

        Collider closestCollider = null;
        float closestDist = Mathf.Infinity;
        Vector3 curPos = transform.position;

        foreach (Collider collider in nearby)
        {
            if(collider == null)
            {
                continue;
            }
            // find the neraest food
            if (collider.gameObject.CompareTag("Dino"))
            {
                // we know it's a dino
                // check if opposite gender
                if (!collider.gameObject.GetComponent<DinoBehavior>().genes.getMale() && collider.gameObject.GetComponent<DinoBehavior>().readyToMate())
                {
                    //Debug.Log("FOUND SUITABLE MATE");
                    Vector3 closestPointOnCollider = collider.ClosestPoint(curPos);
                    float distanceToCollider = Vector3.Distance(curPos, closestPointOnCollider);
                    if (distanceToCollider < closestDist)
                    {
                        closestCollider = collider;
                        closestDist = distanceToCollider;
                    }
                }
            }
        }


        // if we were able to find a mate, find a middle location and inform other
        if (closestCollider != null)
        {
            DinoBehavior other = closestCollider.gameObject.GetComponent<DinoBehavior>();
            //foodTarget = closestCollider.gameObject;
            //currentState = BehaviorState.GoingToFood;
            Vector3 midpoint = (transform.position + closestCollider.transform.position) / 2;
            closestCollider.gameObject.GetComponent<DinoBehavior>().mateRequested(gameObject, midpoint);

            // set the status of this
            currentState = BehaviorState.GoingToMate;
            mateLocation = midpoint;
            mateTarget = closestCollider.gameObject;


            return true;
        }
        // otherwise, set our state as exploring
        else
        {
            currentState = BehaviorState.Exploring;
            return false;
        }
    }

    void searchForFood(Collider[] nearby)
    {
        Collider closestCollider = null;
        float closestDist = Mathf.Infinity;
        Vector3 curPos = transform.position;

        foreach(Collider collider in nearby)
        {
            // find the neraest food
            if (collider.gameObject.CompareTag("Food"))
            {
                Vector3 closestPointOnCollider = collider.ClosestPoint(curPos);
                float distanceToCollider = Vector3.Distance(curPos, closestPointOnCollider);
                if (distanceToCollider < closestDist)
                {
                    closestCollider = collider;
                    closestDist = distanceToCollider;
                }
            }
        }

        // if we were able to find a food, update our target and our state
        if(closestCollider != null)
        {
            foodTarget = closestCollider.gameObject;
            currentState = BehaviorState.GoingToFood;
        }
        // otherwise, set our state as exploring
        else
        {
            currentState = BehaviorState.Exploring;
        }
    }

    void searchForWater(Collider[] nearby)
    {
        Collider closestCollider = null;
        float closestDist = Mathf.Infinity;
        Vector3 curPos = transform.position;

        foreach (Collider collider in nearby)
        {
            // find the nearest water
            if (collider.gameObject.CompareTag("Water"))
            {
                Vector3 closestPointOnCollider = collider.ClosestPoint(curPos);
                float distanceToCollider = Vector3.Distance(curPos, closestPointOnCollider);
                if (distanceToCollider < closestDist)
                {
                    closestCollider = collider;
                    closestDist = distanceToCollider;
                }
            }
        }

        // if we were able to find a food, update our target and our state
        if (closestCollider != null)
        {
            waterTarget = closestCollider.gameObject;
            currentState = BehaviorState.GoingToWater;
        }
        // otherwise, set our state as exploring
        else
        {
            currentState = BehaviorState.Exploring;
        }
    }

    void makeMove()
    {
        switch (currentState)
        {
            case BehaviorState.Exploring:
                // move to a random coord
                Vector3 coord = new Vector3(transform.position.x + Random.Range(5, 20), 0, transform.position.z + Random.Range(5, 20));
                moveToCoord(coord, false);
                break;
            case BehaviorState.GoingToFood:
                // if haven't set this food as a destination yet
                if(!(navMeshAgent.destination == foodTarget.transform.position))
                {
                    // move to food
                    moveToCoord(foodTarget.transform.position, true);
                }

                // check if we've arrived
                if (checkAtPos(foodTarget.transform.position))
                {
                    // if we're in range, stop moving
                    navMeshAgent.SetDestination(transform.position);
                    currentState = BehaviorState.Eating;
                }
                
                break;
            case BehaviorState.GoingToWater:
                // get water edge

                // if haven't set this pos as a destination yet
                if (waterTargetEdge == null || !(navMeshAgent.destination == waterTargetEdge))
                {
                    Vector3 waterEdge = getWaterEdgeCoord();
                    waterTargetEdge = waterEdge;
                    // move to food
                    moveToCoord(waterEdge, true);
                }

                // check if we've arrived
                if (checkAtPos(waterTargetEdge))
                {
                    navMeshAgent.SetDestination(transform.position);

                    currentState = BehaviorState.Drinking;

                }
                break;
            case BehaviorState.GoingToMate:
                // get water edge

                // if haven't set this pos as a destination yet
                if ( !(navMeshAgent.destination == mateLocation))
                {
                    moveToCoord(mateLocation, true);
                }

                // check if we've arrived
                if (checkAtPos(mateLocation))
                {
                    navMeshAgent.SetDestination(transform.position);

                    currentState = BehaviorState.Mating;

                }
                break;
            case BehaviorState.Eating:
                handleInteractions();
                break;
            case BehaviorState.Drinking:
                handleInteractions();
                break;
            case BehaviorState.Mating:
                handleInteractions();
                break;
        }

        handleAnimations();
    }

    Vector3 getWaterEdgeCoord()
    {
        Vector3 agentPosition = navMeshAgent.transform.position;
        Vector3 center = waterTarget.transform.position;
        Vector3 direction = agentPosition - center;
        Vector3 unitDirection = direction.normalized;
        float halfWidth = 21f;
        Vector3 edgePosition = center + (unitDirection * halfWidth);

        return edgePosition;
    }

    // move to a position
    void moveToCoord(Vector3 coord, bool priority)
    {
        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();

        // only set a new location when were done walking
        if(!navMeshAgent.hasPath || priority)
        {
            navMeshAgent.SetDestination(coord);
        }
        
    }

    bool checkAtSquare(Vector2 coords)
    {
        // check if we're in eating range
        Vector2 mapCoords = new Vector2(transform.position.x, transform.position.z);
        Vector2 dist = foodTarget.GetComponent<FoodBush>().coord - mapCoords;


        return Mathf.Abs(dist.x) < 2 && Mathf.Abs(dist.y) < 2;
    }

    bool checkAtPos(Vector3 coords)
    {
        Vector3 dist = transform.position - coords;
        return Mathf.Abs(dist.x) < 4 && Mathf.Abs(dist.z) < 4;
    }

    void handleInteractions()
    {
        //Debug.Log("handling interactions");
        if(currentState == BehaviorState.Eating)
        {
            Eat();

        }
        else if(currentState == BehaviorState.Drinking)
        {
            Drink();

        }
        else if(currentState == BehaviorState.Mating)
        {
            // both call mate
            //Debug.Log("calling mate");
            Mate();
        }
    }

    void Mate()
    {
        // first iteration
        if(mateStart == -1)
        {
            Debug.Log("first mate iteration");
            mateStart = Time.time;
        }

        // duration reached
        if(Time.time - mateStart >= mateDuration)
        {
            // only female lays egg
            if (!genes.getMale())
            {
                GameObject newEgg = Instantiate(egg, transform.position, Quaternion.identity);
            }

            Debug.Log("MATED");

            // reset mate stuff
            mateTarget = null;
            mateLocation = Vector3.zero;

            lastMateTime = Time.time;
            // reset state
            currentState = BehaviorState.None;
        }
        else
        {
            Debug.Log("unfinished iterations");
        }
    }

    void handleAnimations()
    {
        switch (currentState)
        {
            case BehaviorState.None:
                break;
            case BehaviorState.Exploring:
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
                break;
            case BehaviorState.GoingToFood:
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);

                break;
            case BehaviorState.GoingToWater:
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);

                break;
            case BehaviorState.Eating:
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);
                break;
            case BehaviorState.Drinking:
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);
                break;
            case BehaviorState.Mating:
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);
                break;
            case BehaviorState.Dying:
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isDead", true);
                break;
        }
    }

    void Eat()
    {
        float eatAmt = 5;
        // eat if we're hungry
        if(hunger > eatAmt)
        {
            FoodBush foodbush = foodTarget.GetComponent<FoodBush>();

            hunger -= eatAmt;
            bool eaten = foodbush.beEaten(eatAmt);

            // check if we ate all of the foodbush
            if (eaten)
            {
                foodbush.Die();
                // reset our foodTarget
                foodTarget = null;
                // rest our state
                currentState = BehaviorState.None;
            }
        }
    }

    void Drink()
    {
        float drinkAmt = 5;

        if(thirst > drinkAmt)
        {
            thirst -= drinkAmt;
        }
        // change this to be a critical thirs amount
        else
        {
            currentState = BehaviorState.None;
        }
    }


}
