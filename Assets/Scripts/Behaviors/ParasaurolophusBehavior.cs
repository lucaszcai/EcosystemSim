using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ParasaurolophusBehavior : MonoBehaviour
{

    public GameObject closestWaterTile;
    BehaviorState currentState = BehaviorState.None;

    // these represent the urges
    [Header("Urges")]
    public float hunger;
    public float thirst;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(closestWaterTile != null)
        //{
        //    NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        //    navMeshAgent.SetDestination(closestWaterTile.transform.position);
        //}
    }
}
