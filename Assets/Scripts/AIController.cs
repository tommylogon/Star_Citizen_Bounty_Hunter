using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path;
using UnityEngine;

public class AIController : MonoBehaviour, IController
{
    public Transform shipTransform;
    public Vector3 spawnPosition;
    public Ship ship;

    public Radar radar;
    public Radar FireArea;

    public event Action OnDeath;
    public event Action OnShoot;
    public event Action<Ship.Direction> OnMove;
    public event Action<bool> OnBrake;
    public event Action<Vector3, Vector3> OnAim;

    public int minDistanceToTarget = 10, maxDistanceToTarget = 20;

    public State aiState;
    bool hasTarget;

    // Start is called before the first frame update
    void Start()
    {
        tag = "Enemy";
        spawnPosition = UnityEngine.Random.insideUnitCircle * 50;
               
        if (shipTransform.TryGetComponent(out Ship myShip))
        {
            shipTransform.tag = "Enemy";

                ship = myShip;
                ship.Setup(gameObject);
            ship.OnDeath += AIController_OnDeath;
        }
        else
        {
            Debug.Log("AI does not have a ship");
        }
        if(radar == null)
        {
            radar = GetComponent<Radar>();
        }
        Setup();
        aiState = State.Alive;

    }

    private void AIController_OnDeath()
    {
        aiState = State.Dead;
    }

    public void Setup()
    {
        radar.OnRadarStay += AimAtRadarContact;
        radar.OnRadarStay += MoveDependingOnRadarContact;
        radar.OnRadarExit += Radar_OnRadarExit;

        FireArea.OnRadarStay += ShootAtTarget;
    }

    private void Radar_OnRadarExit(Collider2D radarContact)
    {
        if(radarContact.tag == "Player")
        {
            hasTarget = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        if(aiState== State.Alive && !hasTarget)
        {
            float distance = (transform.position - spawnPosition).magnitude;
            if(distance > minDistanceToTarget) 
            {
                OnAim?.Invoke(spawnPosition, new Vector3(0,0,0));
                OnMove?.Invoke(Ship.Direction.Forward);
            }
            else
            {
                OnBrake?.Invoke(true);
            }
                
        }
        
    }

    public void AimAtRadarContact(Collider2D radarContact)
    {
        
        if(aiState == State.Alive && radarContact.tag == "Player")
        {
            hasTarget = true;

            OnAim?.Invoke(radarContact.transform.position, radarContact.attachedRigidbody.velocity);
        }
    }

    public void MoveDependingOnRadarContact(Collider2D radarContact)
    {
        if (aiState == State.Alive)
        {
            Vector2 heading = transform.position - radarContact.transform.position;
            float distance = (heading).magnitude;
            float rotation = AngleDir(transform.forward, heading, transform.up);
            
            if (radarContact.tag == "Player")
            {

                if (distance < minDistanceToTarget)
                {
                    OnMove(Ship.Direction.Backward);
                }
                else if (distance > maxDistanceToTarget)
                {
                    OnMove(Ship.Direction.Forward);
                }
            }
            else if(radarContact.tag == "Enemy" && radarContact.name != ship.name)
            {
                if(rotation > 0)
                {
                   //Debug.Log(name + " is to the left of " + radarContact.name);
                }
                if(rotation < 0)
                {
                   // Debug.Log(this.name + " is to the right of " + radarContact.name);
                }
                if(distance > minDistanceToTarget && (spawnPosition- radarContact.transform.position).magnitude < minDistanceToTarget)
                {
                    spawnPosition = UnityEngine.Random.insideUnitSphere * 50;
                }
            }
        }

    }

    public void ShootAtTarget(Collider2D target)
    {
        if(aiState == State.Alive &&  target.tag == "Player")
        {
            OnShoot?.Invoke();
        }
    }
    public void UpdatePosition()
    {
        if (aiState == State.Alive && shipTransform != null)
        {

            transform.position = shipTransform.position;
            transform.rotation = shipTransform.rotation;
            
        }
    }

    float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

}
