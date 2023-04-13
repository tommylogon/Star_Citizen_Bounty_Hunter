using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour, IDamageable
{
    public enum Direction
    {
        Forward, Backward,Left,Right
    }

    private string targetTag;

    [SerializeField] private Resource health;
    [SerializeField] private Resource shield;

    [SerializeField] private float forwardForce = 9f;
    [SerializeField] private float strafeForce = 0.6f;
    [SerializeField] private float brakingForce = 2.2f;
    [SerializeField] private float maxSpeed = 13f;
    [SerializeField] private float rotationSpeed = 156f;
    [SerializeField] private float velocity;

    [SerializeField] private bool staticPosition;
    [SerializeField] private Transform PositionTransform;

    public event Action OnDeath;

    private bool isBraking = false;

    [SerializeField]
    private Weapon[] weaponsArray;

    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        
        
    }

    public void Setup(GameObject controllerGameobject)
    {
        if(controllerGameobject.TryGetComponent(out IController controller)){
            controller.OnBrake += OnShipBreak;
            controller.OnMove += HandleMovement;
            controller.OnShoot += HandleShooting;
            controller.OnAim += AimShip;
        }
        switch
            (controllerGameobject.tag)
        {
            case "Player":
                targetTag = "Enemy";
                break;
            case "Enemy":
                targetTag = "Player";
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void UnsubscribeEvents()
    {
        if (TryGetComponent(out IController controller))
        {
            controller.OnBrake -= OnShipBreak;
            controller.OnMove -= HandleMovement;
            controller.OnShoot -= HandleShooting;
            controller.OnAim -= AimShip;
        }
    }

    public void HandleMovement(Direction direction)
    {
        if (!isBraking)
        {

            if (direction == Direction.Forward)
            {
                rb.AddForce(transform.right * forwardForce);
            }


            if (direction == Direction.Left)
            {
                rb.AddForce(transform.up * strafeForce);
            }


            if (direction == Direction.Right)
            {
                rb.AddForce(-transform.up * strafeForce);
            }


            if (direction == Direction.Backward)
            {
                rb.AddForce(-transform.right * strafeForce);
            }
        }
        
    }

    public void OnShipBreak(bool isBraking)
    {
        this.isBraking = isBraking;
    }

    private void Update()
    {

        // Limit velocity
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        velocity = rb.velocity.magnitude;

        // Braking
        if (isBraking)
        {
            rb.AddForce(-rb.velocity.normalized * brakingForce);
        }
        if(velocity < 0.1)
        {
            rb.velocity = new Vector2(0,0);
            isBraking = false;
        }
        


    }

    public void AimShip(Vector3 aimPosition, Vector3 targetVelocity)
    {
        
        

        Vector3 predictedTargetPosition = aimPosition + targetVelocity * Time.deltaTime*10;

        Vector3 predictedAimDirection = (predictedTargetPosition - transform.position).normalized;

        float targetAngle = Mathf.Atan2(predictedAimDirection.y, predictedAimDirection.x) * Mathf.Rad2Deg;
        float deltaAngle = Mathf.DeltaAngle(transform.eulerAngles.z, targetAngle);
        float maxRotation = rotationSpeed * Time.deltaTime;

        float rotation = Mathf.Clamp(deltaAngle, -maxRotation, maxRotation);
        transform.Rotate(Vector3.forward * rotation);
    }
    public void HandleShooting()
    {
        
            for (int i = 0; i < weaponsArray.Length; i++)
            {
                if (weaponsArray[i] != null)
                {
                    weaponsArray[i].Shoot(targetTag);
                }
                
            }
        
    }

    public void Damage(int damage)
    {
        if(shield != null && health != null)
        {
            if (shield.IsActive() && shield.GetValue() > 0)
            {
                shield.UpdateValue(-damage);
                shield.ResetRefillTimer();
                SoundManager.PlaySound(SoundManager.Sound.ShieldHit);

            }
            if (shield.GetValue() <= 0)
            {
                shield.SetActive(false);
            }
            if (health.GetValue() > 0 && !shield.IsActive())
            {
                health.UpdateValue(-damage);
                SoundManager.PlaySound(SoundManager.Sound.ShipHit);
            }
            if (health.GetValue() <= 0)
            {
                SoundManager.PlaySound(SoundManager.Sound.ShipDeath);
                OnDeath?.Invoke();
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log(name + " is missing components.");
        }

    }

    

    public void RepairShipHp(int repairHp)
    {
        if (health.IsValueBetweenMinAndMax())
        {
            health.UpdateValue(repairHp);
        }
    }
}
