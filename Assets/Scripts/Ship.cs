using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    
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

    private bool isBraking = false;

    [SerializeField]
    private Weapon[] weaponsArray;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        
    }


    public void HandleMovement()
    {
        if (!isBraking)
        {

            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(transform.right * forwardForce);
            }


            if (Input.GetKey(KeyCode.A))
            {
                rb.AddForce(transform.up * strafeForce);
            }


            if (Input.GetKey(KeyCode.D))
            {
                rb.AddForce(-transform.up * strafeForce);
            }


            if (Input.GetKey(KeyCode.S))
            {
                rb.AddForce(-transform.right * strafeForce);
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            isBraking = true;
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            isBraking = false;
        }
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
        ShieldManager();


    }

    public void AimShip()
    {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        Vector3 aimDirection = (mousePosition - transform.position).normalized;

        float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        float deltaAngle = Mathf.DeltaAngle(transform.eulerAngles.z, targetAngle);
        float maxRotation = rotationSpeed * Time.deltaTime;

        float rotation = Mathf.Clamp(deltaAngle, -maxRotation, maxRotation);
        transform.Rotate(Vector3.forward * rotation);
    }
    public void HandleShooting()
    {
        if (Input.GetMouseButton(0))
        {
            for (int i = 0; i < weaponsArray.Length; i++)
            {
                if (weaponsArray[i] != null)
                {
                    weaponsArray[i].Shoot();
                }
                
            }
        }
    }

    internal void TakeDamage(int damage)
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
        if(hp > 0 && !shieldActive )
        {
            hp -= damage;
            SoundManager.PlaySound(SoundManager.Sound.ShipHit);
        }
        if(hp <= 0)
        {
            SoundManager.PlaySound(SoundManager.Sound.ShipDeath);
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
}
