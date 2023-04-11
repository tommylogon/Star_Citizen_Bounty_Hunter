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

    [SerializeField] private int hp = 100;
    [SerializeField] private int maxHp = 100;


    [SerializeField] private int shield = 100;
    [SerializeField] private int maxShield = 100;
    [SerializeField] private int shieldRegen = 1;
    [SerializeField] private float shieldRegenDelay = 5f;
    [SerializeField] private float shieldRegenTimer = 0f;
    [SerializeField] private bool shieldActive = true;


    [SerializeField] private float forwardForce = 9f;
    [SerializeField] private float strafeForce = 0.6f;
    [SerializeField] private float brakingForce = 2.2f;
    [SerializeField] private float maxSpeed = 13f;
    [SerializeField] private float rotationSpeed = 156f;
    [SerializeField] private float velocity;

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

    private void FixedUpdate()
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
        ShieldManager();


    }

    public void AimShip(Vector3 aimPosition)
    {
        
        Vector3 aimDirection = (aimPosition - transform.position).normalized;

        float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        float deltaAngle = Mathf.DeltaAngle(transform.eulerAngles.z, targetAngle);
        float maxRotation = rotationSpeed * Time.deltaTime;

        float rotation = Mathf.Clamp(deltaAngle, -maxRotation, maxRotation);
        transform.Rotate(Vector3.forward * rotation);
    }
    public void HandleShooting(Vector3 shootPosition)
    {
        
            for (int i = 0; i < weaponsArray.Length; i++)
            {
                if (weaponsArray[i] != null)
                {
                    weaponsArray[i].Shoot(shootPosition, targetTag);
                }
                
            }
        
    }

    public void Damage(int damage)
    {
        if(shieldActive && shield > 0)
        {
            shield -= damage;
            shieldRegenTimer = 0;
            SoundManager.PlaySound(SoundManager.Sound.ShieldHit);

        }
        if(shield <= 0)
        {
            shieldActive = false;
        }
        if(hp > 0 && !shieldActive  )
        {
            hp -= damage;
            SoundManager.PlaySound(SoundManager.Sound.ShipHit);
        }
        if(hp <= 0)
        {
            SoundManager.PlaySound(SoundManager.Sound.ShipDeath);
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    public void ShieldManager()
    {
        if (shield < maxShield)
        {
            shieldRegenTimer += Time.deltaTime;


            if (shield > maxShield)
            {
                shield = maxShield;
            }
            if (shieldActive && shieldRegenTimer > shieldRegenDelay)
            {
                shield += shieldRegen;

            }
            else if (!shieldActive)
            {
                shield += shieldRegen;
                if (shield == maxShield)
                {
                    shieldActive = true;
                }
            }
        }
    }

    public void RepairShipHp(int repairHp)
    {
        if (hp > 0 && hp < maxHp)
        {
            hp += repairHp;
        }
    }
}
