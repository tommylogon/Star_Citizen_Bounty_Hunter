using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightSystem : MonoBehaviour
{
    [SerializeField] private float forwardForce = 9f;
    [SerializeField] private float strafeForce = 0.6f;
    [SerializeField] private float brakingForce = 2.2f;
    [SerializeField] private float maxSpeed = 13f;
    [SerializeField] private float rotationSpeed = 156f;
    [SerializeField] private float velocity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
