using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using CodeMonkey.Utils;
using System;

public class Radar : MonoBehaviour
{
    public Collider2D colliderArea;
    //public event Action<Collider2D> OnRadarEnter;
    public event Action<Collider2D> OnRadarStay;
    public event Action<Collider2D> OnRadarExit;

    void Start()
    {
        if(colliderArea == null)
        {
            colliderArea = GetComponent<CircleCollider2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
       // OnRadarEnter?.Invoke(other);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        OnRadarExit?.Invoke(collision);
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        OnRadarStay?.Invoke(other);
        
        UtilsClass.DebugDrawCircle(other.gameObject.transform.position, 1f, Color.red, 0.1f, 5);
    }
}
